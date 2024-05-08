using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Api;

public static class SwaggerConfig
{
    private static string ToCamelCase(this string name)
        => char.ToLowerInvariant(name[0]) + name[1..];

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

    public static SwaggerGenOptions AddDefaultValues(this SwaggerGenOptions options)
    {
        options.SchemaFilter<DefaultValueFilter>();
        return options;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DefaultApiValueAttribute : Attribute
    {
        public IOpenApiPrimitive Value { get; set; }

        public DefaultApiValueAttribute(string value)
        {
            Value = new OpenApiString(value);
        }
    }

    public class DefaultValueFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
                return;

            foreach (ConstructorInfo cInfo in context.Type.GetConstructors())
            {
                foreach (ParameterInfo pInfo in cInfo.GetParameters())
                {
                    DefaultApiValueAttribute? defaultAttribute = pInfo.GetCustomAttribute<DefaultApiValueAttribute>();
                    var value = defaultAttribute?.Value;
                    if (value != null)
                        schema.Properties[ToCamelCase(pInfo.Name!)].Example = value;
                }
            }
            foreach (PropertyInfo pInfo in context.Type.GetProperties())
            {
                DefaultApiValueAttribute? defaultAttribute = pInfo.GetCustomAttribute<DefaultApiValueAttribute>();
                var value = defaultAttribute?.Value;
                if (value != null)
                    schema.Properties[ToCamelCase(pInfo.Name!)].Example = value;
            }
        }
    }
}
