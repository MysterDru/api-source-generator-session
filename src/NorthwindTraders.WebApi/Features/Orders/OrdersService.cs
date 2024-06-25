using System.Linq.Expressions;
using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.WebApi.Data;
using NorthwindTraders.WebApi.Data.Entities;
using NorthwindTraders.WebApi.Features.Categories;
using NorthwindTraders.WebApi.Features.Orders.Models;
using NorthwindTraders.WebApi.Infrastructure.Attributes;
using NorthwindTraders.WebApi.Infrastructure.Exceptions;

namespace NorthwindTraders.WebApi.Features.Orders;

public partial interface IOrdersService;

partial interface IOrdersService : IGetHandler<OrderResponse>,
	IGetListHandler<GetOrderListRequest, OrderResponse>; 

[RegisterTransient<IOrdersService>]
[GenerateController(Name = "order", ServiceType = typeof(IOrdersService))]
public sealed class OrdersService(ILogger<CategoriesService> logger, INorthwindRepository repository)
	: IOrdersService
{
	private static Expression<Func<Order, OrderResponse>> OrderProjection
		=> p => new OrderResponse()
		{
			OrderId = p.OrderId,
			CustomerId = p.CustomerId,
			EmployeeId = p.EmployeeId,
			OrderDate = p.OrderDate,
			RequiredDate = p.RequiredDate,
			ShippedDate = p.ShippedDate,
			ShipVia = p.ShipVia,
			Freight = p.Freight,
			ShipName = p.ShipName,
			ShipAddress = p.ShipAddress,
			ShipCity = p.ShipCity,
			ShipRegion = p.ShipRegion,
			ShipPostalCode = p.ShipPostalCode,
			ShipCountry = p.ShipCountry,

			Details = p.OrderDetails.Select(d => new OrderDetailResponse()
			{
				ProductId = d.ProductId,
				ProductName = d.Product.ProductName,
				UnitPrice = d.UnitPrice,
				Quantity = d.Quantity,
				Discount = d.Discount
			})
		};

	public async Task<OrderResponse> GetAsync(int id, CancellationToken cancellationToken)
	{
		var result = await repository.Set<Order>()
			.Where(x => x.OrderId == id)
			.Select(OrderProjection)
			.FirstOrDefaultAsync(cancellationToken);

		if (result == null)
		{
			throw new NotFoundException(nameof(Category), id);
		}

		return result;
	}

	public async Task<IReadOnlyList<OrderResponse>> GetListAsync(GetOrderListRequest request,
		CancellationToken cancellationToken)
	{
		return await repository.Set<Order>()
			.Where(x => request.CustomerId == null || x.CustomerId == request.CustomerId)
			.Select(OrderProjection)
			.ToListAsync(cancellationToken);
	}
}
