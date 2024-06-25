using NorthwindTraders.WebApi.SourceGen;

namespace NorthwindTraders.WebApi.Features.Products;

public interface IProductService : IGetHandler<ProductResponse>, IGetListHandler<ProductResponse>,
    ISaveHandler<SaveProductRequest>, IUpdateHandler<SaveProductRequest>,
    ICreateHandler<SaveProductRequest, ProductResponse>;

[GenerateController(Name = "product", ServiceType = typeof(IProductService))]
public class ProductService : IProductService
{
    public async Task<ProductResponse> GetAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<ProductResponse>> GetListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SaveAsync(SaveProductRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, SaveProductRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateAsync(SaveProductRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class ProductResponse
{
}

public class SaveProductRequest
{
}