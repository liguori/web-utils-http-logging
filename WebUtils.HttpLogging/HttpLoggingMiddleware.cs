using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtils.HttpLogging
{
    internal sealed class HttpLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public HttpLoggingMiddleware(RequestDelegate next,
                                         ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<HttpLoggingMiddleware>();


        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}
