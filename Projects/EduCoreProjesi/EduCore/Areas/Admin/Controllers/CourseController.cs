using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using EduCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging; // Logger için eklendi
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<CourseController> _logger; // Logger tanımı

        // Constructor'a Logger enjekte edildi
        public CourseController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ILogger<CourseController> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        // KURS LİSTESİ
        public IActionResult Index(string search)
        {
            _logger.LogInformation("Kurs listeleme sayfası çağrıldı. Arama kelimesi: {Search}", search);

            var courselist = _unitOfWork.Course.GetAll(includeProperties: "Category");

            if (!string.IsNullOrEmpty(search))
            {
                courselist = courselist.Where(c =>
                    c.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (c.Description != null && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            ViewBag.Search = search;
            return View(courselist);
        }

        // KURS EKLEME/DÜZENLEME (GET)
        public IActionResult Crup(int? id = 0)
        {
            CourseVM courseVM = new()
            {
                Course = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (id == null || id <= 0)
            {
                _logger.LogInformation("Yeni Kurs ekleme formu açıldı.");
                return View(courseVM);
            }

            courseVM.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == id);

            if (courseVM.Course == null)
            {
                _logger.LogWarning("Düzenlenmek istenen Kurs bulunamadı. Aranan Id: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Kurs düzenleme formu açıldı. Düzenlenen Kurs: {Title} (Id: {Id})", courseVM.Course.Title, id);
            return View(courseVM);
        }

        // KURS EKLEME/DÜZENLEME (POST)
        [HttpPost]
        public IActionResult Crup(CourseVM courseVM, IFormFile file)
        {
            ModelState.Remove("Course.Category");

            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploadRoot = Path.Combine(wwwRootPath, @"img\courses");
                var extension = Path.GetExtension(file.FileName);

                if (courseVM.Course.CoverImageUrl != null)
                {
                    var oldPicPath = Path.Combine(wwwRootPath, courseVM.Course.CoverImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldPicPath))
                    {
                        System.IO.File.Delete(oldPicPath);
                        _logger.LogInformation("Kursun eski görseli sunucudan silindi. Yol: {Path}", oldPicPath);
                    }
                }

                if (!Directory.Exists(uploadRoot))
                {
                    Directory.CreateDirectory(uploadRoot);
                }

                using (var fileStream = new FileStream(Path.Combine(uploadRoot, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                courseVM.Course.CoverImageUrl = @"\img\courses\" + fileName + extension;
                _logger.LogInformation("Kursa yeni kapak görseli yüklendi. Dosya adı: {FileName}", fileName + extension);
            }

            if (ModelState.IsValid)
            {
                if (courseVM.Course.Id <= 0)
                {
                    _unitOfWork.Course.Add(courseVM.Course);
                    _logger.LogInformation("Veritabanına yeni Kurs başarıyla eklendi. Kurs Adı: {Title}", courseVM.Course.Title);
                }
                else
                {
                    _unitOfWork.Course.Update(courseVM.Course);
                    _logger.LogInformation("Kurs bilgileri başarıyla güncellendi. Kurs: {Title} (Id: {Id})", courseVM.Course.Title, courseVM.Course.Id);
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Kurs kaydetme işlemi geçersiz model nedeniyle iptal edildi.");

            courseVM.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(courseVM);
        }

        // KURS SİLME (GET)
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            CourseVM courseVM = new CourseVM()
            {
                Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == id)
            };

            if (courseVM.Course == null)
            {
                _logger.LogWarning("Silinmek istenen Kurs bulunamadı. Aranan Id: {Id}", id);
                return NotFound();
            }

            return View(courseVM);
        }

        // KURS SİLME (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == id);

            if (course == null)
            {
                _logger.LogWarning("Silinme aşamasında olan Kurs veritabanında bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            if (course.CoverImageUrl != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                var picPath = Path.Combine(wwwRootPath, course.CoverImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(picPath))
                {
                    System.IO.File.Delete(picPath);
                    _logger.LogInformation("Kurs silinirken kapak görseli de sunucudan silindi. Yol: {Path}", picPath);
                }
            }

            _unitOfWork.Course.Remove(course);
            _unitOfWork.Save();

            _logger.LogInformation("Kurs başarıyla veritabanından silindi. Silinen Kurs: {Title} (Id: {Id})", course.Title, id);

            TempData["success"] = "Kurs başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // KURS DETAY/DERSLER LİSTESİ
        public IActionResult Details(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == id);
            if (course == null)
            {
                _logger.LogWarning("Detayı açılmak istenen Kurs bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            if (course.CategoryId.HasValue)
            {
                course.Category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == course.CategoryId.Value);
            }

            var lessons = _unitOfWork.Lesson.GetAll(l => l.CourseId == id).OrderBy(l => l.SortOrder).ToList();

            _logger.LogInformation("Kurs Detay (Dersler) sayfası görüntülendi. Kurs: {Title} (Toplam Ders: {Count})", course.Title, lessons.Count);

            ViewBag.CourseTitle = course.Title;
            ViewBag.CourseCategory = course.Category?.Name;
            ViewBag.CategoryId = course.CategoryId;

            return View(lessons);
        }
    }
}