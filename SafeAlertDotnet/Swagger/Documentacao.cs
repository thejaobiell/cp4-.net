using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SafeAlertApi.Swagger
{
    public class Documentacao : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Usuários", Description = "Gerencia os dados dos usuários" },
                new OpenApiTag { Name = "Localidades", Description = "Define as regiões onde ocorrem os eventos" },
				new OpenApiTag { Name = "Eventos", Description = "Lista e organiza tipos de eventos" },
				new OpenApiTag { Name = "Postagens", Description = "Gerencia as postagens sobre eventos" },
                new OpenApiTag { Name = "Ocorrências", Description = "Rastreamento das ocorrências geradas" }
            };
        }
    }
}