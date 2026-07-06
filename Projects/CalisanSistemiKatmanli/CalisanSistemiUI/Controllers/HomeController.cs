using CalisanSistemi.Data.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CalisanSistemiKatmanlı.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewBag.TotalPersonel = _dbContext.Personels.Count();
            ViewBag.TotalDepartman = _dbContext.Departmans.Count();
            ViewBag.TotalGorev = _dbContext.Gorevs.Count();
            ViewBag.CompletedGorev = _dbContext.Gorevs.Count(x => x.TamamlandiMi);
            ViewBag.PendingGorev = _dbContext.Gorevs.Count(x => !x.TamamlandiMi);
            
            // Get recent tasks
            var recentTasks = _dbContext.Gorevs
                .OrderByDescending(g => g.GorevId)
                .Take(5)
                .Select(g => new {
                    GorevTipi = g.GorevTipi.Ad,
                    Personel = g.Personel.PersonelAdSoyad,
                    g.TamamlandiMi,
                    SonTarih = g.SonTarih
                })
                .ToList();

            ViewBag.RecentTasks = recentTasks;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
