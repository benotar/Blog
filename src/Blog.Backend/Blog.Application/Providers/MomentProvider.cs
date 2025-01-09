using Blog.Application.Interfaces.Providers;

namespace Blog.Application.Providers;

public class MomentProvider : IMomentProvider
{
    public DateTimeOffset DateTimeOffsetUtcNow => DateTimeOffset.UtcNow;
    public DateTime DateTimeUtcNow => DateTime.UtcNow;
}