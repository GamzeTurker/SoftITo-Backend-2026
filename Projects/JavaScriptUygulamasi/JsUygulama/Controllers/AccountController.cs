using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JsUygulama.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JsUygulama.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========================================================
        // ADMIN LOGIN
        // ========================================================

        [HttpGet]
        public IActionResult AdminLogin()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string username, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (admin != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, admin.Name),
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        // ========================================================
        // USER LOGIN
        // ========================================================

        [HttpGet]
        public IActionResult UserLogin()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "User");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        // ========================================================
        // LOGOUT & ACCESS DENIED
        // ========================================================

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
