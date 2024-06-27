namespace NorthwindTraders.WebApi.Features.Categories.Models;

public sealed record ProductSummaryResponse
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public decimal? UnitPrice { get; set; }
}