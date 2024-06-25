using NorthwindTraders.WebApi.Data.Configurations;

namespace NorthwindTraders.WebApi.Features.Orders.Models;

public sealed record OrderResponse
{

	public int OrderId { get; set; }


	public string? CustomerId { get; set; }


	public int? EmployeeId { get; set; }


	public DateTime? OrderDate { get; set; }


	public DateTime? RequiredDate { get; set; }


	public DateTime? ShippedDate { get; set; }


	public int? ShipVia { get; set; }


	public decimal? Freight { get; set; }


	public string? ShipName { get; set; }


	public string? ShipAddress { get; set; }


	public string? ShipCity { get; set; }


	public string? ShipRegion { get; set; }


	public string? ShipPostalCode { get; set; }


	public string? ShipCountry { get; set; }

	public IEnumerable<OrderDetailResponse> Details { get; set; }
}
