using Microsoft.AspNetCore.Mvc;
using OkulNotSistemi.Models;
using System.Diagnostics;

namespace OkulNotSistemi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
