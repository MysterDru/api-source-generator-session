namespace NorthwindTraders.WebApi.Features.Categories.Models;

public sealed record CategorySummaryResponse
{
	public int CategoryId { get; set; }

	public string? CategoryName { get; set; }

	public string? Description { get; set; }

	public ICollection<ProductSummaryResponse> Products { get; set; } = new List<ProductSummaryResponse>();
}
