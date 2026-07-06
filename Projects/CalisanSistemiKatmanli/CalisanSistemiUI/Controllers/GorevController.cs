using CalisanSistemi.Data.Data;
using CalisanSistemi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CalisanSistemiUI.Controllers
{
    public class GorevController : Controller
    {
        public readonly AppDbContext dbContext;

        public GorevController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string search)
        {
            var query = dbContext.Gorevs
                .Include(x => x.Personel)
                .Include(x => x.GorevTipi)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Personel.PersonelAdSoyad.Contains(search)
                                      || x.GorevTipi.Ad.Contains(search));
            }

            var result = query.ToList();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Personel = new SelectList(dbContext.Personels, "PersonelNo", "PersonelAdSoyad");
            ViewBag.GorevTipi = new SelectList(dbContext.GorevTipis, "GorevTipId", "Ad");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Gorev gorev)
        {
            if (ModelState.IsValid)
            {
                dbContext.Gorevs.Add(gorev);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Personel = new SelectList(dbContext.Personels, "PersonelNo", "PersonelAdSoyad");
            ViewBag.GorevTipi = new SelectList(dbContext.GorevTipis, "GorevTipId", "Ad");
            return View(gorev);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var result = dbContext.Gorevs.Find(id);
            if (result == null)
            {
                return NotFound();
            }

            ViewBag.Personel = new SelectList(dbContext.Personels, "PersonelNo", "PersonelAdSoyad", result.PersonelNo);
            ViewBag.GorevTipi = new SelectList(dbContext.GorevTipis, "GorevTipId", "Ad", result.GorevTipId);
            return View(result);
        }

        [HttpPost]
        public IActionResult Edit(Gorev gorev)
        {
            if (ModelState.IsValid)
            {
                dbContext.Update(gorev);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Personel = new SelectList(dbContext.Personels, "PersonelNo", "PersonelAdSoyad", gorev.PersonelNo);
            ViewBag.GorevTipi = new SelectList(dbContext.GorevTipis, "GorevTipId", "Ad", gorev.GorevTipId);
            return View(gorev);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var result = dbContext.Gorevs
                .Include(x => x.Personel)
                .Include(x => x.GorevTipi)
                .FirstOrDefault(x => x.GorevId == id);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        [HttpPost]
        public IActionResult Delete(Gorev gorev)
        {
            dbContext.Remove(gorev);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
