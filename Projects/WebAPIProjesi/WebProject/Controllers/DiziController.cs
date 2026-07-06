using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebProject.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DiziController : Controller
    {
        private readonly HttpClient _client;
        public DiziController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new System.Uri("http://localhost:5159/api/Dizi/");
        }

        public async Task<IActionResult> Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            
            List<Dizi> diziler = new List<Dizi>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<Dizi>>("GetDizi");
                if (response != null) diziler = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                diziler = diziler.Where(d => d.DiziAd.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                          || d.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(diziler);
        }

        public async Task<IActionResult> EY(int id = 0)
        {
            if (id == 0)
            {
                return View(new Dizi());
            }
            
            Dizi? dizi = null;
            try
            {
                var response = await _client.GetFromJsonAsync<List<Dizi>>("GetDizi");
                dizi = response?.FirstOrDefault(d => d.DiziId == id);
            }
            catch { }

            return View(dizi ?? new Dizi());
        }

        [HttpPost]
        public async Task<IActionResult> EY(Dizi dizi)
        {
            try
            {
                if (dizi.DiziId == 0)
                {
                    await _client.PostAsJsonAsync("AddDizi", dizi);
                }
                else
                {
                    await _client.PutAsJsonAsync($"UpdateDizi/{dizi.DiziId}", dizi);
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Dizi? dizi = null;
            try
            {
                var response = await _client.GetFromJsonAsync<List<Dizi>>("GetDizi");
                dizi = response?.FirstOrDefault(d => d.DiziId == id);
            }
            catch { }

            return View(dizi ?? new Dizi());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Dizi dizi)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"DeleteDizi/{dizi.DiziId}");
                request.Content = JsonContent.Create(dizi);
                await _client.SendAsync(request);
            }
            catch { }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportExcel(string searchText = "")
        {
            List<Dizi> diziler = new List<Dizi>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<Dizi>>("GetDizi");
                if (response != null) diziler = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                diziler = diziler.Where(d => d.DiziAd.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                          || d.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var excelBytes = ExportHelper.ExportDizilerToExcel(diziler);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Diziler.xlsx");
        }

        public async Task<IActionResult> ExportPdf(string searchText = "")
        {
            List<Dizi> diziler = new List<Dizi>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<Dizi>>("GetDizi");
                if (response != null) diziler = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                diziler = diziler.Where(d => d.DiziAd.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                          || d.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var pdfBytes = ExportHelper.ExportDizilerToPdf(diziler);
            return File(pdfBytes, "application/pdf", "Diziler.pdf");
        }
    }
}
