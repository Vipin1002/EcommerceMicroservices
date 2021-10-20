using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Middleware
{
    public class AuthHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "X-User-Token",
                Description = "Access Token",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string", Default = new OpenApiString("Basic ") },
                Required = true,
            });
        }
    }
}
