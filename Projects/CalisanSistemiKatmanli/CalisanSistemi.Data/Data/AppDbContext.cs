using CalisanSistemi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalisanSistemi.Data.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Personel> Personels { get; set; }
        public DbSet<Departman> Departmans { get; set; }
        public DbSet<Gorev> Gorevs { get; set; }
        public DbSet<GorevTipi> GorevTipis { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
