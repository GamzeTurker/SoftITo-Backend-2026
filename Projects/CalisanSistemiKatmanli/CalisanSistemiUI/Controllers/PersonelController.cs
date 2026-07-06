using CalisanSistemi.Data.Data;
using CalisanSistemi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CalisanSistemiUI.Controllers
{
    public class PersonelController : Controller
    {
        public readonly AppDbContext dbContext;
        public PersonelController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(string search)
        {
            var result = dbContext.Personels
                .Where(x => string.IsNullOrWhiteSpace(search)
           || x.PersonelAdSoyad.Contains(search)
           || x.Email.Contains(search)
           || x.Meslek.Contains(search)).Include(x=>x.Departman)
             .ToList();
            return View(result);
        }
        

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departman = new SelectList(dbContext.Departmans, "DepartmanNo", "DepartmanAdi");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Personel personel)
        {
            dbContext.Personels.Add(personel);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var personel = dbContext.Personels.Find(id);
            ViewBag.Departman = new SelectList(dbContext.Departmans, "DepartmanNo", "DepartmanAdi");
            return View(personel);
        }
        [HttpPost]
        public IActionResult Edit(Personel personel)
        {
            dbContext.Update(personel);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var personel = dbContext.Personels.Find(id);
            ViewBag.Departman = new SelectList(dbContext.Departmans, "DepartmanNo", "DepartmanAdi");
            return View(personel);
        }
        [HttpPost]
        public IActionResult Delete(Personel personel)
        {
            dbContext.Remove(personel);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
