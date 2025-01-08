using Blog.Application.Interfaces.Providers;

namespace Blog.Application.Providers;

public class MomentProvider : IMomentProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}