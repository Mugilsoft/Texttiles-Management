using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Organization;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IGenericRepository<Branch> _repository;

        public BranchesController(IGenericRepository<Branch> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches()
        {
            var branches = await _repository.GetAllAsync();
            return Ok(branches.Select(b => new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                Address = b.Address,
                Phone = b.Phone,
                IsActive = b.IsActive
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDto>> GetBranch(int id)
        {
            var b = await _repository.GetByIdAsync(id);
            if (b == null) return NotFound();

            return Ok(new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                Address = b.Address,
                Phone = b.Phone,
                IsActive = b.IsActive
            });
        }

        [HttpPost]
        public async Task<ActionResult<BranchDto>> CreateBranch(BranchDto dto)
        {
            var b = new Branch
            {
                Name = dto.Name,
                Code = dto.Code,
                Address = dto.Address,
                Phone = dto.Phone,
                IsActive = dto.IsActive
            };

            await _repository.AddAsync(b);
            await _repository.SaveChangesAsync();

            dto.Id = b.Id;
            return CreatedAtAction(nameof(GetBranch), new { id = b.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, BranchDto dto)
        {
            var b = await _repository.GetByIdAsync(id);
            if (b == null) return NotFound();

            b.Name = dto.Name;
            b.Code = dto.Code;
            b.Address = dto.Address;
            b.Phone = dto.Phone;
            b.IsActive = dto.IsActive;

            _repository.Update(b);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var b = await _repository.GetByIdAsync(id);
            if (b == null) return NotFound();

            _repository.Delete(b);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
