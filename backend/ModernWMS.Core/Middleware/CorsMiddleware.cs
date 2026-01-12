using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ModernWMS.Core.Middleware
{
    /// <summary>
    /// Cross domain middleware
    /// </summary>
    public class CorsMiddleware
    {
        #region parameter
        /// <summary>
        /// agent
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// Allowed origins for CORS
        /// </summary>
        private readonly string[] _allowedOrigins;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Delegate in next step</param>
        /// <param name="configuration">Configuration for reading allowed origins</param>
        public CorsMiddleware(RequestDelegate next, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _next = next;
            _allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:5173", "http://localhost:5174" };
        }
        #endregion

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="httpContext">httpContext</param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            var origin = httpContext.Request.Headers["Origin"].ToString();

            // Validate origin against whitelist
            if (!string.IsNullOrEmpty(origin) && !_allowedOrigins.Contains(origin))
            {
                // Reject unauthorized origins - don't add CORS headers
                return _next.Invoke(httpContext);
            }

            if (httpContext.Request.Method == "OPTIONS")
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", httpContext.Request.Headers["Access-Control-Request-Headers"]);
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS,HEAD,PATCH");
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            }

            if (!string.IsNullOrEmpty(origin))
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            }

            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", httpContext.Request.Headers["Access-Control-Request-Headers"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS,HEAD,PATCH");
            httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");
            return _next.Invoke(httpContext);
        }

    }
}
