namespace NorthwindTraders.WebApi.Features.Products.Models;

public sealed record ProductResponse
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? SupplierId { get; set; }

    public string? SupplierCompanyName { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public bool Discontinued { get; set; }
}