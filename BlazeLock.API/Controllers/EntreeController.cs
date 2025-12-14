using BlazeLock.API.Models;
using BlazeLock.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/entree")]
    public class EntreeController : ControllerBase
    {
        private readonly IEntreeService _service;

        public EntreeController(IEntreeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entrees = await _service.GetAllAsync();
            return Ok(entrees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entree = await _service.GetByIdAsync(id);

            if (entree == null)
                return NotFound();

            return Ok(entree);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Entree entree)
        {
            var createdEntree = await _service.CreateAsync(entree);

            return CreatedAtAction(nameof(GetById), new { id = createdEntree.IdEntree }, createdEntree);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Entree entree)
        {
            if (id != entree.IdEntree) return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(id, entree);

            if (!updated)
                return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}