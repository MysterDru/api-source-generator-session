using System.Linq.Expressions;
using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.WebApi.Data;
using NorthwindTraders.WebApi.Data.Entities;
using NorthwindTraders.WebApi.Features.Categories;
using NorthwindTraders.WebApi.Features.Products.Models;
using NorthwindTraders.WebApi.Infrastructure.Exceptions;

namespace NorthwindTraders.WebApi.Features.Products;

public partial interface IProductsService : IGetListHandler<ProductResponse>,
    IGetHandler<int, ProductResponse>,
    ICreateHandler<SaveProductRequest, ProductResponse>,
    IUpdateHandler<int, SaveProductRequest>,
    IDeleteHandler<int>;

[RegisterTransient<IProductsService>]
[GenerateController(ServiceType = typeof(IProductsService))]
public sealed class ProductsService(ILogger<CategoriesService> logger, INorthwindRepository repository)
 : IProductsService
{
    private static Expression<Func<Product, ProductResponse>> ProductProjection
        => p => new ProductResponse()
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            CategoryId = p.CategoryId,
            SupplierId = p.SupplierId,
            UnitPrice = p.UnitPrice,
            SupplierCompanyName = p.Supplier.CompanyName,
            CategoryName = p.Category.CategoryName,
            Discontinued = p.Discontinued
        };

    public async Task<ProductResponse> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await repository.Set<Product>()
            .Where(x => x.ProductId == id)
            .Select(ProductProjection)
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        return result;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetListAsync(CancellationToken cancellationToken)
    {
        return await repository.Set<Product>()
            .OrderBy(x => x.ProductName)
            .Select(ProductProjection)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CreateAsync(SaveProductRequest request, CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            ProductName = request.ProductName,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            UnitPrice = request.UnitPrice,
            Discontinued = request.Discontinued
        };

        await repository.Set<Product>()
            .AddAsync(entity, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return entity.ProductId;
    }

    public async Task UpdateAsync(int id, SaveProductRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.Set<Product>().FindAsync(id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Product), id);
        }

        entity.ProductId = id;
        entity.ProductName = request.ProductName;
        entity.CategoryId = request.CategoryId;
        entity.SupplierId = request.SupplierId;
        entity.UnitPrice = request.UnitPrice;
        entity.Discontinued = request.Discontinued;

        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var products = repository.Set<Product>();
        var entity = await products.FindAsync(id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Product), id);
        }

        var hasOrders = repository.Set<OrderDetail>().Any(od => od.ProductId == entity.ProductId);
        if (hasOrders)
        {
            throw new DeleteFailureException(nameof(Product), id,
                "There are existing orders associated with this product.");
        }

        products.Remove(entity);

        await repository.SaveChangesAsync(cancellationToken);
    }
}