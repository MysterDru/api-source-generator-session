using NorthwindTraders.WebApi.Features.Orders.Models;

namespace NorthwindTraders.WebApi.Features.Orders;

public interface IOrdersService
{
    Task<OrderResponse> GetAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderResponse>> GetListAsync(GetOrderListRequest request, CancellationToken cancellationToken);
}