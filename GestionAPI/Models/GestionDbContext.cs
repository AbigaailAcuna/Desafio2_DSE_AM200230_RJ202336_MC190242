using Microsoft.EntityFrameworkCore;

namespace GestionAPI.Models
{
    public class GestionDbContext : DbContext
    {
        public GestionDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Organizador> Organizadores { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
