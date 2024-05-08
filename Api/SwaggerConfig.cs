using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api;

public static class SwaggerConfig
{
    public static SwaggerGenOptions AddJwtAuthorization(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "JWT" }
                },
                Array.Empty<string>()
            }
        });
        return options;
    }
}
