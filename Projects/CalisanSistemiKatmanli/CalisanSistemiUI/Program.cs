using CalisanSistemi.Data.Data;
using CalisanSistemi.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.
UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Admins.Any())
        {
            context.Admins.Add(new Admin { KullaniciAdi = "admin", Sifre = "admin123", Email = "admin@softito.com" });
        }

        if (!context.Departmans.Any())
        {
            var depts = new List<Departman>
            {
                new Departman { DepartmanAdi = "Yazılım Geliştirme", CalisanSayisi = 2, Aciklama = "Web ve Mobil uygulama geliştirme departmanı." },
                new Departman { DepartmanAdi = "Tasarım & Arayüz", CalisanSayisi = 1, Aciklama = "Kullanıcı arayüzü ve deneyimi (UI/UX) tasarım departmanı." },
                new Departman { DepartmanAdi = "İnsan Kaynakları", CalisanSayisi = 1, Aciklama = "İşe alım, eğitim ve çalışan ilişkileri departmanı." }
            };
            context.Departmans.AddRange(depts);
            context.SaveChanges();
        }

        if (!context.GorevTipis.Any())
        {
            var types = new List<GorevTipi>
            {
                new GorevTipi { Ad = "Backend API Geliştirme" },
                new GorevTipi { Ad = "Mobil Tasarım Mockup Hazırlama" },
                new GorevTipi { Ad = "Yeni Personel Aday Mülakatı" },
                new GorevTipi { Ad = "Haftalık Veritabanı Yedeklemesi" }
            };
            context.GorevTipis.AddRange(types);
            context.SaveChanges();
        }

        if (!context.Personels.Any())
        {
            var yaziDept = context.Departmans.First(d => d.DepartmanAdi == "Yazılım Geliştirme");
            var tasaDept = context.Departmans.First(d => d.DepartmanAdi == "Tasarım & Arayüz");
            var insanDept = context.Departmans.First(d => d.DepartmanAdi == "İnsan Kaynakları");

            var staff = new List<Personel>
            {
                new Personel { PersonelAdSoyad = "Ahmet Yılmaz", Email = "ahmet.yilmaz@softito.com", Maas = 55000, Yas = 32, Meslek = "Kıdemli Yazılımcı", DepartmanNo = yaziDept.DepartmanNo },
                new Personel { PersonelAdSoyad = "Elif Şahin", Email = "elif.sahin@softito.com", Maas = 48000, Yas = 28, Meslek = "Arayüz Tasarımcısı", DepartmanNo = tasaDept.DepartmanNo },
                new Personel { PersonelAdSoyad = "Mehmet Demir", Email = "mehmet.demir@softito.com", Maas = 42000, Yas = 35, Meslek = "İK Uzmanı", DepartmanNo = insanDept.DepartmanNo },
                new Personel { PersonelAdSoyad = "Cem Kozan", Email = "cem.kozan@softito.com", Maas = 52000, Yas = 30, Meslek = "Yazılım Geliştirici", DepartmanNo = yaziDept.DepartmanNo }
            };
            context.Personels.AddRange(staff);
            context.SaveChanges();
        }

        if (!context.Gorevs.Any())
        {
            var ahmet = context.Personels.First(p => p.PersonelAdSoyad == "Ahmet Yılmaz");
            var elif = context.Personels.First(p => p.PersonelAdSoyad == "Elif Şahin");
            var mehmet = context.Personels.First(p => p.PersonelAdSoyad == "Mehmet Demir");
            var cem = context.Personels.First(p => p.PersonelAdSoyad == "Cem Kozan");

            var apiType = context.GorevTipis.First(t => t.Ad == "Backend API Geliştirme");
            var uiType = context.GorevTipis.First(t => t.Ad == "Mobil Tasarım Mockup Hazırlama");
            var hrType = context.GorevTipis.First(t => t.Ad == "Yeni Personel Aday Mülakatı");
            var dbType = context.GorevTipis.First(t => t.Ad == "Haftalık Veritabanı Yedeklemesi");

            var tasks = new List<Gorev>
            {
                new Gorev { GorevTipId = apiType.GorevTipId, PersonelNo = ahmet.PersonelNo, TamamlandiMi = false, SonTarih = DateTime.Now.AddDays(9) },
                new Gorev { GorevTipId = uiType.GorevTipId, PersonelNo = elif.PersonelNo, TamamlandiMi = true, SonTarih = DateTime.Now.AddDays(-5) },
                new Gorev { GorevTipId = hrType.GorevTipId, PersonelNo = mehmet.PersonelNo, TamamlandiMi = false, SonTarih = DateTime.Now.AddDays(4) },
                new Gorev { GorevTipId = dbType.GorevTipId, PersonelNo = cem.PersonelNo, TamamlandiMi = true, SonTarih = DateTime.Now.AddDays(-1) }
            };
            context.Gorevs.AddRange(tasks);
            context.SaveChanges();
        }
    }
}
