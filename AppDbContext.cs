using Candidate.Models;
using Microsoft.EntityFrameworkCore;
namespace Candidate
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Personas> Personas { get; set; }
        public DbSet<Provincias> Provincias { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Usuario_Provincia> Usuario_Provincia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario_Provincia>()
     .HasKey(up => new { up.IdUsuario, up.IdProvincia });

            modelBuilder.Entity<Personas>()
         .HasOne(p => p.Provincia)
         .WithMany()
         .HasForeignKey(p => p.IdProvincia)
         .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Personas>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

        }


    }
}
