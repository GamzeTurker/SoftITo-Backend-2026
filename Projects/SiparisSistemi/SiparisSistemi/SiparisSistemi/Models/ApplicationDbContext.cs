using Microsoft.EntityFrameworkCore;
using SiparisSistemi.Models;

namespace projectcodefirst.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
            dbContext) : base(dbContext)
        {

        }
        //veritabanında tablo oluşturur
        public DbSet<Siparisler> Siparislers { get; set; }
        public DbSet<Urunler> Urunlers { get; set; }
        public DbSet<Musteriler> Musterilers { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
