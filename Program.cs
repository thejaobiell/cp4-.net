using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SafeAlertApi.Data;
using SafeAlertApi.Models;
using SafeAlertApi.DTOs;
using System.Text.Json.Serialization;
using SafeAlertApi.Swagger;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);


Env.Load(); // carrega o .env

var username = Environment.GetEnvironmentVariable("DB_USER") ?? "";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "";
var service = Environment.GetEnvironmentVariable("DB_SERVICE") ?? "";

var urlConexaoOracle = $"User Id={username};Password={password};Data Source={host}:{port}/{service}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(urlConexaoOracle));

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SafeAlert API",
        Version = "GLOBAL SOLUTION 1",
        Description = "SafeAlert é uma plataforma para monitoramento, alerta e comunicação em tempo real sobre eventos extremos. Esta API oferece CRUDs para gerenciamento de usuários, localidades, postagens e ocorrências."
    });

    c.DocumentFilter<TagOrderDocumentFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SafeAlert API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

#region USUÁRIOS

app.MapGet("/usuarios", async (AppDbContext db) =>
    await db.Usuarios
        .Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Endereco = u.Endereco,
            Tipo_usuario = u.Tipo_usuario,
            Data_cadastro = u.Data_cadastro
        }).ToListAsync())
    .WithTags("Usuários")
    .WithMetadata(new
    {
        Summary = "Retorna a lista de todos os usuários.",
        Description = "Retorna uma coleção completa de usuários cadastrados na plataforma SafeAlert."
    });

app.MapGet("/usuarios/{id}", async (int id, AppDbContext db) =>
{
    var dto = await db.Usuarios
        .Where(u => u.Id == id)
        .Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Endereco = u.Endereco,
            Tipo_usuario = u.Tipo_usuario,
            Data_cadastro = u.Data_cadastro
        }).FirstOrDefaultAsync();

    return dto != null ? Results.Ok(dto) : Results.NotFound();
})
.WithTags("Usuários")
.WithMetadata(new
{
    Summary = "Retorna um usuário específico pelo ID.",
    Description = "Consulta e retorna os detalhes de um usuário com base no ID fornecido."
});

app.MapPost("/usuarios", async (Usuario usuario, AppDbContext db) =>
{
    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();

    var dto = new UsuarioDto
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Endereco = usuario.Endereco,
        Tipo_usuario = usuario.Tipo_usuario,
        Data_cadastro = usuario.Data_cadastro
    };

    return Results.Created($"/usuarios/{usuario.Id}", dto);
})
.WithTags("Usuários")
.WithMetadata(new
{
    Summary = "Cria um novo usuário.",
    Description = "Registra um novo usuário na plataforma SafeAlert com os dados fornecidos."
});

