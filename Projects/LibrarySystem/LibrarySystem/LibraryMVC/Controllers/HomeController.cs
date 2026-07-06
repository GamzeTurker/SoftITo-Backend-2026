using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryMVC.Models;

namespace LibraryMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public HomeController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5298/api/";
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var kitaplarTask = _httpClient.GetFromJsonAsync<List<Kitap>>($"{_apiBaseUrl}kitap");
                var uyelerTask = _httpClient.GetFromJsonAsync<List<Uye>>($"{_apiBaseUrl}uye");
                var yazarlarTask = _httpClient.GetFromJsonAsync<List<Yazar>>($"{_apiBaseUrl}yazar");
                var emanetlerTask = _httpClient.GetFromJsonAsync<List<Emanet>>($"{_apiBaseUrl}emanet");

                await Task.WhenAll(kitaplarTask, uyelerTask, yazarlarTask, emanetlerTask);

                ViewBag.KitapSayisi = kitaplarTask.Result?.Count ?? 0;
                ViewBag.UyeSayisi = uyelerTask.Result?.Count ?? 0;
                ViewBag.YazarSayisi = yazarlarTask.Result?.Count ?? 0;
                ViewBag.EmanetSayisi = emanetlerTask.Result?.Count ?? 0;

                ViewBag.AktifEmanetler = emanetlerTask.Result?.Count(e => e.Durum == "Emanette" || e.Durum == "Gecikmiş") ?? 0;
                ViewBag.SonEmanetler = emanetlerTask.Result?.Take(5).ToList() ?? new List<Emanet>();
            }
            catch (Exception)
            {
                ViewBag.KitapSayisi = 0;
                ViewBag.UyeSayisi = 0;
                ViewBag.YazarSayisi = 0;
                ViewBag.EmanetSayisi = 0;
                ViewBag.AktifEmanetler = 0;
                ViewBag.SonEmanetler = new List<Emanet>();
                ViewBag.Error = "API servis bağlantısı kurulamadı.";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
