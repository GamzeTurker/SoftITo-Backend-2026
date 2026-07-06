using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryMVC.Models;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace LibraryMVC.Controllers
{
    [Authorize]
    public class KitapController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public KitapController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5298/api/";
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}kitap?search={search}");
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<Kitap>>();
                return View(list);
            }
            return View(new List<Kitap>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var yazarResponse = await _httpClient.GetAsync($"{_apiBaseUrl}yazar");
            if (yazarResponse.IsSuccessStatusCode)
            {
                ViewBag.Yazarlar = await yazarResponse.Content.ReadFromJsonAsync<List<Yazar>>();
            }
            else
            {
                ViewBag.Yazarlar = new List<Yazar>();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Kitap model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}kitap", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var yazarResponse = await _httpClient.GetAsync($"{_apiBaseUrl}yazar");
            ViewBag.Yazarlar = yazarResponse.IsSuccessStatusCode 
                ? await yazarResponse.Content.ReadFromJsonAsync<List<Yazar>>() 
                : new List<Yazar>();

            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}kitap/{id}");
            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<Kitap>();
                
                var yazarResponse = await _httpClient.GetAsync($"{_apiBaseUrl}yazar");
                ViewBag.Yazarlar = yazarResponse.IsSuccessStatusCode 
                    ? await yazarResponse.Content.ReadFromJsonAsync<List<Yazar>>() 
                    : new List<Yazar>();

                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Kitap model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}kitap/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var yazarResponse = await _httpClient.GetAsync($"{_apiBaseUrl}yazar");
            ViewBag.Yazarlar = yazarResponse.IsSuccessStatusCode 
                ? await yazarResponse.Content.ReadFromJsonAsync<List<Yazar>>() 
                : new List<Yazar>();

            ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}kitap/{id}");
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // EXPORT EXCEL
        // ========================================================
        public async Task<IActionResult> ExportExcel(string? search)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}kitap?search={search}");
            var list = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<List<Kitap>>() 
                : new List<Kitap>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Kitaplar");
                worksheet.Cells[1, 1].Value = "Kitap ID";
                worksheet.Cells[1, 2].Value = "Kitap Başlığı";
                worksheet.Cells[1, 3].Value = "Yazar";
                worksheet.Cells[1, 4].Value = "ISBN";
                worksheet.Cells[1, 5].Value = "Basım Yılı";
                worksheet.Cells[1, 6].Value = "Sayfa Sayısı";

                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOliveGreen);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list ?? new List<Kitap>())
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.Baslik;
                    worksheet.Cells[row, 3].Value = item.YazarAdSoyad;
                    worksheet.Cells[row, 4].Value = item.ISBN;
                    worksheet.Cells[row, 5].Value = item.BasimYili;
                    worksheet.Cells[row, 6].Value = item.SayfaSayisi;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "KitaplarListesi.xlsx");
            }
        }

        // ========================================================
        // EXPORT PDF
        // ========================================================
        public async Task<IActionResult> ExportPdf(string? search)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}kitap?search={search}");
            var list = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<List<Kitap>>() 
                : new List<Kitap>();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 18, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Kitaplar Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 8f, 32f, 30f, 18f, 12f });

                table.AddCell(new PdfPCell(new Phrase("ID", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Kitap Başlığı", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Yazar", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("ISBN", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Sayfa", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list ?? new List<Kitap>())
                {
                    table.AddCell(new Phrase(item.Id.ToString(), normalFont));
                    table.AddCell(new Phrase(item.Baslik, normalFont));
                    table.AddCell(new Phrase(item.YazarAdSoyad ?? "", normalFont));
                    table.AddCell(new Phrase(item.ISBN, normalFont));
                    table.AddCell(new Phrase(item.SayfaSayisi.ToString(), normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "KitaplarRaporu.pdf");
            }
        }
    }
}
