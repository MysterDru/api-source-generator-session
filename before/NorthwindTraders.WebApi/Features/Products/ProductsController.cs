using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.WebApi.Features.Products.Models;

namespace NorthwindTraders.WebApi.Features.Products;

[ApiController]
public class ProductsServiceController(IProductsService service)
    : ControllerBase
{
    [HttpGet("/api/.list")]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>>
        GetList()
    {
        return Ok(await service.GetListAsync(default));
    }

    [HttpPost("/api/.create")]
    public async Task<ActionResult<ProductResponse>> Create(
        SaveProductRequest request)
    {
        var id = await service.CreateAsync(request, default);
        return Ok(await service.GetAsync(id, default));
    }

    [HttpGet("/api/.get")]
    public async Task<ActionResult<ProductResponse>> Get(int id)
    {
        return Ok(await service.GetAsync(id, default));
    }

    [HttpPost("/api/.delete")]
    public async Task<NoContentResult> Delete(int args)
    {
        await service.DeleteAsync(args, default);
        return NoContent();
    }
}