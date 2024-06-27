namespace NorthwindTraders.WebApi.Features.Orders.Models;

public sealed class OrderDetailResponse
{
	public int ProductId { get; set; }

	public string? ProductName { get; set; }

	public decimal UnitPrice { get; set; }

	public short Quantity { get; set; }

	public float Discount { get; set; }
}
