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
    public class CizgiFilmController : Controller
    {
        private readonly HttpClient _client;
        public CizgiFilmController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new System.Uri("http://localhost:5159/api/CizgiFilm/");
        }

        public async Task<IActionResult> Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            
            List<CizgiFilm> cizgiFilmler = new List<CizgiFilm>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<CizgiFilm>>("GetCFilm");
                if (response != null) cizgiFilmler = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                cizgiFilmler = cizgiFilmler.Where(c => c.Ad.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                                    || c.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(cizgiFilmler);
        }

        public async Task<IActionResult> EY(int id = 0)
        {
            if (id == 0)
            {
                return View(new CizgiFilm());
            }
            
            CizgiFilm? cizgiFilm = null;
            try
            {
                var response = await _client.GetFromJsonAsync<List<CizgiFilm>>("GetCFilm");
                cizgiFilm = response?.FirstOrDefault(c => c.Id == id);
            }
            catch { }

            return View(cizgiFilm ?? new CizgiFilm());
        }

        [HttpPost]
        public async Task<IActionResult> EY(CizgiFilm cizgiFilm)
        {
            try
            {
                if (cizgiFilm.Id == 0)
                {
                    await _client.PostAsJsonAsync("AddCFilm", cizgiFilm);
                }
                else
                {
                    await _client.PutAsJsonAsync($"UpdateCFilm/{cizgiFilm.Id}", cizgiFilm);
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            CizgiFilm? cizgiFilm = null;
            try
            {
                var response = await _client.GetFromJsonAsync<List<CizgiFilm>>("GetCFilm");
                cizgiFilm = response?.FirstOrDefault(c => c.Id == id);
            }
            catch { }

            return View(cizgiFilm ?? new CizgiFilm());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CizgiFilm cizgiFilm)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"DeleteCFilm/{cizgiFilm.Id}");
                request.Content = JsonContent.Create(cizgiFilm);
                await _client.SendAsync(request);
            }
            catch { }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportExcel(string searchText = "")
        {
            List<CizgiFilm> cizgiFilmler = new List<CizgiFilm>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<CizgiFilm>>("GetCFilm");
                if (response != null) cizgiFilmler = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                cizgiFilmler = cizgiFilmler.Where(c => c.Ad.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                                    || c.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var excelBytes = ExportHelper.ExportCizgiFilmlerToExcel(cizgiFilmler);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CizgiFilmler.xlsx");
        }

        public async Task<IActionResult> ExportPdf(string searchText = "")
        {
            List<CizgiFilm> cizgiFilmler = new List<CizgiFilm>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<CizgiFilm>>("GetCFilm");
                if (response != null) cizgiFilmler = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                cizgiFilmler = cizgiFilmler.Where(c => c.Ad.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                                    || c.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var pdfBytes = ExportHelper.ExportCizgiFilmlerToPdf(cizgiFilmler);
            return File(pdfBytes, "application/pdf", "CizgiFilmler.pdf");
        }
    }
}
