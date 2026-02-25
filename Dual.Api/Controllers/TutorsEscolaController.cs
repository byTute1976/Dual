using Dual.Api.Models;
using Dual.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dual.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorsEscolaController(DualContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTutors()
        {
            var tutors = await context.TTutors
                .Select(t => new
                {
                    t.Id,
                    t.Nom,
                    t.Correu,
                    t.Observacions,

                    Cicle = new
                    {
                        t.IdCicleNavigation.Id,
                        t.IdCicleNavigation.Codi,
                        t.IdCicleNavigation.Nom
                    }
                })
                .ToListAsync();
            
            return Ok(tutors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTutor(int id)
        {
            var tutor = await context.TTutors
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Nom,
                    t.Correu,
                    t.Observacions,

                    Cicle = new
                    {
                        t.IdCicleNavigation.Id,
                        t.IdCicleNavigation.Codi,
                        t.IdCicleNavigation.Nom
                    }
                })
                .FirstOrDefaultAsync(); // Executem la consulta i agafem el primer resultat (o null)

            if (tutor == null)
            {
                return NotFound($"No s'ha trobat cap Tutor amb l'ID {id}");
            }

            return Ok(tutor);
        }

        [HttpPost]
        public async Task<IActionResult> PostTutor(TTutor tutor)
        {
            context.TTutors.Add(tutor);

            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTutor), new { id = tutor.Id }, tutor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutor(int id, TTutor tutor)
        {
            if (id != tutor.Id)
                return BadRequest("L'ID de la URL no coincideix amb l'ID de l'objecte enviat.");

            context.Entry(tutor).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorExists(id))
                {
                    return NotFound($"No es pot actualitzar. El Tutor amb ID {id} no existeix.");
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            var tutor = await context.TTutors.FindAsync(id);
            if (tutor == null)
            {
                return NotFound($"No es pot esborrar. El Tutor amb ID {id} no existeix.");
            }

            context.TTutors.Remove(tutor);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool TutorExists(int id)
        {
            return context.TTutors.Any(e => e.Id == id);
        }
    }
}
