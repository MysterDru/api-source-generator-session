namespace NorthwindTraders.WebApi.Features.Products.Models;

public sealed record SaveProductRequest
{
    public required string ProductName { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    public bool Discontinued { get; set; }
}