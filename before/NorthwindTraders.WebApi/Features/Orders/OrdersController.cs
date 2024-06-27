using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.WebApi.Features.Orders.Models;

namespace NorthwindTraders.WebApi.Features.Orders;

[ApiController]
public class OrdersController(IOrdersService service) : ControllerBase
{
    [HttpGet("/api/order.get")]
    public async Task<ActionResult<OrderResponse>> Get(int id)
    {
        return Ok(await service.GetAsync(id, default));
    }

    [HttpGet("/api/order.list")]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetList(GetOrderListRequest args)
    {
        return Ok(await service.GetListAsync(args, default));
    }
}