namespace Blog.Application.Interfaces.Providers;

public interface IMomentProvider
{
    DateTimeOffset Now { get; }
}