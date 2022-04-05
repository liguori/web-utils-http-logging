using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using WebUtils.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

static bool SourceContextEquals(LogEvent logEvent, string category)
           => logEvent.Properties.GetValueOrDefault("SourceContext") is ScalarValue sv &&
              sv.Value?.ToString() == category;
static bool sourceContextLogging(LogEvent le) => SourceContextEquals(le, "WebUtils.HttpLogging.HttpLoggingMiddleware");


Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Logger(lc =>
                lc.Filter.ByIncludingOnly(le => sourceContextLogging(le))
                .WriteTo.File($"httpLog.json", buffered: false)
            //.WriteTo.File(new CompactJsonFormatter(), $"httpLog.json", buffered: true)
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
        return "Hello World!";
    }
);

app.Run();
