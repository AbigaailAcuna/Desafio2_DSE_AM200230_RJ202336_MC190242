using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionAPI.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace GestionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizadoresController : ControllerBase
    {
        private readonly GestionDbContext _context;
        private readonly IConnectionMultiplexer _redis;

        public OrganizadoresController(GestionDbContext context, IConnectionMultiplexer? redis=null)
        {
            _context = context;
            _redis = redis;
        }

        // GET: api/Organizadores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organizador>>> GetOrganizadores()
        {
            var db = _redis.GetDatabase();
            string cacheKey = "organizadoresList";
            var organizadoresCache = await db.StringGetAsync(cacheKey);
            if (!organizadoresCache.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<Organizador>>(organizadoresCache);
            }
            var organizadores = await _context.Organizadores.ToListAsync();
            await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(organizadores), TimeSpan.FromMinutes(10));
            return organizadores;
        }

        // GET: api/Organizadores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Organizador>> GetOrganizador(int id)
        {
            if (_redis != null)
            {
                var db = _redis.GetDatabase();
                string cacheKey = "organizador_" + id.ToString();
                var organizadoresCache = await db.StringGetAsync(cacheKey);
                if (!organizadoresCache.IsNullOrEmpty)
                {
                    return JsonSerializer.Deserialize<Organizador>(organizadoresCache);
                }
            }
            
            var organizador = await _context.Organizadores.FindAsync(id);
            if (organizador == null)
            {
                return NotFound();
            }
            if (_redis != null)
            {
                var db = _redis.GetDatabase();
                string cacheKey = "organizador_" + id.ToString();
                await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(organizador), TimeSpan.FromMinutes(10));
               
            }
            return organizador;
        }

        // PUT: api/Organizadores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganizador(int id, Organizador organizador)
        {
            if (id != organizador.Id)
            {
                return BadRequest();
            }
            _context.Entry(organizador).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                var db = _redis.GetDatabase();
                string cacheKeyOrganizador = "organizador_" + id.ToString();
                string cacheKeyList = "organizadorList";
                await db.KeyDeleteAsync(cacheKeyOrganizador);
                await db.KeyDeleteAsync(cacheKeyList);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizadorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
            return NoContent();
        }

        // POST: api/Organizadores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Organizador>> PostOrganizador(Organizador organizador)
        {
            // Validar campos del organizador 
            if (string.IsNullOrEmpty(organizador.NombreOrganizador) || organizador.NombreOrganizador.Length < 3 || organizador.NombreOrganizador.Length > 50)
            {
                return BadRequest("El nombre del organizador debe tener entre 3 y 50 caracteres.");
            }
            if (string.IsNullOrEmpty(organizador.CargoOrganizador) || organizador.CargoOrganizador.Length < 3 || organizador.CargoOrganizador.Length > 50)
            {
                return BadRequest("El cargo del organizador debe tener entre 3 y 50 caracteres.");
            }
            _context.Organizadores.Add(organizador);
            await _context.SaveChangesAsync();
            if (_redis != null)
            {
                var db = _redis.GetDatabase();
                string cacheKeyList = "organizadorList";
                await db.KeyDeleteAsync(cacheKeyList);
            }
          
            return CreatedAtAction("GetOrganizador", new { id = organizador.Id }, organizador);
        }

        // DELETE: api/Organizadores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganizador(int id)
        {
            var organizador = await _context.Organizadores.FindAsync(id);
            if (organizador == null)
            {
                return NotFound();
            }
            _context.Organizadores.Remove(organizador);
            await _context.SaveChangesAsync();
            var db = _redis.GetDatabase();
            string cacheKeyOrganizador = "organizador_" + id.ToString();
            string cacheKeyList = "organizadorList";
            await db.KeyDeleteAsync(cacheKeyOrganizador);
            await db.KeyDeleteAsync(cacheKeyList);
            return NoContent();
        }

        private bool OrganizadorExists(int id)
        {
            return _context.Organizadores.Any(e => e.Id == id);
        }
    }
}