app.MapPut("/usuarios/{id}", async (int id, Usuario input, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario == null) return Results.NotFound();

    usuario.Nome = input.Nome;
    usuario.Email = input.Email;
    usuario.Senha = input.Senha;
    usuario.Endereco = input.Endereco;
    usuario.Tipo_usuario = input.Tipo_usuario;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Usuários")
.WithMetadata(new
{
    Summary = "Atualiza os dados de um usuário existente.",
    Description = "Modifica as informações do usuário identificado pelo ID fornecido."
});

app.MapDelete("/usuarios/{id}", async (int id, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario == null) return Results.NotFound();

    db.Usuarios.Remove(usuario);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Usuários")
.WithMetadata(new
{
    Summary = "Remove um usuário pelo ID.",
    Description = "Exclui o usuário da base de dados com o ID especificado."
});

#endregion

#region LOCALIDADES

app.MapGet("/localidades", async (AppDbContext db) =>
    await db.Localidades
        .Select(l => new LocalidadeDto
        {
            Id = l.Id,
            Bairro = l.Bairro ?? "",
            Zona = l.Zona ?? ""
        }).ToListAsync())
    .WithTags("Localidades")
    .WithMetadata(new
    {
        Summary = "Retorna a lista de todas as localidades.",
        Description = "Consulta todas as localidades cadastradas na plataforma."
    });

app.MapGet("/localidades/{id}", async (int id, AppDbContext db) =>
{
    var dto = await db.Localidades
        .Where(l => l.Id == id)
        .Select(l => new LocalidadeDto
        {
            Id = l.Id,
            Bairro = l.Bairro ?? "",
            Zona = l.Zona ?? ""
        }).FirstOrDefaultAsync();

    return dto != null ? Results.Ok(dto) : Results.NotFound();
})
.WithTags("Localidades")
.WithMetadata(new
{
    Summary = "Retorna uma localidade pelo ID.",
    Description = "Consulta uma localidade específica pelo seu identificador."
});

app.MapPost("/localidades", async (Localidade input, AppDbContext db) =>
{
    db.Localidades.Add(input);
    await db.SaveChangesAsync();

    var dto = new LocalidadeDto
    {
        Id = input.Id,
        Bairro = input.Bairro ?? "",
        Zona = input.Zona ?? ""
    };

    return Results.Created($"/localidades/{input.Id}", dto);
})
.WithTags("Localidades")
.WithMetadata(new
{
    Summary = "Cria uma nova localidade.",
    Description = "Registra uma nova localidade na plataforma SafeAlert."
});

app.MapPut("/localidades/{id}", async (int id, Localidade input, AppDbContext db) =>
{
    var entity = await db.Localidades.FindAsync(id);
    if (entity == null) return Results.NotFound();

    entity.Bairro = input.Bairro;
    entity.Zona = input.Zona;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Localidades")
.WithMetadata(new
{
    Summary = "Atualiza uma localidade existente.",
    Description = "Modifica os dados de uma localidade pelo ID informado."
});

app.MapDelete("/localidades/{id}", async (int id, AppDbContext db) =>
{
    var entity = await db.Localidades.FindAsync(id);
    if (entity == null) return Results.NotFound();

    db.Localidades.Remove(entity);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Localidades")
.WithMetadata(new
{
    Summary = "Remove uma localidade pelo ID.",
    Description = "Exclui a localidade cadastrada com o ID especificado."
});

#endregion

#region EVENTOS

app.MapGet("/eventos", async (AppDbContext db) =>
    await db.Eventos
        .Select(e => new EventoDto
        {
            Id = e.Id,
            Tipo = e.Tipo ?? "",
            Descricao = e.Descricao ?? ""
        }).ToListAsync())
    .WithTags("Eventos")
    .WithMetadata(new
    {
        Summary = "Retorna todos os eventos cadastrados.",
        Description = "Lista todos os eventos monitorados pela plataforma SafeAlert."
    });

app.MapGet("/eventos/{id}", async (int id, AppDbContext db) =>
{
    var dto = await db.Eventos
        .Where(e => e.Id == id)
        .Select(e => new EventoDto
        {
            Id = e.Id,
            Tipo = e.Tipo ?? "",
            Descricao = e.Descricao ?? ""
        }).FirstOrDefaultAsync();

    return dto != null ? Results.Ok(dto) : Results.NotFound();
})
.WithTags("Eventos")
.WithMetadata(new
{
    Summary = "Retorna um evento específico pelo ID.",
    Description = "Consulta os detalhes de um evento identificado pelo ID."
});

app.MapPost("/eventos", async (Evento evento, AppDbContext db) =>
{
    db.Eventos.Add(evento);
    await db.SaveChangesAsync();

    var dto = new EventoDto
    {
        Id = evento.Id,
        Tipo = evento.Tipo ?? "",
        Descricao = evento.Descricao ?? ""
    };

    return Results.Created($"/eventos/{evento.Id}", dto);
})
.WithTags("Eventos")
.WithMetadata(new
{
    Summary = "Cria um novo evento.",
    Description = "Registra um evento que pode ser monitorado e notificado."
});

app.MapPut("/eventos/{id}", async (int id, Evento input, AppDbContext db) =>
{
    var entity = await db.Eventos.FindAsync(id);
    if (entity == null) return Results.NotFound();

    entity.Tipo = input.Tipo;
    entity.Descricao = input.Descricao;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Eventos")
.WithMetadata(new
{
    Summary = "Atualiza um evento existente.",
    Description = "Modifica as informações de um evento pelo ID informado."
});

app.MapDelete("/eventos/{id}", async (int id, AppDbContext db) =>
{
    var entity = await db.Eventos.FindAsync(id);
    if (entity == null) return Results.NotFound();

    db.Eventos.Remove(entity);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Eventos")
.WithMetadata(new
{
    Summary = "Remove um evento pelo ID.",
    Description = "Exclui o evento cadastrado com o ID especificado."
});

#endregion

#region POSTAGENS

app.MapGet("/postagens", async (AppDbContext db) =>
    await db.Postagens
        .Select(p => new PostagemDto
        {
            Id = p.Id,
            Usuario_id = p.Usuario_id,
            Evento_id = p.Evento_id,
            Localidade_id = p.Localidade_id,
            Titulo = p.Titulo ?? "",
            Descricao = p.Descricao ?? "",
            Imagem_url = p.Imagem_url ?? "",
            Data_criacao = p.Data_criacao
        }).ToListAsync())
    .WithTags("Postagens")
    .WithMetadata(new
    {
        Summary = "Retorna todas as postagens.",
        Description = "Lista todas as postagens realizadas pelos usuários na plataforma."
    });

app.MapGet("/postagens/{id}", async (int id, AppDbContext db) =>
{
    var dto = await db.Postagens
        .Where(p => p.Id == id)
        .Select(p => new PostagemDto
        {
            Id = p.Id,
            Usuario_id = p.Usuario_id,
            Evento_id = p.Evento_id,
            Localidade_id = p.Localidade_id,
            Titulo = p.Titulo ?? "",
            Descricao = p.Descricao ?? "",
            Imagem_url = p.Imagem_url ?? "",
            Data_criacao = p.Data_criacao
        }).FirstOrDefaultAsync();

    return dto != null ? Results.Ok(dto) : Results.NotFound();
})
.WithTags("Postagens")
.WithMetadata(new
{
    Summary = "Retorna uma postagem pelo ID.",
    Description = "Consulta uma postagem específica realizada na plataforma."
});

app.MapPost("/postagens", async (Postagem postagem, AppDbContext db) =>
{
    db.Postagens.Add(postagem);
    await db.SaveChangesAsync();

    var dto = new PostagemDto
    {
        Id = postagem.Id,
        Usuario_id = postagem.Usuario_id,
        Evento_id = postagem.Evento_id,
        Localidade_id = postagem.Localidade_id,
        Titulo = postagem.Titulo ?? "",
        Descricao = postagem.Descricao ?? "",
        Imagem_url = postagem.Imagem_url ?? "",
        Data_criacao = postagem.Data_criacao
    };

    return Results.Created($"/postagens/{postagem.Id}", dto);
})
.WithTags("Postagens")
.WithMetadata(new
{
    Summary = "Cria uma nova postagem.",
    Description = "Registra uma postagem de um usuário com relação a um evento e localidade."
});

app.MapPut("/postagens/{id}", async (int id, Postagem input, AppDbContext db) =>
{
    var entity = await db.Postagens.FindAsync(id);
    if (entity == null) return Results.NotFound();

    entity.Titulo = input.Titulo;
    entity.Descricao = input.Descricao;
    entity.Imagem_url = input.Imagem_url;
    entity.Usuario_id = input.Usuario_id;
    entity.Evento_id = input.Evento_id;
    entity.Localidade_id = input.Localidade_id;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Postagens")
.WithMetadata(new
{
    Summary = "Atualiza uma postagem existente.",
    Description = "Modifica os dados de uma postagem pelo ID informado."
});

app.MapDelete("/postagens/{id}", async (int id, AppDbContext db) =>
{
    var entity = await db.Postagens.FindAsync(id);
    if (entity == null) return Results.NotFound();

    db.Postagens.Remove(entity);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Postagens")
.WithMetadata(new
{
    Summary = "Remove uma postagem pelo ID.",
    Description = "Exclui uma postagem cadastrada com o ID especificado."
});

#endregion

#region OCORRÊNCIAS

app.MapGet("/ocorrencias", async (AppDbContext db) =>
    await db.Ocorrencias
        .Select(o => new OcorrenciaDto
        {
            Id = o.Id,
            Postagem_id = o.Postagem_id,
            Status = o.Status ?? "",
            Data_ocorrencia = o.Data_ocorrencia
        }).ToListAsync())
    .WithTags("Ocorrências")
    .WithMetadata(new
    {
        Summary = "Retorna todas as ocorrências registradas.",
        Description = "Lista todas as ocorrências vinculadas às postagens na plataforma SafeAlert."
    });

app.MapGet("/ocorrencias/{id}", async (int id, AppDbContext db) =>
{
    var dto = await db.Ocorrencias
        .Where(o => o.Id == id)
        .Select(o => new OcorrenciaDto
        {
            Id = o.Id,
            Postagem_id = o.Postagem_id,
            Status = o.Status ?? "",
            Data_ocorrencia = o.Data_ocorrencia
        }).FirstOrDefaultAsync();

    return dto != null ? Results.Ok(dto) : Results.NotFound();
})
.WithTags("Ocorrências")
.WithMetadata(new
{
    Summary = "Retorna uma ocorrência específica pelo ID.",
    Description = "Consulta os detalhes de uma ocorrência vinculada a uma postagem."
});

app.MapPost("/ocorrencias", async (Ocorrencia ocorrencia, AppDbContext db) =>
{
    db.Ocorrencias.Add(ocorrencia);
    await db.SaveChangesAsync();

    var dto = new OcorrenciaDto
    {
        Id = ocorrencia.Id,
        Postagem_id = ocorrencia.Postagem_id,
        Status = ocorrencia.Status ?? "",
        Data_ocorrencia = ocorrencia.Data_ocorrencia
    };

    return Results.Created($"/ocorrencias/{ocorrencia.Id}", dto);
})
.WithTags("Ocorrências")
.WithMetadata(new
{
    Summary = "Cria uma nova ocorrência.",
    Description = "Registra uma nova ocorrência vinculada a uma postagem."
});

app.MapPut("/ocorrencias/{id}", async (int id, Ocorrencia input, AppDbContext db) =>
{
    var entity = await db.Ocorrencias.FindAsync(id);
    if (entity == null) return Results.NotFound();

    entity.Status = input.Status;
    entity.Data_ocorrencia = input.Data_ocorrencia;
    entity.Postagem_id = input.Postagem_id;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Ocorrências")
.WithMetadata(new
{
    Summary = "Atualiza uma ocorrência existente.",
    Description = "Modifica os dados de uma ocorrência pelo ID informado."
});

app.MapDelete("/ocorrencias/{id}", async (int id, AppDbContext db) =>
{
    var entity = await db.Ocorrencias.FindAsync(id);
    if (entity == null) return Results.NotFound();

    db.Ocorrencias.Remove(entity);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Ocorrências")
.WithMetadata(new
{
    Summary = "Remove uma ocorrência pelo ID.",
    Description = "Exclui a ocorrência cadastrada com o ID especificado."
});

#endregion

app.MapGet("/", () => "API SafeAlert está ativa!")
    .WithMetadata(new
    {
        Summary = "Endpoint raiz",
        Description = "Verifica se a API está ativa."
    });

//
await app.RunAsync();