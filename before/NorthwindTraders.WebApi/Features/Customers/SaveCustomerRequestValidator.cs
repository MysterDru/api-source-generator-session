using FluentValidation;
using NorthwindTraders.WebApi.Features.Customers.Models;

namespace NorthwindTraders.WebApi.Features.Customers;

public sealed class SaveCustomerRequestValidator : AbstractValidator<SaveCustomerRequest>
{
    public SaveCustomerRequestValidator()
    {
        RuleFor(x => x.Id).Length(5).NotEmpty();
        RuleFor(x => x.Address).MaximumLength(60);
        RuleFor(x => x.City).MaximumLength(15);
        RuleFor(x => x.CompanyName).MaximumLength(40).NotEmpty();
        RuleFor(x => x.ContactName).MaximumLength(30);
        RuleFor(x => x.ContactTitle).MaximumLength(30);
        RuleFor(x => x.Country).MaximumLength(15);
        RuleFor(x => x.Fax).MaximumLength(24);
        RuleFor(x => x.Phone).MaximumLength(24);
        RuleFor(x => x.PostalCode).MaximumLength(10).NotEmpty();
        RuleFor(x => x.Region).MaximumLength(15);
    }
}