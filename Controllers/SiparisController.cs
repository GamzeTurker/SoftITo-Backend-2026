using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projectcodefirst.Models;
using SiparisSistemi.Models;

namespace SiparisSistemi.Controllers
{
    public class SiparisController : Controller
    {
        public readonly ApplicationDbContext dbContext;
        public SiparisController
            (ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(string arama)
        {
            var siparisler = dbContext.Siparislers.Include("musteriler").Include("urunler").ToList();
            

            return View(siparisler);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Musteriler = new SelectList(dbContext.Musterilers, "MusteriId", "MusteriAdSoyad");
            ViewBag.Urunler = new SelectList(dbContext.Urunlers, "UrunId", "UrunAdi");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Siparisler siparisler)
        {
            var urun = dbContext.Urunlers.Find(siparisler.UrunId);

            if (urun == null)
            {
                ModelState.AddModelError("", "Ürün bulunamadı!");
            }
            else if (urun.Stok < siparisler.Adet)
            {
                ModelState.AddModelError("", "Yetersiz stok!");
            }

            // ❗ HATA VARSA GERİ DÖN
            if (!ModelState.IsValid)
            {
                ViewBag.Musteriler = new SelectList(dbContext.Musterilers, "MusteriId", "MusteriAdSoyad");
                ViewBag.Urunler = new SelectList(dbContext.Urunlers, "UrunId", "UrunAdi");

                return View(siparisler);
            }
            // 💰 toplam hesap
            siparisler.ToplamTutar = urun.Fiyat * siparisler.Adet;

            // 📉 stok düş
            urun.Stok -= siparisler.Adet;
            dbContext.Siparislers.Add(siparisler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var result = dbContext.Siparislers.Find(id);
            ViewBag.Musteriler = new SelectList(dbContext.Musterilers, "MusteriId", "MusteriAdSoyad");
            ViewBag.Urunler = new SelectList(dbContext.Urunlers, "UrunId", "UrunAdi");
            return View(result);
        }

        [HttpPost]
        public IActionResult Edit(Siparisler siparisler)
        {
            var urun = dbContext.Urunlers.Find(siparisler.UrunId);

            siparisler.ToplamTutar = urun.Fiyat * siparisler.Adet;
            dbContext.Update(siparisler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var result = dbContext.Siparislers
            .Include(x => x.musteriler)
            .Include(x => x.urunler)
            .FirstOrDefault(x => x.SiparisId == id);

            return View(result);


            return View(result);
        }

        [HttpPost]
        public IActionResult Delete(Siparisler siparisler)
        {
          
            
            dbContext.Remove(siparisler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
