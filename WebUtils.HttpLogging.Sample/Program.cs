using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using WebUtils.HttpLogging;

static bool SourceContextEquals(LogEvent logEvent, string category)
           => logEvent.Properties.GetValueOrDefault("SourceContext") is ScalarValue sv &&
              sv.Value?.ToString() == category;
static bool sourceContextLogging(LogEvent le) => SourceContextEquals(le, "WebUtils.HttpLogging.HttpLoggingMiddleware");


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Logger(lc =>
                lc.Filter.ByIncludingOnly(le => sourceContextLogging(le))
                   //.WriteTo.File($"httpLog.json", buffered: false,)
                   .WriteTo.File($"httpLog.json", buffered: false, outputTemplate: "{Message:lj}{NewLine}")
           // .WriteTo.File(new CompactJsonFormatter(), $"httpLog.json", buffered: false)
            )
           .WriteTo.Logger(lc =>
               lc.Filter.ByIncludingOnly(le => !sourceContextLogging(le))
               .WriteTo.Console()
            )
           .CreateLogger();

builder.Host.UseSerilog();



var app = builder.Build();

app.UseWebUtilsHttpLoggingFull();

app.Map("/{**path}",
    (HttpRequest request, HttpResponse response) =>
    {
        response.Headers.Add("Pippo", "Pluto");
        return "Hello World!";
    }
);

app.Run();
