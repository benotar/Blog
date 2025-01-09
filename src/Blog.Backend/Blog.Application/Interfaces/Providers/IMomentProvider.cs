namespace Blog.Application.Interfaces.Providers;

public interface IMomentProvider
{
    DateTimeOffset DateTimeOffsetUtcNow { get; }
    DateTime DateTimeUtcNow { get; }

}