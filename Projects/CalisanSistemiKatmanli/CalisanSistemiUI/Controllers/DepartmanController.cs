using CalisanSistemi.Data.Data;
using CalisanSistemi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CalisanSistemiUI.Controllers
{
    public class DepartmanController : Controller
    {
        public readonly AppDbContext dbContext;
        public DepartmanController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(string search)
        {
            var result = dbContext.Departmans.Where(x => string.IsNullOrWhiteSpace(search)
           || x.DepartmanAdi.Contains(search)
           );

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(Departman departman)
        {
            dbContext.Departmans.Add(departman);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var departman = dbContext.Departmans.Find(id);
          
            return View(departman);
        }
        [HttpPost]
        public IActionResult Edit(Departman departman)
        {
            dbContext.Update(departman);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var departman = dbContext.Departmans.Find(id);
            
            return View(departman);
        }
        [HttpPost]
        public IActionResult Delete(Departman departman)
        {
            dbContext.Remove(departman);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
