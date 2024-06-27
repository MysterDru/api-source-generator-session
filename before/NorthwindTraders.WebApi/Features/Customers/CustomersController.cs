using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.WebApi.Features.Customers.Models;

namespace NorthwindTraders.WebApi.Features.Customers;

[ApiController]
public class CustomersServiceController(ICustomersService service) : ControllerBase
{
    [HttpGet("/api/customer.list")]
    public async
        Task<ActionResult<IReadOnlyList<CustomerSummaryResponse>>>
        GetList()
    {
        return Ok(await service.GetListAsync(default));
    }

    [HttpGet("/api/customer.get")]
    public async Task<ActionResult<CustomerDetailResponse>>
        Get(string args)
    {
        return Ok(await service.GetAsync(args, default));
    }

    [HttpPost("/api/customer.save")]
    public async Task<NoContentResult> Save(
        SaveCustomerRequest request)
    {
        await service.SaveAsync(request, default);
        return NoContent();
    }

    [HttpPost("/api/customer.delete")]
    public async Task<NoContentResult> Delete(string args)
    {
        await service.DeleteAsync(args, default);
        return NoContent();
    }
}