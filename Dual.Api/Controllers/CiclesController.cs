using Dual.Api.Models;
using Dual.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dual.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CiclesController(DualContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCicles()
        {
            var cicles = await context.TCicles
                .Include(c => c.TTutors)
                .ToListAsync();
            
            return Ok(cicles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCicle(int id)
        {
            var cicle = await context.TCicles
                .Include(c => c.TTutors)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cicle == null)
                return NotFound($"No s'ha trobat cap cicle amb l'ID {id}");

            return Ok(cicle);
        }

        [HttpPost]
        public async Task<IActionResult> PostCicle(TCicle cicle)
        {
            context.TCicles.Add(cicle);

            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCicle), new { id = cicle.Id }, cicle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCicle(int id, TCicle cicle)
        {
            if (id != cicle.Id)
                return BadRequest("L'ID de la URL no coincideix amb l'ID de l'objecte enviat.");

            context.Entry(cicle).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CicleExists(id))
                {
                    return NotFound($"No es pot actualitzar. El cicle amb ID {id} no existeix.");
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCicle(int id)
        {
            var cicle = await context.TCicles.FindAsync(id);
            if (cicle == null)
            {
                return NotFound($"No es pot esborrar. El cicle amb ID {id} no existeix.");
            }

            context.TCicles.Remove(cicle);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CicleExists(int id)
        {
            return context.TCicles.Any(e => e.Id == id);
        }
    }
}