using Microsoft.AspNetCore.Builder;

namespace WebUtils.HttpLogging
{
    public static class HttpLoggingBuilderExtensions
    {
        public static IApplicationBuilder UseWebUtilsHttpLoggingFull(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<HttpLoggingMiddleware>();
            return app;
        }
    }
}
