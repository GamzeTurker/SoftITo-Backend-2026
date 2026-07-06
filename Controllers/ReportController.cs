using Microsoft.AspNetCore.Mvc;
using projectcodefirst.Models;
using SiparisSistemi.Models;
using System;
using System.Linq;

namespace SiparisSistemi.Controllers
{
    public class ReportController : Controller
    {
        public readonly ApplicationDbContext dbContext;
        public ReportController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // RAPOR ANA SAYFASI (GET)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // COMBOBOX SEÇİMİNE GÖRE YÖNLENDİRME (POST)
        [HttpPost]
        public IActionResult Index(string rapor)
        {
            switch (rapor)
            {
                case "TumSiparisDetay":
                    return RedirectToAction("TumSiparisDetay");

                case "MusteriSiparisOzeti":
                    return RedirectToAction("MusteriSiparisOzeti");

                case "UrunSiparisDetaylari":
                    return RedirectToAction("UrunSiparisDetaylari");

                case "UrunSatisAnalizi":
                    return RedirectToAction("UrunSatisAnalizi");

                case "KritikStokUrunler":
                    return RedirectToAction("KritikStokUrunler");

                case "AktifSiparisler":
                    return RedirectToAction("AktifSiparisler");

                default:
                    return View();
            }
        }


        public IActionResult TumSiparisDetay()
        {
            var result = (from siparis in dbContext.Siparislers
                          join musteri in dbContext.Musterilers on siparis.MusteriId equals musteri.MusteriId
                          join urun in dbContext.Urunlers on siparis.UrunId equals urun.UrunId
                          select new Report
                          {
                              SiparisId = siparis.SiparisId,
                              MusteriAdSoyad = musteri.MusteriAdSoyad,
                              UrunAdi = urun.UrunAdi,
                              Adet = siparis.Adet,
                              ToplamTutar = siparis.ToplamTutar,
                              SiparisTarihi = siparis.SiparisTarihi,
                              SiparisDurumu = siparis.SiparisDurumu
                          }).ToList();

            return View(result);
        }

        // 2. Rapor: 2'li Join (Müşteri ve Sipariş)
        public IActionResult MusteriSiparisOzeti()
        {
            var result = (from siparis in dbContext.Siparislers
                          join musteri in dbContext.Musterilers on siparis.MusteriId equals musteri.MusteriId
                          select new Report
                          {
                              SiparisId = siparis.SiparisId,
                              MusteriAdSoyad = musteri.MusteriAdSoyad,
                              Telefon = musteri.Telefon,
                              SiparisTarihi = siparis.SiparisTarihi,
                              ToplamTutar = siparis.ToplamTutar
                          }).ToList();

            return View(result);
        }

        // 3. Rapor: 2'li Join (Ürün ve Sipariş)
        public IActionResult UrunSiparisDetaylari()
        {
            var result = (from siparis in dbContext.Siparislers
                          join urun in dbContext.Urunlers on siparis.UrunId equals urun.UrunId
                          select new Report
                          {
                              SiparisId = siparis.SiparisId,
                              UrunAdi = urun.UrunAdi,
                              Fiyat = urun.Fiyat,
                              Adet = siparis.Adet,
                              ToplamTutar = siparis.ToplamTutar,
                              SiparisDurumu = siparis.SiparisDurumu
                          }).ToList();

            return View(result);
        }

        // 4. Rapor: Group By (Ürün Satış Analizi)
        public IActionResult UrunSatisAnalizi()
        {
            var result = (from siparis in dbContext.Siparislers
                          join urun in dbContext.Urunlers on siparis.UrunId equals urun.UrunId
                          group new { siparis, urun } by urun.UrunAdi into grup
                          select new Report
                          {
                              UrunAdi = grup.Key,
                              ToplamSatisAdeti = grup.Sum(x => x.siparis.Adet),
                              ToplamCiro = grup.Sum(x => x.siparis.ToplamTutar)
                          }).ToList();

            return View(result);
        }

        // 5. Rapor: 1. Normal Where (Kritik Stoktaki Ürünler)
        public IActionResult KritikStokUrunler()
        {
            var result = (from urun in dbContext.Urunlers
                          where urun.Stok < 10
                          select new Report
                          {
                              UrunAdi = urun.UrunAdi,
                              Fiyat = urun.Fiyat,
                              Stok = urun.Stok
                          }).ToList();

            return View(result);
        }

        // 6. Rapor: 2. Normal Where (Aktif Siparişler)
        public IActionResult AktifSiparisler()
        {
            var result = (from siparis in dbContext.Siparislers
                          where siparis.SiparisDurumu != "Teslim Edildi"
                          select new Report
                          {
                              SiparisId = siparis.SiparisId,
                              Adet = siparis.Adet,
                              ToplamTutar = siparis.ToplamTutar,
                              SiparisTarihi = siparis.SiparisTarihi,
                              SiparisDurumu = siparis.SiparisDurumu
                          }).ToList();

            return View(result);
        }

    }
}

