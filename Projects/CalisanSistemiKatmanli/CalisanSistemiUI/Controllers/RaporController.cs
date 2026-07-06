using CalisanSistemi.Data.Data;
using CalisanSistemi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;

namespace CalisanSistemiUI.Controllers
{
    public class RaporController : Controller
    {
        private readonly AppDbContext _dbContext;

        public RaporController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new RaporModel
            {
                ToplamPersonel = _dbContext.Personels.Count(),
                ToplamDepartman = _dbContext.Departmans.Count(),
                ToplamGorev = _dbContext.Gorevs.Count(),
                TamamlananGorevSayisi = _dbContext.Gorevs.Count(x => x.TamamlandiMi),
                BekleyenGorevSayisi = _dbContext.Gorevs.Count(x => !x.TamamlandiMi)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string rapor)
        {
            switch (rapor)
            {
                case "Personel":
                    return RedirectToAction("YasRapor");
                case "Departman":
                    return RedirectToAction("DepartmanRaporu");
                case "Gorev":
                    return RedirectToAction("GorevRaporu");
                case "Tamamlanan":
                    return RedirectToAction("TamamlananGorevler");
                case "Bekleyen":
                    return RedirectToAction("BekleyenGorevler");
                default:
                    return View();
            }
        }

        // ==========================================
        // YAS RAPORU
        // ==========================================
        public IActionResult YasRapor()
        {
            var model = new RaporModel
            {
                PersonelListesi = _dbContext.Personels.Include(x => x.Departman)
                                            .Where(personel => personel.Yas >= 30)
                                            .ToList()
            };
            return View(model);
        }

