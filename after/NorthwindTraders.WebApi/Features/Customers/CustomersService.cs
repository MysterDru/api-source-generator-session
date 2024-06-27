using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.WebApi.Data;
using NorthwindTraders.WebApi.Data.Entities;
using NorthwindTraders.WebApi.Features.Customers.Models;
using NorthwindTraders.WebApi.Infrastructure.Exceptions;

namespace NorthwindTraders.WebApi.Features.Customers;

public partial interface ICustomersService : IGetListHandler<CustomerSummaryResponse>,
	IGetHandler<string, CustomerDetailResponse>,
	ISaveHandler<SaveCustomerRequest>,
	IDeleteHandler<string>;

[RegisterTransient<ICustomersService>]
[GenerateController(Name= "customer", ServiceType = typeof(ICustomersService))]
public sealed class CustomersService(INorthwindRepository repository) :
	ICustomersService
{
	public async Task<CustomerDetailResponse> GetAsync(string id, CancellationToken cancellationToken)
	{
		var result = await repository.Set<Customer>()
			.Where(c => c.CustomerId == id)
			.Select(customer => new CustomerDetailResponse()
			{
				Id = customer.CustomerId,
				Address = customer.Address,
				City = customer.City,
				CompanyName = customer.CompanyName,
				ContactName = customer.ContactName,
				ContactTitle = customer.ContactTitle,
				Country = customer.Country,
				Fax = customer.Fax,
				Phone = customer.Phone,
				PostalCode = customer.PostalCode,
				Region = customer.Region
			})
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);

		if (result == null)
		{
			throw new NotFoundException("Customer", id);
		}

		return result;
	}

	public async Task<IReadOnlyList<CustomerSummaryResponse>> GetListAsync(CancellationToken cancellationToken)
	{
		return await repository.Set<Customer>()
			.Select(c => new CustomerSummaryResponse()
			{
				Id = c.CustomerId,
				Name = c.CompanyName
			})
			.ToListAsync(cancellationToken);
	}

	public async Task SaveAsync(SaveCustomerRequest request, CancellationToken cancellationToken)
	{
		var customers = repository.Set<Customer>();
		var entity = await customers.SingleOrDefaultAsync(x => x.CustomerId == request.Id, cancellationToken);

		bool isEdit = true;
		if (entity == null)
		{
			isEdit = false;
			entity = new Customer()
			{
				CustomerId = request.Id
			};
			customers.Add(entity);
		}

		entity.Address = request.Address;
		entity.City = request.City;
		entity.CompanyName = request.CompanyName;
		entity.ContactName = request.ContactName;
		entity.ContactTitle = request.ContactTitle;
		entity.Country = request.Country;
		entity.Fax = request.Fax;
		entity.Phone = request.Phone;
		entity.PostalCode = request.PostalCode;

		if (isEdit)
		{
			customers.Update(entity);
		}

		await repository.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(string id, CancellationToken cancellationToken)
	{
		var customers = repository.Set<Customer>();
		var entity = await customers.FindAsync(id);

		if (entity == null)
		{
			throw new NotFoundException(nameof(Product), id);
		}

		var hasOrders = repository.Set<OrderDetail>().Any(od => od.Order.CustomerId == entity.CustomerId);
		if (hasOrders)
		{
			throw new DeleteFailureException(nameof(Product), id, "There are existing orders associated with this customer.");
		}

		customers.Remove(entity);

		await repository.SaveChangesAsync(cancellationToken);
	}
}