using NorthwindTraders.WebApi.Features.Customers.Models;

namespace NorthwindTraders.WebApi.Features.Customers;

public partial interface ICustomersService
{
    Task<CustomerDetailResponse> GetAsync(string id, CancellationToken cancellationToken);


    Task<IReadOnlyList<CustomerSummaryResponse>> GetListAsync(CancellationToken cancellationToken);


    Task SaveAsync(SaveCustomerRequest request, CancellationToken cancellationToken);


    Task DeleteAsync(string id, CancellationToken cancellationToken);
}