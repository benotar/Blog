using Blog.Application.Common;

namespace Blog.Application.Interfaces.Providers;

public interface IEncryptionProvider
{
    SaltAndHash HashPassword(string password);
    bool VerifyPasswordHash(string password, SaltAndHash saltAndHash);
}