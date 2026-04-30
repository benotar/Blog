using Blog.API;
using Blog.API.Infrastructure;
using Blog.Application;
using Blog.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Scalar
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<AuthorizeOperationTransformer>();
});

// Custom configurations
builder.Services.AddCustomConfigurations(builder.Configuration);

builder.Services.AddAuth(builder.Configuration);

// Application layers
builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration);

// Configured controllers
builder.Services.AddControllersWithConfiguredApiBehavior(builder.Configuration);

// Exceptions handling
builder.Services.AddExceptionHandlerWithProblemDetails();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference("/");

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Use exceptions handling
app.UseExceptionHandler();

app.MapControllers();

// Apply migrations
await app.ApplyMigrationsAsync();

app.Run();
