using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CMS.Api.Configuration
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFile[]))
                .ToArray();

            if (fileParameters.Length == 0)
                return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>()
                        }
                    }
                }
            };

            var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

            foreach (var parameter in context.MethodInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(IFormFile))
                {
                    schema.Properties[parameter.Name!] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    };
                }
                else if (parameter.ParameterType == typeof(IFormFile[]))
                {
                    schema.Properties[parameter.Name!] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    };
                }
                else if (parameter.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromFormAttribute), false).Any())
                {
                    schema.Properties[parameter.Name!] = new OpenApiSchema
                    {
                        Type = GetSchemaType(parameter.ParameterType)
                    };
                }
            }

            // Remove parameters that are handled by the request body
            operation.Parameters = operation.Parameters?.Where(p => 
                !fileParameters.Any(fp => fp.Name == p.Name)).ToList();
        }

        private string GetSchemaType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(int?)) return "integer";
            if (type == typeof(bool) || type == typeof(bool?)) return "boolean";
            if (type.IsEnum) return "string";
            return "string";
        }
    }
}