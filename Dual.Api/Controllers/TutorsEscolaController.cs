using Dual.Api.Models;
using Dual.Shared.Models.Empresa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Dual.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorsEscolaController(DualContext context, IDistributedCache cache) : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static string KeyAll => "Tutors:all";
        private static string KeyById(int id) => $"Tutors:{id}";

        [HttpGet]
        public async Task<IActionResult> GetTutors()
        {
            var cached = await cache.GetStringAsync(KeyAll);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var fromCache = JsonSerializer.Deserialize<List<Tutor>>(cached, JsonOptions);
                return Ok(fromCache);
            }

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

            var serialized = JsonSerializer.Serialize(tutors, JsonOptions);
            await cache.SetStringAsync(
                KeyAll,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });

            return Ok(tutors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTutor(int id)
        {
            var key = KeyById(id);

            var cached = await cache.GetStringAsync(key);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var fromCache = JsonSerializer.Deserialize<Tutor>(cached, JsonOptions);
                return Ok(fromCache);
            }

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
                return NotFound($"No s'ha trobat cap Tutor amb l'ID {id}");

            var serialized = JsonSerializer.Serialize(tutor, JsonOptions);
            await cache.SetStringAsync(
                key,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return Ok(tutor);
        }

        [HttpPost]
        public async Task<IActionResult> PostTutor(Tutor tutor)
        {
            context.TTutors.Add(tutor);

            await context.SaveChangesAsync();

            await cache.RemoveAsync(KeyAll);
            await cache.RemoveAsync(KeyById(tutor.Id));

            return CreatedAtAction(nameof(GetTutor), new { id = tutor.Id }, tutor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutor(int id, Tutor tutor)
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

            await cache.RemoveAsync(KeyAll);
            await cache.RemoveAsync(KeyById(id));

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

            await cache.RemoveAsync(KeyAll);
            await cache.RemoveAsync(KeyById(id));

            return NoContent();
        }

        private bool TutorExists(int id)
        {
            return context.TTutors.Any(e => e.Id == id);
        }
    }
}
