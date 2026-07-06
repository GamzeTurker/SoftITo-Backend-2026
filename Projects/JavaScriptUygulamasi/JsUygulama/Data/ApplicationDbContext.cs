using Microsoft.EntityFrameworkCore;
using JsUygulama.Models;

namespace JsUygulama.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
