namespace Blog.Application.Configurations;

public class DatabaseConfiguration
{
    public static readonly string ConfigurationKey = "Database";
    public string AzureSqlSectionName { get; init; }
}