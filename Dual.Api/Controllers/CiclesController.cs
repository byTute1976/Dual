using Dual.Shared.Models.Empresa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Dual.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CiclesController(IDistributedCache cache) : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static string KeyAll => "cicles:all";
        private static string KeyById(int id) => $"cicles:{id}";

        [HttpGet]
        public async Task<IActionResult> GetCicles()
        {
            var cached = await cache.GetStringAsync(KeyAll);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var fromCache = JsonSerializer.Deserialize<List<Cicle>>(cached, JsonOptions);
                return Ok(fromCache);
            }

            var cicles = await context.TCicles
                .Include(c => c.TTutors)
                .ToListAsync();

            var serialized = JsonSerializer.Serialize(cicles, JsonOptions);
            await cache.SetStringAsync(
                KeyAll,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });

            return Ok(cicles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCicle(int id)
        {
            var key = KeyById(id);

            var cached = await cache.GetStringAsync(key);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var fromCache = JsonSerializer.Deserialize<Cicle>(cached, JsonOptions);
                return Ok(fromCache);
            }

            var cicle = await context.TCicles
                .Include(c => c.TTutors)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cicle == null)
                return NotFound($"No s'ha trobat cap cicle amb l'ID {id}");

            var serialized = JsonSerializer.Serialize(cicle, JsonOptions);
            await cache.SetStringAsync(
                key,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return Ok(cicle);
        }

        [HttpPost]
        public async Task<IActionResult> PostCicle(Cicle cicle)
        {
            context.TCicles.Add(cicle);

            await context.SaveChangesAsync();

            await cache.RemoveAsync(KeyAll);
            await cache.RemoveAsync(KeyById(cicle.Id));

            return CreatedAtAction(nameof(GetCicle), new { id = cicle.Id }, cicle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCicle(int id, Cicle cicle)
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

            await cache.RemoveAsync(KeyAll);
            await cache.RemoveAsync(KeyById(id));

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

            await cache.RemoveAsync(KeyAll);
            await cache.RemoveAsync(KeyById(id));

            return NoContent();
        }

        private bool CicleExists(int id)
        {
            return context.TCicles.Any(e => e.Id == id);
        }
    }
}