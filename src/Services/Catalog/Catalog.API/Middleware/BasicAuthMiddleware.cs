using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.API.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string realm;
        private readonly ILogger<BasicAuthMiddleware> logger;

        public BasicAuthMiddleware(RequestDelegate next, ILogger<BasicAuthMiddleware> logger)
        {
            this.next = next ;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            LogData(context);

            using(logger.BeginScope<BasicAuthMiddleware>(this))
            {
                if(context.Request.Path.ToString().ToLower().Contains("swagger") ||
                    context.Request.Path.ToString().ToLower().Contains("favicon"))
                {
                    await next.Invoke(context);
                    return;
                }
                else
                {
                    string authHeader = context.Request.Headers["X-User-Token"];
                    if (authHeader != null && authHeader.StartsWith("Basic "))
                    {
                        string encodedAuthenticationToken = authHeader.Substring("Basic ".Length).Trim();
                        var decodedAuthenticationToken = Base64Decode(encodedAuthenticationToken);
                        var tokenArray = decodedAuthenticationToken.Split(':');

                        var userName = tokenArray[0];
                        var password = tokenArray[1];

                        //check if login is correct
                        if(IsAuthorized(userName, password))
                        {
                            await next.Invoke(context);
                            return;
                        }
                    }
                }

                //Return authentication type
                context.Response.Headers["WWW-Authenticate"] = "Basic";

                //return unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            }
        }

        public bool IsAuthorized(string userName, string password)
        {
            //check username and password are correct
            return userName.Equals("admin", StringComparison.InvariantCulture)
                && password.Equals("Admin@123$");
        }

        private void LogData(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
