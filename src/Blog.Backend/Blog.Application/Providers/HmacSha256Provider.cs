using System.Security.Cryptography;
using System.Text;
using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;

namespace Blog.Application.Providers;

public class HmacSha256Provider : IEncryptionProvider
{
    public SaltAndHash HashPassword(string password)
    {
        using var hmac = new HMACSHA256();

        var salt = hmac.Key;

        var bytes = Encoding.UTF8.GetBytes(password);

        var hash = hmac.ComputeHash(bytes);

        return new SaltAndHash(salt, hash);
    }
}