using Blog.Application.Models.Abstraction;
using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Models;

public class UserModel : BaseModel<UserModel, User>
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public override void AddCustomMappings()
    {
        SetCustomMappingsInverse()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Email, src => src.Email);
    }
    
}