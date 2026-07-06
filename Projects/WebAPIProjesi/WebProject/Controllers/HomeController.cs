using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebProject.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebProject.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize]
public class HomeController : Controller
{
    private readonly HttpClient _client;
    public HomeController()
    {
        _client = new HttpClient();
    }

    public async Task<IActionResult> Index()
    {
        int filmCount = 0;
        int diziCount = 0;
        int cfilmCount = 0;

        try
        {
            var films = await _client.GetFromJsonAsync<List<Film>>("http://localhost:5159/api/Film/GetFilm");
            if (films != null) filmCount = films.Count;
        }
        catch { }

        try
        {
            var diziler = await _client.GetFromJsonAsync<List<Dizi>>("http://localhost:5159/api/Dizi/GetDizi");
            if (diziler != null) diziCount = diziler.Count;
        }
        catch { }

        try
        {
            var cfilms = await _client.GetFromJsonAsync<List<CizgiFilm>>("http://localhost:5159/api/CizgiFilm/GetCFilm");
            if (cfilms != null) cfilmCount = cfilms.Count;
        }
        catch { }

        ViewBag.FilmCount = filmCount;
        ViewBag.DiziCount = diziCount;
        ViewBag.CFilmCount = cfilmCount;

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
