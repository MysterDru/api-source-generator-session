using NorthwindTraders.WebApi.Features.Categories.Models;

namespace NorthwindTraders.WebApi.Features.Categories;

public interface ICategoriesService
{
    Task<CategorySummaryResponse> GetAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CategorySummaryResponse>> GetListAsync(CancellationToken cancellationToken);
}