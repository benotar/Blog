using Blog.Application.Interfaces.FactoryMethod;
using Blog.Domain.Enums;

namespace Blog.Application.FactoryMethod;

public class PaginationFactory<TEntity> : IPaginationFactory<TEntity>
{
    public IPagination<TEntity> CreatePaginationFactory(PaginationType paginationType)
    {
        return paginationType switch
        {
            PaginationType.Offset => new OffsetPagination<TEntity>(),
            PaginationType.Paged => new PagedPagination<TEntity>(),
            _ => throw new ArgumentException("Invalid pagination type.", nameof(paginationType))
        };
    }
}