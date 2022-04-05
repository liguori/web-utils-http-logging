using WebUtils.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebUtilsHttpLoggingFull();

app.Map("/{**path}",
    (HttpRequest request, HttpResponse response) =>
    {
        return "Hello World!";
    }
);

app.Run();
