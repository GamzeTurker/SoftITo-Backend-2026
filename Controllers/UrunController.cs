using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projectcodefirst.Models;
using SiparisSistemi.Models;

namespace SiparisSistemi.Controllers
{
    public class UrunController : Controller
    {
        public readonly ApplicationDbContext dbContext;
        public UrunController
            (ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(string arama)
        {

            var urunler = dbContext.Urunlers.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                urunler = urunler.Where(x =>
                    x.UrunAdi.Contains(arama));
            }

            return View(urunler.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Urunler urunler)
        {
            dbContext.Urunlers.Add(urunler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var result = dbContext.Urunlers.Find(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Urunler urunler)
        {
            dbContext.Update(urunler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {

            var result = dbContext.Urunlers.Find(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Delete(Urunler urunler)
        {

            dbContext.Remove(urunler);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
