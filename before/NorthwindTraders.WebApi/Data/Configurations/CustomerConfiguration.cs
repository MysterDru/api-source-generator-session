using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NorthwindTraders.WebApi.Data.Entities;

namespace NorthwindTraders.WebApi.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(e => e.CustomerId)
            .IsRequired()
            .HasColumnName("CustomerID")
            .HasMaxLength(5)
            .ValueGeneratedNever();

        builder.Property(e => e.Address).HasMaxLength(60);

        builder.Property(e => e.City).HasMaxLength(15);

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.ContactName).HasMaxLength(30);

        builder.Property(e => e.ContactTitle).HasMaxLength(30);

        builder.Property(e => e.Country).HasMaxLength(15);

        builder.Property(e => e.Fax).HasMaxLength(24);

        builder.Property(e => e.Phone).HasMaxLength(24);

        builder.Property(e => e.PostalCode).HasMaxLength(10);

        builder.Property(e => e.Region).HasMaxLength(15);
    }
}