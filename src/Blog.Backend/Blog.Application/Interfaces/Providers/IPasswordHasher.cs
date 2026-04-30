namespace Blog.Application.Interfaces.Providers;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPasswordHash(string password, string hash);
}
