using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using projectcodefirst.Models;
using SiparisSistemi.Migrations;
using SiparisSistemi.Models;
using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
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
            var siparisler = dbContext.Siparislers
                .Include(x => x.musteriler)
                .Include(x => x.urunler)
                .AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                siparisler = siparisler.Where(x =>
                    x.musteriler.MusteriAdSoyad.Contains(arama) ||
                    x.urunler.UrunAdi.Contains(arama));
            }

            return View(siparisler.ToList());
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
            var mevcutSiparis = dbContext.Siparislers.Find(siparisler.SiparisId);

            if (mevcutSiparis == null)
                return NotFound();

            var urun = dbContext.Urunlers.Find(siparisler.UrunId);

            if (urun == null)
                return NotFound();

            // Eski stok geri ekleniyor
            urun.Stok += mevcutSiparis.Adet;

            // Yeni stok düşülüyor
            urun.Stok -= siparisler.Adet;

            // Sipariş bilgileri güncelleniyor
            mevcutSiparis.MusteriId = siparisler.MusteriId;
            mevcutSiparis.UrunId = siparisler.UrunId;
            mevcutSiparis.Adet = siparisler.Adet;
            mevcutSiparis.SiparisTarihi = siparisler.SiparisTarihi;
            mevcutSiparis.SiparisDurumu = siparisler.SiparisDurumu;
            mevcutSiparis.ToplamTutar = urun.Fiyat * siparisler.Adet;

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

        }

        [HttpPost]
public IActionResult Delete(Siparisler siparis)
{
    var dbSiparis = dbContext.Siparislers.Find(siparis.SiparisId);

    if (dbSiparis != null)
    {
        var urun = dbContext.Urunlers.Find(dbSiparis.UrunId);

        if (urun != null)
        {
            // 🔁 stok geri eklenir
            urun.Stok += dbSiparis.Adet;
        }

        dbContext.Siparislers.Remove(dbSiparis);
        dbContext.SaveChanges();
    }

    return RedirectToAction("Index");
        }
        public IActionResult ExportToPdf()
        {
            var siparisler = dbContext.Siparislers
                .Include(x => x.musteriler)
                .Include(x => x.urunler)
                .ToList();

        var pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header()
                    .Text("Sipariş Listesi")
                    .FontSize(20)
                    .Bold();

                page.Content().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(80);
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("ID").Bold();
                        header.Cell().Text("Müşteri").Bold();
                        header.Cell().Text("Ürün").Bold();
                        header.Cell().Text("Adet").Bold();
                        header.Cell().Text("Toplam").Bold();
                        header.Cell().Text("Durum").Bold();
                    });

                    foreach (var item in siparisler)
                    {
                        table.Cell().Text(item.SiparisId.ToString());
                        table.Cell().Text(item.musteriler?.MusteriAdSoyad ?? "");
                        table.Cell().Text(item.urunler?.UrunAdi ?? "");
                        table.Cell().Text(item.Adet.ToString());
                        table.Cell().Text(item.ToplamTutar.ToString("C"));
                        table.Cell().Text(item.SiparisDurumu);
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                    });
            });
        });

        var pdfBytes = pdfDocument.GeneratePdf();

            return File(pdfBytes,
                "application/pdf",
                $"Siparis_Listesi_{DateTime.Now:yyyyMMdd}.pdf");
        }
        public IActionResult ExportToExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Backend softito");

            var siparisler = dbContext.Siparislers
                .Include(x => x.musteriler)
                .Include(x => x.urunler)
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Siparişler");

                worksheet.Cells[1, 1].Value = "Sipariş ID";
                worksheet.Cells[1, 2].Value = "Müşteri";
                worksheet.Cells[1, 3].Value = "Ürün";
                worksheet.Cells[1, 4].Value = "Adet";
                worksheet.Cells[1, 5].Value = "Sipariş Tarihi";
                worksheet.Cells[1, 6].Value = "Toplam Tutar";
                worksheet.Cells[1, 7].Value = "Durum";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 2;

                foreach (var item in siparisler)
                {
                    worksheet.Cells[row, 1].Value = item.SiparisId;
                    worksheet.Cells[row, 2].Value = item.musteriler?.MusteriAdSoyad;
                    worksheet.Cells[row, 3].Value = item.urunler?.UrunAdi;
                    worksheet.Cells[row, 4].Value = item.Adet;
                    worksheet.Cells[row, 5].Value = item.SiparisTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 6].Value = item.ToplamTutar;
                    worksheet.Cells[row, 7].Value = item.SiparisDurumu;

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var bytes = package.GetAsByteArray();

                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Siparis_Listesi_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

    }
}
