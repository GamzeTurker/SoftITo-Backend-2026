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
    public class UyeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public UyeController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5298/api/";
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}uye?search={search}");
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<Uye>>();
                return View(list);
            }
            return View(new List<Uye>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Uye model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}uye", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}uye/{id}");
            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<Uye>();
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Uye model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}uye/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}uye/{id}");
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // EXPORT EXCEL
        // ========================================================
        public async Task<IActionResult> ExportExcel(string? search)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}uye?search={search}");
            var list = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<List<Uye>>() 
                : new List<Uye>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Üyeler");
                worksheet.Cells[1, 1].Value = "Üye ID";
                worksheet.Cells[1, 2].Value = "Adı Soyadı";
                worksheet.Cells[1, 3].Value = "E-Posta";
                worksheet.Cells[1, 4].Value = "Telefon";
                worksheet.Cells[1, 5].Value = "Kayıt Tarihi";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkRed);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var item in list ?? new List<Uye>())
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.AdSoyad;
                    worksheet.Cells[row, 3].Value = item.Eposta;
                    worksheet.Cells[row, 4].Value = item.Telefon;
                    worksheet.Cells[row, 5].Value = item.KayitTarihi.ToString("dd.MM.yyyy");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UyelerListesi.xlsx");
            }
        }

        // ========================================================
        // EXPORT PDF
        // ========================================================
        public async Task<IActionResult> ExportPdf(string? search)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}uye?search={search}");
            var list = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<List<Uye>>() 
                : new List<Uye>();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                BaseFont bf = BaseFont.CreateFont("Helvetica", "Cp1254", BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(bf, 18, Font.BOLD);
                Font boldFont = new Font(bf, 10, Font.BOLD);
                Font normalFont = new Font(bf, 10, Font.NORMAL);

                Paragraph title = new Paragraph("Üyeler Raporu\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 35f, 35f, 20f });

                table.AddCell(new PdfPCell(new Phrase("ID", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Adı Soyadı", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("E-Posta", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Telefon", boldFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var item in list ?? new List<Uye>())
                {
                    table.AddCell(new Phrase(item.Id.ToString(), normalFont));
                    table.AddCell(new Phrase(item.AdSoyad, normalFont));
                    table.AddCell(new Phrase(item.Eposta, normalFont));
                    table.AddCell(new Phrase(item.Telefon, normalFont));
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "UyelerRaporu.pdf");
            }
        }
    }
}
