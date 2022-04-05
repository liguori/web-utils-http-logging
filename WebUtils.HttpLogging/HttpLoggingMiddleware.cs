using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Text.Json;
using WebUtils.HttpLogging.Models;

namespace WebUtils.HttpLogging
{
    internal sealed class HttpLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await LogRequest(context);
            }
            finally
            {

                var originalBodyStream = context.Response.Body;
                await using var responseBody = _recyclableMemoryStreamManager.GetStream();
                context.Response.Body = responseBody;

                // Call the next delegate/middleware in the pipeline.
                await _next(context);

                await LogResponse(context);

                await responseBody.CopyToAsync(originalBodyStream);
            }

        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            var body = ReadStreamInChunks(requestStream);

            var httpRequestLog = new HttpRequestLog();
            httpRequestLog.Protocol = context.Request.Protocol;
            httpRequestLog.Method = context.Request.Method;
            httpRequestLog.Scheme = context.Request.Scheme;
            httpRequestLog.Host = context.Request.Host.ToString();
            httpRequestLog.Path = context.Request.Path;
            httpRequestLog.PathBase = context.Request.PathBase;
            httpRequestLog.QueryString = context.Request.QueryString.ToString();
            httpRequestLog.Headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            httpRequestLog.Body = body;

            _logger.Log(LogLevel.Information, new EventId(11, "HttpRequestLog"), httpRequestLog, null, (HttpRequestLog, ex) => JsonSerializer.Serialize(HttpRequestLog));

            context.Request.Body.Position = 0;
        }
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }


        private async Task LogResponse(HttpContext context)
        {

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var httpResponseLog = new HttpResponseLog();
            httpResponseLog.StatusCode = context.Response.StatusCode;
            httpResponseLog.Headers = context.Response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            httpResponseLog.Body = text;

            _logger.Log(LogLevel.Information, new EventId(22, "HttpResponseLog"), httpResponseLog, null, (HttpRequestLog, ex) => JsonSerializer.Serialize(httpResponseLog));

        }
    }
}
