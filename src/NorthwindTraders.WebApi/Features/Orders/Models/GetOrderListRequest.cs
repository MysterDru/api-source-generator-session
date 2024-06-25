namespace NorthwindTraders.WebApi.Features.Orders.Models;

public sealed record GetOrderListRequest
{
	public string? CustomerId { get; set; }
}
