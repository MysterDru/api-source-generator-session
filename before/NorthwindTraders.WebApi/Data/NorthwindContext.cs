using Microsoft.EntityFrameworkCore;
using NorthwindTraders.WebApi.Data.Entities;

namespace NorthwindTraders.WebApi.Data;

/// <summary>
///     The Northwind context.
/// </summary>
internal class NorthwindContext : DbContext, INorthwindRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NorthwindContext" /> class.
    /// </summary>
    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Region> Region { get; set; }

    public DbSet<Shipper> Shippers { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Territory> Territories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NorthwindContext).Assembly);
    }
}