        public IActionResult ExportYasRaporExcel()
        {
            var list = _dbContext.Personels.Include(x => x.Departman)
                                            .Where(p => p.Yas >= 30)
                                            .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("30 Yas Ustu Personeller");
                worksheet.Cells[1, 1].Value = "Personel No";
                worksheet.Cells[1, 2].Value = "Adı Soyadı";
                worksheet.Cells[1, 3].Value = "E-Posta";
                worksheet.Cells[1, 4].Value = "Yaş";
                worksheet.Cells[1, 5].Value = "Maaş";
                worksheet.Cells[1, 6].Value = "Meslek";
                worksheet.Cells[1, 7].Value = "Departman";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.PersonelNo;
                    worksheet.Cells[row, 2].Value = item.PersonelAdSoyad;
                    worksheet.Cells[row, 3].Value = item.Email;
                    worksheet.Cells[row, 4].Value = item.Yas;
                    worksheet.Cells[row, 5].Value = item.Maas;
                    worksheet.Cells[row, 6].Value = item.Meslek;
                    worksheet.Cells[row, 7].Value = item.Departman?.DepartmanAdi;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "YasRaporu.xlsx");
            }
        }

        public IActionResult ExportYasRaporPdf()
        {
            var list = _dbContext.Personels.Include(x => x.Departman)
                                            .Where(p => p.Yas >= 30)
                                            .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 16, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("30 Yaş ve Üstü Personeller Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 25f, 25f, 10f, 20f, 20f });

                table.AddCell(new PdfPCell(new Phrase("Ad Soyad", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("E-Posta", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Yaş", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Meslek", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Departman", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list)
                {
                    table.AddCell(new Phrase(item.PersonelAdSoyad, normalFont));
                    table.AddCell(new Phrase(item.Email, normalFont));
                    table.AddCell(new Phrase(item.Yas.ToString(), normalFont));
                    table.AddCell(new Phrase(item.Meslek, normalFont));
                    table.AddCell(new Phrase(item.Departman?.DepartmanAdi ?? "", normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "YasRaporu.pdf");
            }
        }

        // ==========================================
        // TAMAMLANAN GÖREVLER
        // ==========================================
        public IActionResult TamamlananGorevler()
        {
            var model = new RaporModel
            {
                GorevListesi = _dbContext.Gorevs.Include(x => x.Personel)
                                          .Include(x => x.GorevTipi)
                                          .Where(gorev => gorev.TamamlandiMi == true)
                                          .ToList()
            };
            return View(model);
        }

        public IActionResult ExportTamamlananExcel()
        {
            var list = _dbContext.Gorevs.Include(x => x.Personel)
                                        .Include(x => x.GorevTipi)
                                        .Where(g => g.TamamlandiMi == true)
                                        .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tamamlanan Gorevler");
                worksheet.Cells[1, 1].Value = "Görev ID";
                worksheet.Cells[1, 2].Value = "Görev Tipi";
                worksheet.Cells[1, 3].Value = "Sorumlu Personel";
                worksheet.Cells[1, 4].Value = "Son Tarih";
                worksheet.Cells[1, 5].Value = "Durum";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGreen);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.GorevId;
                    worksheet.Cells[row, 2].Value = item.GorevTipi?.Ad;
                    worksheet.Cells[row, 3].Value = item.Personel?.PersonelAdSoyad;
                    worksheet.Cells[row, 4].Value = item.SonTarih?.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 5].Value = "Tamamlandı";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TamamlananGorevler.xlsx");
            }
        }

        public IActionResult ExportTamamlananPdf()
        {
            var list = _dbContext.Gorevs.Include(x => x.Personel)
                                        .Include(x => x.GorevTipi)
                                        .Where(g => g.TamamlandiMi == true)
                                        .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 16, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Tamamlanan Görevler Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 15f, 35f, 35f, 15f });

                table.AddCell(new PdfPCell(new Phrase("Görev ID", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Görev Tipi", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Sorumlu Personel", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Son Tarih", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list)
                {
                    table.AddCell(new Phrase(item.GorevId.ToString(), normalFont));
                    table.AddCell(new Phrase(item.GorevTipi?.Ad ?? "", normalFont));
                    table.AddCell(new Phrase(item.Personel?.PersonelAdSoyad ?? "", normalFont));
                    table.AddCell(new Phrase(item.SonTarih?.ToString("dd.MM.yyyy") ?? "-", normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "TamamlananGorevler.pdf");
            }
        }

        // ==========================================
        // BEKLEYEN GÖREVLER
        // ==========================================
        public IActionResult BekleyenGorevler()
        {
            var model = new RaporModel
            {
                GorevListesi = _dbContext.Gorevs.Include(x => x.Personel)
                                          .Include(x => x.GorevTipi)
                                          .Where(gorev => gorev.TamamlandiMi == false)
                                          .ToList()
            };
            return View(model);
        }

        public IActionResult ExportBekleyenExcel()
        {
            var list = _dbContext.Gorevs.Include(x => x.Personel)
                                        .Include(x => x.GorevTipi)
                                        .Where(g => g.TamamlandiMi == false)
                                        .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Bekleyen Gorevler");
                worksheet.Cells[1, 1].Value = "Görev ID";
                worksheet.Cells[1, 2].Value = "Görev Tipi";
                worksheet.Cells[1, 3].Value = "Sorumlu Personel";
                worksheet.Cells[1, 4].Value = "Son Tarih";
                worksheet.Cells[1, 5].Value = "Durum";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.GorevId;
                    worksheet.Cells[row, 2].Value = item.GorevTipi?.Ad;
                    worksheet.Cells[row, 3].Value = item.Personel?.PersonelAdSoyad;
                    worksheet.Cells[row, 4].Value = item.SonTarih?.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 5].Value = "Bekliyor";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BekleyenGorevler.xlsx");
            }
        }

        public IActionResult ExportBekleyenPdf()
        {
            var list = _dbContext.Gorevs.Include(x => x.Personel)
                                        .Include(x => x.GorevTipi)
                                        .Where(g => g.TamamlandiMi == false)
                                        .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 16, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Bekleyen Görevler Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 15f, 35f, 35f, 15f });

                table.AddCell(new PdfPCell(new Phrase("Görev ID", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Görev Tipi", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Sorumlu Personel", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Son Tarih", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list)
                {
                    table.AddCell(new Phrase(item.GorevId.ToString(), normalFont));
                    table.AddCell(new Phrase(item.GorevTipi?.Ad ?? "", normalFont));
                    table.AddCell(new Phrase(item.Personel?.PersonelAdSoyad ?? "", normalFont));
                    table.AddCell(new Phrase(item.SonTarih?.ToString("dd.MM.yyyy") ?? "-", normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "BekleyenGorevler.pdf");
            }
        }

        // ==========================================
        // TÜM GÖREVLER
        // ==========================================
        public IActionResult GorevRaporu()
        {
            var model = new RaporModel
            {
                GorevListesi = _dbContext.Gorevs.Include(x => x.Personel)
                                          .Include(x => x.GorevTipi)
                                          .ToList()
            };
            return View(model);
        }

        public IActionResult ExportGorevExcel()
        {
            var list = _dbContext.Gorevs.Include(x => x.Personel)
                                        .Include(x => x.GorevTipi)
                                        .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tum Gorevler");
                worksheet.Cells[1, 1].Value = "Görev ID";
                worksheet.Cells[1, 2].Value = "Görev Tipi";
                worksheet.Cells[1, 3].Value = "Sorumlu Personel";
                worksheet.Cells[1, 4].Value = "Son Tarih";
                worksheet.Cells[1, 5].Value = "Durum";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateGray);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.GorevId;
                    worksheet.Cells[row, 2].Value = item.GorevTipi?.Ad;
                    worksheet.Cells[row, 3].Value = item.Personel?.PersonelAdSoyad;
                    worksheet.Cells[row, 4].Value = item.SonTarih?.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 5].Value = item.TamamlandiMi ? "Tamamlandı" : "Bekliyor";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TumGorevler.xlsx");
            }
        }

        public IActionResult ExportGorevPdf()
        {
            var list = _dbContext.Gorevs.Include(x => x.Personel)
                                        .Include(x => x.GorevTipi)
                                        .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 16, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Tüm Görevler Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 30f, 30f, 15f, 15f });

                table.AddCell(new PdfPCell(new Phrase("Görev ID", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Görev Tipi", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Sorumlu Personel", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Son Tarih", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Durum", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list)
                {
                    table.AddCell(new Phrase(item.GorevId.ToString(), normalFont));
                    table.AddCell(new Phrase(item.GorevTipi?.Ad ?? "", normalFont));
                    table.AddCell(new Phrase(item.Personel?.PersonelAdSoyad ?? "", normalFont));
                    table.AddCell(new Phrase(item.SonTarih?.ToString("dd.MM.yyyy") ?? "-", normalFont));
                    table.AddCell(new Phrase(item.TamamlandiMi ? "Tamamlandı" : "Bekliyor", normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "TumGorevler.pdf");
            }
        }

        // ==========================================
        // DEPARTMAN RAPORU
        // ==========================================
        public IActionResult DepartmanRaporu()
        {
            var model = new RaporModel
            {
                DepartmanListesi = _dbContext.Departmans.ToList()
            };
            return View(model);
        }

        public IActionResult ExportDepartmanExcel()
        {
            var list = _dbContext.Departmans.ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Departmanlar");
                worksheet.Cells[1, 1].Value = "Departman No";
                worksheet.Cells[1, 2].Value = "Departman Adı";
                worksheet.Cells[1, 3].Value = "Açıklama";
                worksheet.Cells[1, 4].Value = "Personel Sayısı";

                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.DepartmanNo;
                    worksheet.Cells[row, 2].Value = item.DepartmanAdi;
                    worksheet.Cells[row, 3].Value = item.Aciklama;
                    worksheet.Cells[row, 4].Value = item.CalisanSayisi;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DepartmanRaporu.xlsx");
            }
        }

        public IActionResult ExportDepartmanPdf()
        {
            var list = _dbContext.Departmans.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 16, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Departman Personel Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 30f, 50f, 20f });

                table.AddCell(new PdfPCell(new Phrase("Departman Adı", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Açıklama", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Personel Sayısı", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list)
                {
                    table.AddCell(new Phrase(item.DepartmanAdi ?? "", normalFont));
                    table.AddCell(new Phrase(item.Aciklama ?? "", normalFont));
                    table.AddCell(new Phrase(item.CalisanSayisi.ToString(), normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "DepartmanRaporu.pdf");
            }
        }
    }
}
