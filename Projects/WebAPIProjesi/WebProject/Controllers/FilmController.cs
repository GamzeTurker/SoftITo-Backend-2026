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
    public class FilmController : Controller
    {
        private readonly HttpClient _client;
        public FilmController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new System.Uri("http://localhost:5159/api/Film/");
        }

        public async Task<IActionResult> Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            
            List<Film> films = new List<Film>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<Film>>("GetFilm");
                if (response != null) films = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                films = films.Where(f => f.FilmAd.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                      || f.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(films);
        }

        public async Task<IActionResult> EY(int id = 0)
        {
            if (id == 0)
            {
                return View(new Film());
            }
            
            Film? film = null;
            try
            {
                var response = await _client.GetFromJsonAsync<List<Film>>("GetFilm");
                film = response?.FirstOrDefault(f => f.FilmId == id);
            }
            catch { }

            return View(film ?? new Film());
        }

        [HttpPost]
        public async Task<IActionResult> EY(Film film)
        {
            try
            {
                if (film.FilmId == 0)
                {
                    await _client.PostAsJsonAsync("AddFilm", film);
                }
                else
                {
                    await _client.PutAsJsonAsync($"UpdateFilm/{film.FilmId}", film);
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Film? film = null;
            try
            {
                var response = await _client.GetFromJsonAsync<List<Film>>("GetFilm");
                film = response?.FirstOrDefault(f => f.FilmId == id);
            }
            catch { }

            return View(film ?? new Film());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Film film)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"DeleteFilm/{film.FilmId}");
                request.Content = JsonContent.Create(film);
                await _client.SendAsync(request);
            }
            catch { }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportExcel(string searchText = "")
        {
            List<Film> films = new List<Film>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<Film>>("GetFilm");
                if (response != null) films = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                films = films.Where(f => f.FilmAd.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                      || f.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var excelBytes = ExportHelper.ExportFilmsToExcel(films);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Filmler.xlsx");
        }

        public async Task<IActionResult> ExportPdf(string searchText = "")
        {
            List<Film> films = new List<Film>();
            try
            {
                var response = await _client.GetFromJsonAsync<List<Film>>("GetFilm");
                if (response != null) films = response;
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
            {
                films = films.Where(f => f.FilmAd.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) 
                                      || f.Tur.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var pdfBytes = ExportHelper.ExportFilmsToPdf(films);
            return File(pdfBytes, "application/pdf", "Filmler.pdf");
        }
    }
}
