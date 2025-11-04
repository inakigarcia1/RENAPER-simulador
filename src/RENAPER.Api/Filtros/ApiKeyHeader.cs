using System.Reflection;
using Microsoft.OpenApi.Models;
using RENAPER.Api.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RENAPER.Api.Filtros;

public class ApiKeyHeader : Attribute, IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.ApiDescription.TryGetMethodInfo(out var methodInfo)) return;
        if(methodInfo.GetCustomAttributes(typeof(RequiresApiKeyAttribute), true).Length == 0) return;
        
        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = "X-API-Key",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema() { Type = "string" }
        });
    }
}
