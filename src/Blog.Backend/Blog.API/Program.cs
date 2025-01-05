using Blog.API;
using Blog.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomConfigurations(builder.Configuration);

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

// Standard route for the home page
app.MapGet("/", () => $"Welcome to the Home Page Blog API!\nUTC Time: {DateTime.UtcNow}");

app.Run();