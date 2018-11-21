using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPlayground.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPlayground.Services
{
	public interface ICustomerService
	{
		bool TryGetCustomerById(Guid id, out Customer customer);
		Task<bool> TryAddCustomer(Customer newCustomer);
		Task<bool> TryUpdateCustomer(Customer customer);
		bool DoesEmailExist(string email);
		Task<bool> TryDeleteCustomer(Guid id);
	}
	public class CustomerService : ICustomerService
	{
		private readonly FlixOneStoreContext _context;
		public CustomerService(FlixOneStoreContext context)
		{
			_context = context;
		}

		public bool TryGetCustomerById(Guid id, out Customer customer)
		{
			customer = _context.Customers.FirstOrDefault(c => c.Id == id);
			return customer != null;
		}

		public async Task<bool> TryAddCustomer(Customer newCustomer)
		{
			newCustomer.Id = Guid.NewGuid();
			_context.Add(newCustomer);
			return await _context.SaveChangesAsync() > 0;
		}

		public bool DoesEmailExist(string email)
		{
			email = email.Trim();
			return _context.Customers.Any(c => c.Email == email);
		}

		public async Task<bool> TryUpdateCustomer(Customer customer)
		{
			_context.Entry(customer).State = EntityState.Modified;

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> TryDeleteCustomer(Guid id)
		{
			if (TryGetCustomerById(id, out var customer))
			{
				_context.Customers.Remove(customer);
				return await _context.SaveChangesAsync() > 0;
			}
			return false;
		}
	}
}
