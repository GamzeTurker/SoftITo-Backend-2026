using Microsoft.AspNetCore.Mvc;
using OkulNotSistemi.Models;

namespace OkulNotSistemi.Controllers
{
    public class DerslerController : Controller
    {
        private readonly AppDbContext dbContext;

        public DerslerController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string search)
        {
            var query = dbContext.Derslers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.DersAd.Contains(search));
            }

            return View(query.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Dersler ders)
        {
            dbContext.Derslers.Add(ders);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ders = dbContext.Derslers.Find(id);
            return View(ders);
        }

        [HttpPost]
        public IActionResult Edit(Dersler ders)
        {
            dbContext.Derslers.Update(ders);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ders = dbContext.Derslers.Find(id);
            return View(ders);
        }

        [HttpPost]
        public IActionResult Delete(Dersler ders)
        {
            dbContext.Derslers.Remove(ders);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}