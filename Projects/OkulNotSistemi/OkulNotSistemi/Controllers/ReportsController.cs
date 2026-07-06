using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkulNotSistemi.Models;

namespace OkulNotSistemi.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext dbContext;

        public ReportsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // 📊 Öğrenci Not Raporu
        public IActionResult StudentGrades()
        {
            var data = dbContext.Notlars
                .Include(x => x.Ogrenci)
                .Include(x => x.Ders)
                .ToList();

            return View(data);
        }


        public IActionResult EnBasarili5Ogrenci()
        {
            var result = (from n in dbContext.Notlars
                          join o in dbContext.Ogrencilers on n.OgrenciId equals o.OgrenciId
                          join d in dbContext.Derslers on n.DersId equals d.DersId

                          let ortalama = ((n.Vize ?? 0) * 0.4m) + ((n.Final ?? 0) * 0.6m)

                          orderby ortalama descending

                          select new Reports
                          {
                              OgrenciAdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                              Sinif = o.Sinif,
                              DersAd = d.DersAd,
                              Vize = n.Vize,
                              Final = n.Final,
                              Ortalama = ortalama,
                              Durum = ortalama >= 50 ? "Geçti" : "Kaldı"
                          })
                          .Take(5)
                          .ToList();

            return View(result);
        }
        public IActionResult DersNotDagilim()
        {
            var result = (from n in dbContext.Notlars
                          join o in dbContext.Ogrencilers on n.OgrenciId equals o.OgrenciId
                          join d in dbContext.Derslers on n.DersId equals d.DersId
                          select new Reports
                          {
                              OgrenciAdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                              Sinif = o.Sinif,
                              DersAd = d.DersAd,
                              Ortalama = (n.Vize * 0.4m) + (n.Final * 0.6m)
                          }).ToList();

            return View(result);
        }
        public IActionResult SinifBasariAnaliz()
        {
            var result = (from n in dbContext.Notlars
                          join d in dbContext.Derslers on n.DersId equals d.DersId
                          join o in dbContext.Ogrencilers on n.OgrenciId equals o.OgrenciId
                          select new Reports
                          {
                              DersAd = d.DersAd,
                              OgrenciAdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                              Ortalama = (n.Vize * 0.4m) + (n.Final * 0.6m)
                          }).ToList();

            return View(result);
        }
        public IActionResult SinifOrtalama()
        {
            var result = (from n in dbContext.Notlars
                          join o in dbContext.Ogrencilers on n.OgrenciId equals o.OgrenciId
                          group n by o.Sinif into g
                          select new Reports
                          {
                              Sinif = g.Key,
                              OgrenciSayisi = g.Select(x => x.OgrenciId).Distinct().Count(),
                              SinifOrtalamasi = g.Average(x => (x.Vize * 0.4m) + (x.Final * 0.6m))
                          }).ToList();

            return View(result);
        }
        public IActionResult OgrenciDurum()
        {
            var result = (from n in dbContext.Notlars
                          join o in dbContext.Ogrencilers on n.OgrenciId equals o.OgrenciId
                          join d in dbContext.Derslers on n.DersId equals d.DersId

                          let ortalama = ((n.Vize ?? 0) * 0.4m) + ((n.Final ?? 0) * 0.6m)

                          select new Reports
                          {
                              OgrenciAdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                              DersAd = d.DersAd,
                              Vize = n.Vize,
                              Final = n.Final,
                              Ortalama = ortalama,
                              Durum = ortalama >= 50 ? "Geçti" : "Kaldı"
                          }).ToList();

            return View(result);
        }
    }
}