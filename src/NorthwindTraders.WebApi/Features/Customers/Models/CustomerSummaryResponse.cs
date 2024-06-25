namespace NorthwindTraders.WebApi.Features.Customers.Models;

public record CustomerSummaryResponse
{
	public required string Id { get; set; }

	public string? Name { get; set; }
}
