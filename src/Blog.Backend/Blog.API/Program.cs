using Blog.API;
using Blog.Application;
using Blog.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Custom configurations
builder.Services.AddCustomConfigurations(builder.Configuration);
builder.AddConfiguredAzureKeyVault();
builder.Services.AddTextTranslator(builder.Configuration);

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

app.UseAuthentication();
app.UseAuthorization();

// Use exceptions handling
app.UseExceptionHandler();

app.MapControllers();

// Apply migrations
// var scope = app.Services.CreateScope();
// await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
// await dbContext.Database.MigrateAsync();
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

// Standard route for the home page
app.MapGet("/", () => $"Welcome to the Home Page Blog API!\nUTC Time: {DateTime.UtcNow}");

app.Run();