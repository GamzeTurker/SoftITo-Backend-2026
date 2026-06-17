using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectcodefirst.Models;

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
        public IActionResult Index()
        {

            var urunler = dbContext.Urunlers.AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                urunler = urunler.Where(x =>
                    x.UrunAdi.Contains(arama));
            }

            return View(urunler.ToList());
        }
    }
}
