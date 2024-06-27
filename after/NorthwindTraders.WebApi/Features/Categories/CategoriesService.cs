using System.Linq.Expressions;
using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.WebApi.Data;
using NorthwindTraders.WebApi.Data.Entities;
using NorthwindTraders.WebApi.Features.Categories.Models;
using NorthwindTraders.WebApi.Infrastructure.Exceptions;

namespace NorthwindTraders.WebApi.Features.Categories;

public interface ICategoriesService : IGetHandler<CategorySummaryResponse>,
	IGetListHandler<CategorySummaryResponse>; 

[RegisterTransient<ICategoriesService>]
[GenerateController(Name= "category", ServiceType = typeof(ICategoriesService))]
public sealed class CategoriesService(ILogger<CategoriesService> logger, INorthwindRepository repository)
	: ICategoriesService
{
	private static Expression<Func<Category, CategorySummaryResponse>> CategoryProjection => c => new CategorySummaryResponse
	{
		CategoryId = c.CategoryId,
		CategoryName = c.CategoryName,
		Description = c.Description,
		Products = c.Products.AsQueryable()
			.Select(ProductProjection)
			.Take(5)
			.OrderBy(p => p.ProductName)
			.ToList()
	};

	private static Expression<Func<Product, ProductSummaryResponse>> ProductProjection => p => new ProductSummaryResponse
	{
		ProductId = p.ProductId,
		ProductName = p.ProductName,
		UnitPrice = p.UnitPrice
	};

	public async Task<CategorySummaryResponse> GetAsync(int id, CancellationToken cancellationToken)
	{
		var result = await repository.Set<Category>()
			.Where(x => x.CategoryId == id)
			.Select(CategoryProjection)
			.FirstOrDefaultAsync(cancellationToken);

		if (result == null)
		{
			throw new NotFoundException(nameof(Category), id);
		}

		return result;
	}

	public async Task<IReadOnlyList<CategorySummaryResponse>> GetListAsync(CancellationToken cancellationToken)
	{
		// load data into memory and project in-memory to avoid N + 1 query.
		var data = await repository.Set<Category>()
			.Select(CategoryProjection)
			.ToListAsync(cancellationToken);

		return data;
	}
}
