using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Models.Abstraction;

public abstract class BaseModel<TModel, TEntity> : IRegister
    where TModel : class, new()
    where TEntity : DbEntity, new()
{
    public TEntity ToEntity() => this.Adapt<TEntity>();

    public TEntity ToEntity(TEntity entity) => (this as TModel).Adapt(entity);

    public static TModel FromEntity(TEntity entity) => entity.Adapt<TModel>();
    private TypeAdapterConfig Config { get; set; }

    public virtual void AddCustomMappings()
    {
    }

    protected TypeAdapterSetter<TModel, TEntity> SetCustomMappings()
        => Config.ForType<TModel, TEntity>();

    protected TypeAdapterSetter<TEntity, TModel> SetCustomMappingsInverse()
        => Config.ForType<TEntity, TModel>();

    public void Register(TypeAdapterConfig config)
    {
        Config = config;
        AddCustomMappings();
    }
}