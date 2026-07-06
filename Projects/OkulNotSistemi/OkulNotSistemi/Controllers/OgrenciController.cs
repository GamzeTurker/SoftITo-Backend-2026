using Microsoft.AspNetCore.Mvc;
using OkulNotSistemi.Models;

namespace OkulNotSistemi.Controllers
{
    public class OgrencilerController : Controller
    {
        private readonly AppDbContext dbContext;

        public OgrencilerController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string search)
        {
            var query = dbContext.Ogrencilers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.OgrenciAd.Contains(search) ||
                    x.OgrenciSoyad.Contains(search) ||
                    x.Numara.Contains(search)
                );
            }

            var liste = query.ToList();
            return View(liste);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ogrenciler ogrenci)
        {
            dbContext.Ogrencilers.Add(ogrenci);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ogrenci = dbContext.Ogrencilers.Find(id);
            return View(ogrenci);
        }

        [HttpPost]
        public IActionResult Edit(Ogrenciler ogrenci)
        {
            dbContext.Ogrencilers.Update(ogrenci);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ogrenci = dbContext.Ogrencilers.Find(id);
            return View(ogrenci);
        }

        [HttpPost]
        public IActionResult Delete(Ogrenciler ogrenci)
        {
            dbContext.Ogrencilers.Remove(ogrenci);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}