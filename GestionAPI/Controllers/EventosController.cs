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
    public class EventosController : ControllerBase
    {
        private readonly GestionDbContext _context;
        private readonly IConnectionMultiplexer _redis;

        public EventosController(GestionDbContext context, IConnectionMultiplexer redis)
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
            var db = _redis.GetDatabase();
            string cacheKey = "evento_" + id.ToString();
            var eventoCache = await db.StringGetAsync(cacheKey);
            if (!eventoCache.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<Evento>(eventoCache);
            }
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(evento), TimeSpan.FromMinutes(10));
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
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            var db = _redis.GetDatabase();
            string cacheKeyList = "eventoList";
            await db.KeyDeleteAsync(cacheKeyList);
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
