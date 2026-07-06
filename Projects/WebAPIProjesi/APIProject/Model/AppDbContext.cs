using Microsoft.EntityFrameworkCore;

namespace APIProject.Model
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
       

        public DbSet<Dizi> Diziler{ get; set; }
        public DbSet<Film> Filmler { get; set; }
        public DbSet<CizgiFilm> CizgiFilmler { get; set; }
        public DbSet<Admin> Adminler { get; set; }
    }
}
