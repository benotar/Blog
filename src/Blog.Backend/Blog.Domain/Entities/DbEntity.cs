using System.ComponentModel.DataAnnotations;

namespace Blog.Domain.Entities;

public class DbEntity
{
    [Key] public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}