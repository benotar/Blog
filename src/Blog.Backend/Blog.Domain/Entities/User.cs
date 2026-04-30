namespace Blog.Domain.Entities;

public class User : DbEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
}
