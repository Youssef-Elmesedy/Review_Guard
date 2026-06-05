using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Review_Guard.API.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var hasAuthorize = metadata.OfType<AuthorizeAttribute>().Any();
        var hasAllowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();

        if (hasAllowAnonymous || !hasAuthorize)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}
public class CultureParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        // منع التكرار
        if (operation.Parameters.Any(p => p.Name == "culture"))
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "culture",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Select language (en / ar)",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("en"),
                Enum = new List<IOpenApiAny>
                {
                    new OpenApiString("en"),
                    new OpenApiString("ar")
                }
            }
        });
    }
}