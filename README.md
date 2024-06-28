# Generated API Controllers With Roslyn Generators

This repository contains the sample solution(s) to accompany my talk: **Stop Copy Pasting Code! Use Source Generators Instead".

The solutions contain 2 variants of a web api project built on top of the [Northwinds Sample Database](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/readme.md). If you want to run the Web Api you will need to setup and deploy the database. There is a schema and data seeder script in the [Database](./src/Database/) directory. If you want to run either API,  be sure to update the connection strings as appropriate in `appsettings.json`.


### Before

The before solution contains code without a source generator. Each controller and service are implemented manually.

### After

The after solution contains code with a source generator. The included source generator is designed to allow composing a data access service with interfaces that are directly associated to an API Controller structure. 

To use, decorate a service class with an attribute:

```csharp
[GenerateController(Name= "category", ServiceType = typeof(ICategoriesService))]
```

Define an interface, or class implementation that implements one to many of the available handler interfaces:

```csharp
public partial interface ICategoriesService : 
    IGetHandler<CategorySummaryResponse>,
    IGetListHandler<CategorySummaryResponse>
{
    // empty interface definition   
}
```

Then decorate your service class with the `GenerateController` attribute, and create implementations for the handler interfaces:

```csharp
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
```

This will auto-generate a simple API Controller for this service:

```csharp
[ApiController]
public class CustomersServiceController(NorthwindTraders.WebApi.Features.Customers.ICustomersService service) : ControllerBase
{

	[HttpGet("/api/customer.list")]
	public async Task<ActionResult<IReadOnlyList<NorthwindTraders.WebApi.Features.Customers.Models.CustomerSummaryResponse>>> GetList()
		=> Ok(await service.GetListAsync(default));

	[HttpPost("/api/customer.save")]
	public async Task<NoContentResult> Save(NorthwindTraders.WebApi.Features.Customers.Models.SaveCustomerRequest request)
	{
		await service.SaveAsync(request, default);
		return NoContent();
	}
}
```
