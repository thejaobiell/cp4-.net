using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SafeAlertApi.Swagger
{
    public class TagOrderDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var orderedTags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Usuários" },
                new OpenApiTag { Name = "Localidades" },
                new OpenApiTag { Name = "Eventos" },
                new OpenApiTag { Name = "Postagens" },
                new OpenApiTag { Name = "Ocorrências" }
            };
            swaggerDoc.Tags = orderedTags;
        }
    }
}
