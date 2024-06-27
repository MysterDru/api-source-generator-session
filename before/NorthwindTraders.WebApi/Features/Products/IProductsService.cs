using NorthwindTraders.WebApi.Features.Products.Models;

namespace NorthwindTraders.WebApi.Features.Products;

public partial interface IProductsService
{
    Task<ProductResponse> GetAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductResponse>> GetListAsync(CancellationToken cancellationToken);

    Task<int> CreateAsync(SaveProductRequest request, CancellationToken cancellationToken);

    Task UpdateAsync(int id, SaveProductRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}