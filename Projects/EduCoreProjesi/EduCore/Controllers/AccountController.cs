using EduCore.Models;
using EduCore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EduCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // Rol yönetimi eklendi

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. ROL SEÇİM EKRANI (GET)
        public IActionResult SelectRole()
        {
            return View();
        }

        // 2. GİRİŞ SAYFASI (GET)
        public async Task<IActionResult> Login(string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Student"))
            {
                // Rol seçilmemişse seçim ekranına yönlendir
                return RedirectToAction(nameof(SelectRole));
            }

            // Sistemdeki rolleri kontrol et, yoksa otomatik oluştur
            await EnsureRolesExist();

            ViewBag.Role = role; // Hangi giriş ekranı olduğunu View'a taşıyoruz
            return View();
        }

        // 3. GİRİŞ İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string role)
        {
            ViewBag.Role = role;

            if (ModelState.IsValid)
            {
                // Önce kullanıcının e-posta adresiyle sistemde kayıtlı olup olmadığını bulalım
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "E-posta adresi veya şifre hatalı.");
                    return View(model);
                }

                // Seçilen role sahip olup olmadığını kontrol ediyoruz
                var isUserInRole = await _userManager.IsInRoleAsync(user, role);
                if (!isUserInRole)
                {
                    ModelState.AddModelError("", $"Bu hesapla {role} panelinize giriş izniniz bulunmamaktadır.");
                    return View(model);
                }

                // Giriş işlemini gerçekleştiriyoruz
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (role == "Admin")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("HomePage", "Home", new { area = "User" });
                    }
                }

                ModelState.AddModelError("", "E-posta adresi veya şifre hatalı.");
            }
            return View(model);
        }

        // 4. KAYIT EKRANI (GET)
        public IActionResult Register(string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Student"))
            {
                role = "Student"; // Rol belirtilmemişse varsayılan olarak Öğrenci
            }

            ViewBag.Role = role; // Rolü arayüze taşıyoruz
            return View();
        }

        // 5. KAYIT İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Student"))
            {
                role = "Student";
            }
            ViewBag.Role = role;

            if (ModelState.IsValid)
            {
                await EnsureRolesExist(); // Roller yoksa oluştur

                User user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    ProfileImageUrl = "/img/default-profile.png"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Kullanıcıyı, kayıt esnasında seçtiği role (Admin veya Student) atıyoruz
                    await _userManager.AddToRoleAsync(user, role);

                    TempData["success"] = $"{role} kaydınız başarıyla tamamlandı. Giriş yapabilirsiniz.";
                    return RedirectToAction("Login", "Account", new { role = role });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        // 6. ÇIKIŞ YAPMA
        // 6. ÇIKIŞ YAPMA (Alternatif Temel Yöntem)
        public async Task<IActionResult> LogOut()
        {
            // Identity çerezlerini ve oturumunu doğrudan sıfırlar
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Doğrudan rol seçim ekranına yönlendirir
            return RedirectToAction(nameof(SelectRole));
        }

        // 7. ŞİFREMİ UNUTTUM / E-POSTA KONTROLÜ (GET)
        public IActionResult VerifyEmail()
        {
            return View();
        }

        // 8. ŞİFREMİ UNUTTUM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Bu e-posta adresi ile kayıtlı bir kullanıcı bulunamadı.");
                    return View(model);
                }

                return RedirectToAction("ChangePassword", "Account", new { email = user.Email });
            }
            return View(model);
        }

        // 9. ŞİFRE DEĞİŞTİRME (GET)
        public IActionResult ChangePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(VerifyEmail));
            }
            return View(new ChangePasswordViewModel { Email = email });
        }

        // 10. ŞİFRE DEĞİŞTİRME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Güvenli şifre sıfırlama (Eski şifreyi kaldırıp yenisini atama)
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (result.Succeeded)
                    {
                        TempData["success"] = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
                        return RedirectToAction(nameof(SelectRole));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                }
            }
            return View(model);
        }

        // YARDIMCI METOT: Veritabanında Roller Yoksa Oluşturur
        private async Task EnsureRolesExist()
        {
            string[] roleNames = { "Admin", "Student" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
                }
            }
        }
    }
}