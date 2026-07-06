using EduCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EduCore.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly EducationDbContext _db;

        public DbInitializer(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            EducationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            // 1. Veritabanında uygulanmamış migration'lar varsa otomatik uygula
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Hata oluşursa loglanabilir
            }

            // 2. Rolleri oluştur (Admin ve Student)
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin" }).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole<int> { Name = "Student" }).GetAwaiter().GetResult();

                // 3. İlk Admin (Yönetici) Kullanıcısını Oluştur
                var adminUser = new User
                {
                    UserName = "admin@educore.com",
                    Email = "admin@educore.com",
                    EmailConfirmed = true,
                    Name = "Sistem Yöneticisi",
                    ProfileImageUrl = "/img/default-profile.png"
                };

                // Şifresini "Admin123*" olarak ayarlıyoruz
                _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();

                // 4. Admin kullanıcısına "Admin" rolünü ata
                User user = _db.Users.FirstOrDefault(u => u.Email == "admin@educore.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                }
            }
        }
    }
}