namespace Blog.Application.Common;

public record struct SaltAndHash(
    byte[] Salt,
    byte[] Hash
);