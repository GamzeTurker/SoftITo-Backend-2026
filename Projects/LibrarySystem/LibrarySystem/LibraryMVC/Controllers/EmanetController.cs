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
    public class EmanetController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public EmanetController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5298/api/";
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}emanet?search={search}");
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<Emanet>>();
                return View(list);
            }
            return View(new List<Emanet>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var kitapResponse = await _httpClient.GetAsync($"{_apiBaseUrl}kitap");
            var uyeResponse = await _httpClient.GetAsync($"{_apiBaseUrl}uye");

            ViewBag.Kitaplar = kitapResponse.IsSuccessStatusCode 
                ? await kitapResponse.Content.ReadFromJsonAsync<List<Kitap>>() 
                : new List<Kitap>();

            ViewBag.Uyeler = uyeResponse.IsSuccessStatusCode 
                ? await uyeResponse.Content.ReadFromJsonAsync<List<Uye>>() 
                : new List<Uye>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Emanet model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}emanet", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var kitapResponse = await _httpClient.GetAsync($"{_apiBaseUrl}kitap");
            var uyeResponse = await _httpClient.GetAsync($"{_apiBaseUrl}uye");

            ViewBag.Kitaplar = kitapResponse.IsSuccessStatusCode 
                ? await kitapResponse.Content.ReadFromJsonAsync<List<Kitap>>() 
                : new List<Kitap>();

            ViewBag.Uyeler = uyeResponse.IsSuccessStatusCode 
                ? await uyeResponse.Content.ReadFromJsonAsync<List<Uye>>() 
                : new List<Uye>();

            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}emanet/{id}");
            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<Emanet>();

                var kitapResponse = await _httpClient.GetAsync($"{_apiBaseUrl}kitap");
                var uyeResponse = await _httpClient.GetAsync($"{_apiBaseUrl}uye");

                ViewBag.Kitaplar = kitapResponse.IsSuccessStatusCode 
                    ? await kitapResponse.Content.ReadFromJsonAsync<List<Kitap>>() 
                    : new List<Kitap>();

                ViewBag.Uyeler = uyeResponse.IsSuccessStatusCode 
                    ? await uyeResponse.Content.ReadFromJsonAsync<List<Uye>>() 
                    : new List<Uye>();

                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Emanet model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}emanet/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var kitapResponse = await _httpClient.GetAsync($"{_apiBaseUrl}kitap");
            var uyeResponse = await _httpClient.GetAsync($"{_apiBaseUrl}uye");

            ViewBag.Kitaplar = kitapResponse.IsSuccessStatusCode 
                ? await kitapResponse.Content.ReadFromJsonAsync<List<Kitap>>() 
                : new List<Kitap>();

            ViewBag.Uyeler = uyeResponse.IsSuccessStatusCode 
                ? await uyeResponse.Content.ReadFromJsonAsync<List<Uye>>() 
                : new List<Uye>();

            ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}emanet/{id}");
            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<Emanet>();
                if (model != null)
                {
                    model.TeslimTarihi = DateTime.Now;
                    model.Durum = "Teslim Edildi";
                    await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}emanet/{id}", model);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}emanet/{id}");
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // EXPORT EXCEL
        // ========================================================
        public async Task<IActionResult> ExportExcel(string? search)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}emanet?search={search}");
            var list = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<List<Emanet>>() 
                : new List<Emanet>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Emanetler");
                worksheet.Cells[1, 1].Value = "Emanet ID";
                worksheet.Cells[1, 2].Value = "Kitap Başlığı";
                worksheet.Cells[1, 3].Value = "Üye Adı Soyadı";
                worksheet.Cells[1, 4].Value = "Emanet Tarihi";
                worksheet.Cells[1, 5].Value = "Teslim Tarihi";
                worksheet.Cells[1, 6].Value = "Durumu";

                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkMagenta);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list ?? new List<Emanet>())
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.KitapBaslik;
                    worksheet.Cells[row, 3].Value = item.UyeAdSoyad;
                    worksheet.Cells[row, 4].Value = item.EmanetTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 5].Value = item.TeslimTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cells[row, 6].Value = item.Durum;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmanetListesi.xlsx");
            }
        }

        // ========================================================
        // EXPORT PDF
        // ========================================================
        public async Task<IActionResult> ExportPdf(string? search)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}emanet?search={search}");
            var list = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<List<Emanet>>() 
                : new List<Emanet>();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 18, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Emanet Kitap Hareketleri Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 8f, 25f, 25f, 15f, 15f, 12f });

                table.AddCell(new PdfPCell(new Phrase("ID", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Kitap Başlığı", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Üye Adı Soyadı", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Emanet Tarihi", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Teslim Tarihi", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Durum", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list ?? new List<Emanet>())
                {
                    table.AddCell(new Phrase(item.Id.ToString(), normalFont));
                    table.AddCell(new Phrase(item.KitapBaslik ?? "", normalFont));
                    table.AddCell(new Phrase(item.UyeAdSoyad ?? "", normalFont));
                    table.AddCell(new Phrase(item.EmanetTarihi.ToString("dd.MM.yyyy"), normalFont));
                    table.AddCell(new Phrase(item.TeslimTarihi?.ToString("dd.MM.yyyy") ?? "-", normalFont));
                    table.AddCell(new Phrase(item.Durum, normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "EmanetRaporu.pdf");
            }
        }
    }
}
