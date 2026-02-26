using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Partners;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IGenericRepository<Customer> _repository;

        public CustomersController(IGenericRepository<Customer> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _repository.GetAllAsync();
            return Ok(customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                RegistrationNumber = c.RegistrationNumber,
                LoyaltyPoints = c.LoyaltyPoints
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return NotFound();

            return Ok(new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                RegistrationNumber = c.RegistrationNumber,
                LoyaltyPoints = c.LoyaltyPoints
            });
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto dto)
        {
            var c = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                RegistrationNumber = dto.RegistrationNumber,
                LoyaltyPoints = dto.LoyaltyPoints
            };

            await _repository.AddAsync(c);
            await _repository.SaveChangesAsync();

            dto.Id = c.Id;
            return CreatedAtAction(nameof(GetCustomer), new { id = c.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto dto)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return NotFound();

            c.Name = dto.Name;
            c.Email = dto.Email;
            c.Phone = dto.Phone;
            c.Address = dto.Address;
            c.RegistrationNumber = dto.RegistrationNumber;
            c.LoyaltyPoints = dto.LoyaltyPoints;

            _repository.Update(c);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return NotFound();

            _repository.Delete(c);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
