namespace Blog.Domain.Entities;

public class User : DbEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }
    public string Role { get; set; }
}