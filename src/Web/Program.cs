using Application;
using Application.Interfaces.Services.Equipe;
using Application.Interfaces.Services.Module;
using Application.Services.Equipe;
using Application.Services.Module;
using Domain.Common;
using Domain.Extensions;
using Domain.Repositories;
using FastEndpoints;
using FastEndpoints.Swagger;
using Infrastructure;
using Infrastructure.Repositories.Conversations;
using Infrastructure.Repositories.Module;
using Microsoft.AspNetCore.Diagnostics;
using Persistence;
using Serilog;
using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddSignalR();
builder.Configuration.AddJsonFile("appsettings.local.json", true);
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(x =>
    {
        x.ExcludeNonFastEndpoints = true;
        x.ShortSchemaNames = true;
    });

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration)
    .Filter.ByExcluding(x =>
    {
        if (x.Exception?.GetType().Name == null)
            return false;
        var handledErrors = builder.Configuration.GetSection("HandledErrors").Get<List<string>>();
        return handledErrors!.Contains(x.Exception.GetType().Name);
    })
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IEquipeRepository, EquipeRepository>();
builder.Services.AddScoped<IEquipeService, EquipeService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IQuizRepository, Infrastructure.Repositories.Quiz.QuizRepository>();
builder.Services.AddScoped<IQuizAssignmentRepository, Infrastructure.Repositories.Quiz.QuizAssignmentRepository>();
builder.Services.AddScoped<IUserQuizResponseRepository, Infrastructure.Repositories.Quiz.UserQuizResponseRepository>();

/*builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "corsDomains",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("CorsDomains")
                    .GetChildren()
                    .Select(c => c.Value)
                    .ToArray()!)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});*/

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsDomains", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7101",
                "http://localhost:8080",
                "https://localhost:8080"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();
await app.Services.InitializeAndSeedDatabase();

var supportedCultures = new[] { "en-CA", "fr-CA" };
app.UseRequestLocalization(options =>
{
    // the order of QueryStringRequestCultureProvider and CookieRequestCultureProvider is switched,
    // so the RequestLocalizationMiddleware looks for the cultures from the cookies first, then query string.
    var questStringCultureProvider = options.RequestCultureProviders[0];
    options.RequestCultureProviders.RemoveAt(0);
    options.RequestCultureProviders.Insert(1, questStringCultureProvider);
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

app.UseExceptionHandler(c => c.Run(async context =>
{
    var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
    if (exceptionHandler?.Error == null)
        return;

    var responseBody = new SucceededOrNotResponse(false, exceptionHandler.Error.ErrorObject());
    switch (exceptionHandler.Error)
    {
        case ValidationFailureException exception:
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            responseBody = new SucceededOrNotResponse(false, exception.ErrorObjects());
            break;
    }
    await context.Response.WriteAsJsonAsync(responseBody);
}));

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (context.File.Name.EndsWith(".js") || context.File.Name.EndsWith(".css"))
        {
            context.Context.Response.Headers.CacheControl = "public, max-age=3600";
        }
    }
});
app.UseRouting();
app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .WithOrigins("http://localhost:8080", "https://localhost:8080", "https://localhost:7101")
    .WithOrigins("http://localhost:8080", "https://localhost:8080", "https://localhost:7101")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
app.UseCors("corsDomains");
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(config => { config.Endpoints.RoutePrefix = "api"; });
app.UseSwaggerGen();

app.MapHub<Web.Hubs.ChatHub>("/api/chat-hub");

// SPA fallback - serve Vue app for any non-API route
app.MapFallbackToFile("vue/index.html");

app.Run();
