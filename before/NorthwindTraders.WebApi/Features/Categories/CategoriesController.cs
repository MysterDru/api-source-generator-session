using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.WebApi.Features.Categories.Models;

namespace NorthwindTraders.WebApi.Features.Categories;

[ApiController]
public class CategoriesController(ICategoriesService service) : ControllerBase
{
    [HttpGet("/api/category.get")]
    public async Task<ActionResult<CategorySummaryResponse>> Get(int id)
    {
        return Ok(await service.GetAsync(id, default));
    }

    [HttpGet("/api/category.list")]
    public async Task<ActionResult<IReadOnlyList<CategorySummaryResponse>>> GetList()
    {
        return Ok(await service.GetListAsync(default));
    }
}