using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Organization;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountersController : ControllerBase
    {
        private readonly IGenericRepository<Counter> _repository;
        private readonly ApplicationDbContext _context;

        public CountersController(IGenericRepository<Counter> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CounterDto>>> GetCounters()
        {
            var counters = await _context.Counters.Include(c => c.Branch).ToListAsync();
            return Ok(counters.Select(c => new CounterDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                BranchId = c.BranchId,
                BranchName = c.Branch?.Name,
                IsActive = c.IsActive
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CounterDto>> GetCounter(int id)
        {
            var c = await _context.Counters.Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();

            return Ok(new CounterDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                BranchId = c.BranchId,
                BranchName = c.Branch?.Name,
                IsActive = c.IsActive
            });
        }

        [HttpPost]
        public async Task<ActionResult<CounterDto>> CreateCounter(CounterDto dto)
        {
            var c = new Counter
            {
                Name = dto.Name,
                Code = dto.Code,
                BranchId = dto.BranchId,
                IsActive = dto.IsActive
            };

            await _repository.AddAsync(c);
            await _repository.SaveChangesAsync();

            dto.Id = c.Id;
            return CreatedAtAction(nameof(GetCounter), new { id = c.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCounter(int id, CounterDto dto)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return NotFound();

            c.Name = dto.Name;
            c.Code = dto.Code;
            c.BranchId = dto.BranchId;
            c.IsActive = dto.IsActive;

            _repository.Update(c);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounter(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return NotFound();

            _repository.Delete(c);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
