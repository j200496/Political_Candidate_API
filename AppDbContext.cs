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
    }
}
