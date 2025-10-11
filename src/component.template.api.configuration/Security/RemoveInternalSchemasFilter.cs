using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace component.template.api.configuration.Security;

public class RemoveInternalSchemasFilter: IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemasToRemove = context.SchemaRepository.Schemas
            .Where(s => s.Key.Contains("Dto"))
            .ToList();

        foreach (var schema in schemasToRemove)
        {
            context.SchemaRepository.Schemas.Remove(schema.Key);
        }
    }
}