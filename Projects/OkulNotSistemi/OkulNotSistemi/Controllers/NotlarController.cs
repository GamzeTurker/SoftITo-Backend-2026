using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OkulNotSistemi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Drawing;
using System.Reflection.Metadata;
using Document = QuestPDF.Fluent.Document;
namespace OkulNotSistemi.Controllers
{
    public class NotlarController : Controller
    {
        public readonly AppDbContext dbContext;
        public NotlarController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(string search)
        {
            var query = dbContext.Notlars
                .Include(x => x.Ogrenci)
                .Include(x => x.Ders)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.Ogrenci.OgrenciAd.Contains(search) ||
                    x.Ogrenci.OgrenciSoyad.Contains(search) ||
                    x.Ders.DersAd.Contains(search)
                );
            }

            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Ogrenciler = dbContext.Ogrencilers.ToList();
            ViewBag.Dersler = dbContext.Derslers.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Notlar notlar)
        {
            dbContext.Notlars.Add(notlar);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var not = dbContext.Notlars.Find(id);
            ViewBag.Ogrenciler = dbContext.Ogrencilers.ToList();
            ViewBag.Dersler = dbContext.Derslers.ToList();

            return View(not);
        }
        [HttpPost]
        public IActionResult Edit(Notlar notlar)
        {
            dbContext.Update(notlar);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var not = dbContext.Notlars.Find(id);
            ViewBag.Ogrenciler = dbContext.Ogrencilers.ToList();
            ViewBag.Dersler = dbContext.Derslers.ToList();

            return View(not);
        }
        [HttpPost]
        public IActionResult Delete(Notlar notlar)
        {
            dbContext.Remove(notlar);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ExportPdf()
        {
            var data = dbContext.Notlars
                .Include(x => x.Ogrenci)
                .Include(x => x.Ders)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Header()
                        .Text("📊 Notlar Raporu")
                        .FontSize(20)
                        .Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Öğrenci").Bold();
                            header.Cell().Text("Ders").Bold();
                            header.Cell().Text("Vize").Bold();
                            header.Cell().Text("Final").Bold();
                            header.Cell().Text("Ortalama").Bold();
                        });

                        foreach (var item in data)
                        {
                            decimal ort = ((item.Vize ?? 0) * 0.4m) + ((item.Final ?? 0) * 0.6m);

                            table.Cell().Text(item.Ogrenci.OgrenciAd + " " + item.Ogrenci.OgrenciSoyad);
                            table.Cell().Text(item.Ders.DersAd);
                            table.Cell().Text(item.Vize?.ToString() ?? "0");
                            table.Cell().Text(item.Final?.ToString() ?? "0");
                            table.Cell().Text(ort.ToString("0.00"));
                        }
                    });
                });
            });

            var pdf = document.GeneratePdf();
            return File(pdf, "application/pdf", "Notlar.pdf");
        }
        public IActionResult ExportExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("App");

            var data = dbContext.Notlars
                .Include(x => x.Ogrenci)
                .Include(x => x.Ders)
                .ToList();

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Notlar");

                // Header
                sheet.Cells[1, 1].Value = "Öğrenci";
                sheet.Cells[1, 2].Value = "Ders";
                sheet.Cells[1, 3].Value = "Vize";
                sheet.Cells[1, 4].Value = "Final";
                sheet.Cells[1, 5].Value = "Ortalama";

                using (var range = sheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                int row = 2;

                foreach (var item in data)
                {
                    decimal ort = ((item.Vize ?? 0) * 0.4m) + ((item.Final ?? 0) * 0.6m);

                    sheet.Cells[row, 1].Value = item.Ogrenci.OgrenciAd + " " + item.Ogrenci.OgrenciSoyad;
                    sheet.Cells[row, 2].Value = item.Ders.DersAd;
                    sheet.Cells[row, 3].Value = item.Vize;
                    sheet.Cells[row, 4].Value = item.Final;
                    sheet.Cells[row, 5].Value = ort;

                    row++;
                }

                sheet.Cells.AutoFitColumns();

                var file = package.GetAsByteArray();
                return File(file,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Notlar.xlsx");
            }
        }
    }
}
