using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SafeAlertApi.Data;
using SafeAlertApi.Models;
using SafeAlertApi.DTOs;
using SafeAlertApi.Swagger;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Carrega variáveis de ambiente
Env.Load();

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

// Helper centralizado para tratamento de exceções
async Task<IResult> ExecuteAsync(Func<Task<IResult>> func)
{
    try
    {
        return await func();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}

// Mapear todos os endpoints
MapUsuariosEndpoints(app);
MapLocalidadesEndpoints(app);
MapEventosEndpoints(app);
MapPostagensEndpoints(app);
MapOcorrenciasEndpoints(app);

app.MapGet("/", () => "API SafeAlert está ativa!")
    .WithMetadata(new
    {
        Summary = "Endpoint raiz",
        Description = "Verifica se a API está ativa."
    });

await app.RunAsync();

#region Métodos para cada grupo de endpoints

void MapUsuariosEndpoints(WebApplication app)
{
    app.MapGet("/usuarios", (AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var list = await db.Usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Endereco = u.Endereco,
                Tipo_usuario = u.Tipo_usuario,
                Data_cadastro = u.Data_cadastro
            }).ToListAsync();
            return Results.Ok(list);
        }))
        .WithTags("Usuários");

    app.MapGet("/usuarios/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Usuários");

    app.MapPost("/usuarios", (Usuario usuario, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Usuários");

    app.MapPut("/usuarios/{id}", (int id, Usuario input, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Usuários");

    app.MapDelete("/usuarios/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var usuario = await db.Usuarios.FindAsync(id);
            if (usuario == null) return Results.NotFound();
            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Usuários");
}

void MapLocalidadesEndpoints(WebApplication app)
{
    app.MapGet("/localidades", (AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var list = await db.Localidades.Select(l => new LocalidadeDto
            {
                Id = l.Id,
                Bairro = l.Bairro ?? "",
                Zona = l.Zona ?? ""
            }).ToListAsync();
            return Results.Ok(list);
        }))
        .WithTags("Localidades");

    app.MapGet("/localidades/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Localidades");

    app.MapPost("/localidades", (Localidade input, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Localidades");

    app.MapPut("/localidades/{id}", (int id, Localidade input, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Localidades.FindAsync(id);
            if (entity == null) return Results.NotFound();
            entity.Bairro = input.Bairro;
            entity.Zona = input.Zona;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Localidades");

    app.MapDelete("/localidades/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Localidades.FindAsync(id);
            if (entity == null) return Results.NotFound();
            db.Localidades.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Localidades");
}

void MapEventosEndpoints(WebApplication app)
{
    app.MapGet("/eventos", (AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var list = await db.Eventos.Select(e => new EventoDto
            {
                Id = e.Id,
                Tipo = e.Tipo ?? "",
                Descricao = e.Descricao ?? ""
            }).ToListAsync();
            return Results.Ok(list);
        }))
        .WithTags("Eventos");

    app.MapGet("/eventos/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Eventos");

    app.MapPost("/eventos", (Evento evento, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Eventos");

    app.MapPut("/eventos/{id}", (int id, Evento input, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Eventos.FindAsync(id);
            if (entity == null) return Results.NotFound();
            entity.Tipo = input.Tipo;
            entity.Descricao = input.Descricao;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Eventos");

    app.MapDelete("/eventos/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Eventos.FindAsync(id);
            if (entity == null) return Results.NotFound();
            db.Eventos.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Eventos");
}

void MapPostagensEndpoints(WebApplication app)
{
    app.MapGet("/postagens", (AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var list = await db.Postagens.Select(p => new PostagemDto
            {
                Id = p.Id,
                Usuario_id = p.Usuario_id,
                Evento_id = p.Evento_id,
                Localidade_id = p.Localidade_id,
                Titulo = p.Titulo ?? "",
                Descricao = p.Descricao ?? "",
                Imagem_url = p.Imagem_url ?? "",
                Data_criacao = p.Data_criacao
            }).ToListAsync();
            return Results.Ok(list);
        }))
        .WithTags("Postagens");

    app.MapGet("/postagens/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Postagens");

    app.MapPost("/postagens", (Postagem postagem, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Postagens");

    app.MapPut("/postagens/{id}", (int id, Postagem input, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Postagens");

    app.MapDelete("/postagens/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Postagens.FindAsync(id);
            if (entity == null) return Results.NotFound();
            db.Postagens.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Postagens");
}

void MapOcorrenciasEndpoints(WebApplication app)
{
    app.MapGet("/ocorrencias", (AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var list = await db.Ocorrencias.Select(o => new OcorrenciaDto
            {
                Id = o.Id,
                Postagem_id = o.Postagem_id,
                Status = o.Status ?? "",
                Data_ocorrencia = o.Data_ocorrencia
            }).ToListAsync();
            return Results.Ok(list);
        }))
        .WithTags("Ocorrências");

    app.MapGet("/ocorrencias/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Ocorrências");

    app.MapPost("/ocorrencias", (Ocorrencia ocorrencia, AppDbContext db) =>
        ExecuteAsync(async () =>
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
        }))
        .WithTags("Ocorrências");

    app.MapPut("/ocorrencias/{id}", (int id, Ocorrencia input, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Ocorrencias.FindAsync(id);
            if (entity == null) return Results.NotFound();
            entity.Status = input.Status;
            entity.Data_ocorrencia = input.Data_ocorrencia;
            entity.Postagem_id = input.Postagem_id;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Ocorrências");

    app.MapDelete("/ocorrencias/{id}", (int id, AppDbContext db) =>
        ExecuteAsync(async () =>
        {
            var entity = await db.Ocorrencias.FindAsync(id);
            if (entity == null) return Results.NotFound();
            db.Ocorrencias.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }))
        .WithTags("Ocorrências");
}

#endregion

await app.RunAsync();