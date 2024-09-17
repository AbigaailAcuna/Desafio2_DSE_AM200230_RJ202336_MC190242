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
using Microsoft.IdentityModel.Tokens;

namespace GestionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly GestionDbContext _context;
        private readonly IConnectionMultiplexer _redis;

        public EventosController(GestionDbContext context, IConnectionMultiplexer? redis=null)
        {
            _context = context;
            _redis = redis;
        }

        // GET: api/Eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
        {
            var db = _redis.GetDatabase();
            string cacheKey = "eventosList";
            var eventosCache = await db.StringGetAsync(cacheKey);
            if (!eventosCache.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<Evento>>(eventosCache);
            }
            var eventos = await _context.Eventos.ToListAsync();
            await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(eventos), TimeSpan.FromMinutes(10));
            return eventos;
        }

        // GET: api/Eventos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evento>> GetEvento(int id)
        {
            if(_redis != null)
            {
                var db = _redis.GetDatabase();
                string cacheKey = "evento_" + id.ToString();
                var eventoCache = await db.StringGetAsync(cacheKey);
                if (!eventoCache.IsNullOrEmpty)
                {
                    return JsonSerializer.Deserialize<Evento>(eventoCache);
                }
            }
 
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            if(_redis != null)
            {
                var db = _redis.GetDatabase();
                string cacheKey = "evento_" + id.ToString();
                await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(evento), TimeSpan.FromMinutes(10));
            }
            return evento;
        }

        // PUT: api/Eventos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, Evento evento)
        {
            if (id != evento.Id)
            {
                return BadRequest();
            }
            _context.Entry(evento).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                var db = _redis.GetDatabase();
                string cacheKeyEvento = "evento_" + id.ToString();
                string cacheKeyList = "eventoList";
                await db.KeyDeleteAsync(cacheKeyEvento);
                await db.KeyDeleteAsync(cacheKeyList);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(id))
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

        // POST: api/Eventos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Evento>> PostEvento(Evento evento)
        {
            // Validar campos del evento 
            if (string.IsNullOrEmpty(evento.NombreEvento) || evento.NombreEvento.Length < 5 || evento.NombreEvento.Length > 100)
            {
                return BadRequest("El nombre del evento debe tener entre 5 y 100 caracteres.");
            }
            if (evento.FechaEvento == default || evento.FechaEvento < DateTime.Now)
            {
                return BadRequest("La fecha del evento no puede ser nula y debe ser válida");
            }
            // Validar campos del evento 
            if (string.IsNullOrEmpty(evento.LugarEvento) || evento.LugarEvento.Length < 5 || evento.LugarEvento.Length > 100)
            {
                return BadRequest("El lugar del evento debe tener entre 5 y 100 caracteres.");
            }
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            if (_redis != null)
            {
                var db = _redis.GetDatabase();
                string cacheKeyList = "eventoList";
                await db.KeyDeleteAsync(cacheKeyList);
            }
            
            return CreatedAtAction("GetEvento", new { id = evento.Id }, evento);

        }

        // DELETE: api/Eventos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            var db = _redis.GetDatabase();
            string cacheKeyEvento = "evento_" + id.ToString();
            string cacheKeyList = "eventoList";
            await db.KeyDeleteAsync(cacheKeyEvento);
            await db.KeyDeleteAsync(cacheKeyList);
            return NoContent();
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}
