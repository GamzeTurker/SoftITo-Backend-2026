using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using projectcodefirst.Models;
using SiparisSistemi.Models;

namespace SiparisSistemi.Controllers
{
    public class MusteriController : Controller
    {
        public readonly ApplicationDbContext dbContext;
        public MusteriController
            (ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(string arama)
        {
            var musteriler = dbContext.Musterilers.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                musteriler = musteriler.Where(x =>
                    x.MusteriAdSoyad.Contains(arama));
            }

            return View(musteriler.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
          
            return View();
        }
        [HttpPost]
        public IActionResult Create(Musteriler musteriler)
        {
            dbContext.Musterilers.Add(musteriler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var result = dbContext.Musterilers.Find(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Musteriler musteriler)
        {
            dbContext.Update(musteriler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {

            var result = dbContext.Musterilers.Find(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Delete(Musteriler musteriler)
        {
           
            dbContext.Remove(musteriler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
