using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.FactoryMethod;

public interface IPaginationFactory<TEntity>
{
    public IPagination<TEntity> CreatePaginationFactory(PaginationType paginationType);
}