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
    public class VendorsController : ControllerBase
    {
        private readonly IGenericRepository<Vendor> _repository;

        public VendorsController(IGenericRepository<Vendor> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendors()
        {
            var vendors = await _repository.GetAllAsync();
            return Ok(vendors.Select(v => new VendorDto
            {
                Id = v.Id,
                Name = v.Name,
                ContactPerson = v.ContactPerson,
                Email = v.Email,
                Phone = v.Phone,
                Address = v.Address,
                GSTNumber = v.GSTNumber,
                IsActive = v.IsActive
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorDto>> GetVendor(int id)
        {
            var v = await _repository.GetByIdAsync(id);
            if (v == null) return NotFound();

            return Ok(new VendorDto
            {
                Id = v.Id,
                Name = v.Name,
                ContactPerson = v.ContactPerson,
                Email = v.Email,
                Phone = v.Phone,
                Address = v.Address,
                GSTNumber = v.GSTNumber,
                IsActive = v.IsActive
            });
        }

        [HttpPost]
        public async Task<ActionResult<VendorDto>> CreateVendor(VendorDto dto)
        {
            var v = new Vendor
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                GSTNumber = dto.GSTNumber,
                IsActive = dto.IsActive
            };

            await _repository.AddAsync(v);
            await _repository.SaveChangesAsync();

            dto.Id = v.Id;
            return CreatedAtAction(nameof(GetVendor), new { id = v.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, VendorDto dto)
        {
            var v = await _repository.GetByIdAsync(id);
            if (v == null) return NotFound();

            v.Name = dto.Name;
            v.ContactPerson = dto.ContactPerson;
            v.Email = dto.Email;
            v.Phone = dto.Phone;
            v.Address = dto.Address;
            v.GSTNumber = dto.GSTNumber;
            v.IsActive = dto.IsActive;

            _repository.Update(v);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var v = await _repository.GetByIdAsync(id);
            if (v == null) return NotFound();

            _repository.Delete(v);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
