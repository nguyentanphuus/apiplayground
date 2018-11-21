using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPlayground.Models;
using ApiPlayground.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPlayground.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : ControllerBase
	{
		private readonly ICustomerService _customerService;

		public CustomersController(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<Customer> GetCustomer(Guid id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (_customerService.TryGetCustomerById(id, out var customer))
			{
				return Ok(customer);
			}

			return NotFound();
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Customer>> PostCustomer([FromBody]Customer customer)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (_customerService.DoesEmailExist(customer.Email))
			{
				ModelState.AddModelError("email", "User with mail id already exists!");
				return BadRequest(ModelState);
			}

			if (await _customerService.TryAddCustomer(customer))
			{
				return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
			}

			return BadRequest();
		}
		[HttpPut]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> PutCustomer(Guid id, Customer customer)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (await _customerService.TryUpdateCustomer(customer))
			{
				return NoContent();
			}

			return NotFound();
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DeleteCustomer(Guid id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (await _customerService.TryDeleteCustomer(id))
			{
				return Accepted();
			}
			return NotFound();
		}
	}
}
