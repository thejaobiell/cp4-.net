using Microsoft.EntityFrameworkCore;
using SafeAlertApi.Data;
using SafeAlertApi.Models;

namespace SafeAlertApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Localidade> Localidades { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Postagem> Postagens { get; set; }
        public DbSet<Ocorrencia> Ocorrencias { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Postagem>().HasOne(p => p.Usuario).WithMany(u => u.Postagens).HasForeignKey(p => p.Usuario_id);
            modelBuilder.Entity<Postagem>().HasOne(p => p.Localidade).WithMany(l => l.Postagens).HasForeignKey(p => p.Localidade_id);
            modelBuilder.Entity<Postagem>().HasOne(p => p.Evento).WithMany(e => e.Postagens).HasForeignKey(p => p.Evento_id);
            modelBuilder.Entity<Ocorrencia>().HasOne(o => o.Postagem).WithMany(p => p.Ocorrencias).HasForeignKey(o => o.Postagem_id);
        }
    }
}