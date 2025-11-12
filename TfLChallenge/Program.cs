using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;
using TfLChallenge;
using TfLChallenge.Abstractions;
using TfLChallenge.Formatters;
using TfLChallenge.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.SetBasePath(AppContext.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var baseUrl = builder.Configuration["TflApi:BaseUrl"];
var apiId = builder.Configuration["TflApi:Auth:Id"];
var apiKey = builder.Configuration["TflApi:Auth:Key"];

builder.Services.AddRefitClient<ITflApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(baseUrl);
        c.DefaultRequestHeaders.Add("app_id", apiId);
        c.DefaultRequestHeaders.Add("app_key", apiKey);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.BackoffType = DelayBackoffType.Exponential;
    });

builder.Services.AddTransient<IRoadStatusService, RoadStatusService>();
builder.Services.AddTransient<IRoadStatusFormatter, PlainTextRoadStatusFormatter>();
builder.Services.AddTransient<App>();

builder.Logging.SetMinimumLevel(LogLevel.Warning); 

using var host = builder.Build();

await host.Services.GetRequiredService<App>().Run();