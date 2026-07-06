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

namespace EduCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LessonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<LessonController> _logger; // Logger tanımı

        // Constructor'a Logger enjekte edildi
        public LessonController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ILogger<LessonController> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        // DERS LİSTESİ
        public IActionResult Index(string search)
        {
            _logger.LogInformation("Ders listeleme sayfası çağrıldı. Arama kelimesi: {Search}", search);

            var lessonlist = _unitOfWork.Lesson.GetAll(includeProperties: "Course");

            if (!string.IsNullOrEmpty(search))
            {
                lessonlist = lessonlist.Where(l =>
                    l.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            ViewBag.Search = search;
            return View(lessonlist);
        }

        // DERS EKLEME/DÜZENLEME (GET)
        public IActionResult Crup(int? id = 0)
        {
            LessonVM lessonVM = new()
            {
                Lesson = new(),
                CourseList = _unitOfWork.Course.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                })
            };

            if (id == null || id <= 0)
            {
                _logger.LogInformation("Yeni Ders ekleme formu açıldı.");
                return View(lessonVM);
            }

            lessonVM.Lesson = _unitOfWork.Lesson.GetFirstOrDefault(x => x.Id == id);

            if (lessonVM.Lesson == null)
            {
                _logger.LogWarning("Düzenlenmek istenen Ders bulunamadı. Aranan Id: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Ders düzenleme formu açıldı. Düzenlenen Ders: {Title} (Id: {Id})", lessonVM.Lesson.Title, id);
            return View(lessonVM);
        }

        // DERS EKLEME/DÜZENLEME (POST)
        [HttpPost]
        public IActionResult Crup(LessonVM lessonVM, IFormFile file)
        {
            ModelState.Remove("Lesson.Course");
            ModelState.Remove("file");

            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploadRoot = Path.Combine(wwwRootPath, @"videos");
                var extension = Path.GetExtension(file.FileName);

                if (lessonVM.Lesson.VideoUrl != null)
                {
                    var oldVidPath = Path.Combine(wwwRootPath, lessonVM.Lesson.VideoUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldVidPath))
                    {
                        System.IO.File.Delete(oldVidPath);
                        _logger.LogInformation("Dersin eski videosu sunucudan silindi. Yol: {Path}", oldVidPath);
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

                lessonVM.Lesson.VideoUrl = @"\videos\" + fileName + extension;
                _logger.LogInformation("Derse yeni video dosyası yüklendi. Dosya adı: {FileName}", fileName + extension);
            }

            if (ModelState.IsValid)
            {
                if (lessonVM.Lesson.Id <= 0)
                {
                    _unitOfWork.Lesson.Add(lessonVM.Lesson);
                    _logger.LogInformation("Veritabanına yeni Ders eklendi. Ders Adı: {Title}", lessonVM.Lesson.Title);
                }
                else
                {
                    _unitOfWork.Lesson.Update(lessonVM.Lesson);
                    _logger.LogInformation("Ders bilgileri güncellendi. Ders: {Title} (Id: {Id})", lessonVM.Lesson.Title, lessonVM.Lesson.Id);
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Ders kaydetme işlemi geçersiz model nedeniyle iptal edildi.");

            lessonVM.CourseList = _unitOfWork.Course.GetAll().Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString()
            });

            return View(lessonVM);
        }

        // DERS SİLME (GET)
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            LessonVM lessonVM = new()
            {
                Lesson = _unitOfWork.Lesson.GetFirstOrDefault(x => x.Id == id)
            };

            if (lessonVM.Lesson == null)
            {
                _logger.LogWarning("Silinmek istenen Ders bulunamadı. Aranan Id: {Id}", id);
                return NotFound();
            }

            return View(lessonVM);
        }

        // DERS SİLME (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var lesson = _unitOfWork.Lesson.GetFirstOrDefault(x => x.Id == id);

            if (lesson == null)
            {
                _logger.LogWarning("Silinme aşamasında olan Ders bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            if (lesson.VideoUrl != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                var vidPath = Path.Combine(wwwRootPath, lesson.VideoUrl.TrimStart('\\'));
                if (System.IO.File.Exists(vidPath))
                {
                    System.IO.File.Delete(vidPath);
                    _logger.LogInformation("Ders silinirken video dosyası da sunucudan silindi. Yol: {Path}", vidPath);
                }
            }

            _unitOfWork.Lesson.Remove(lesson);
            _unitOfWork.Save();

            _logger.LogInformation("Ders veritabanından silindi. Silinen Ders: {Title} (Id: {Id})", lesson.Title, id);

            TempData["success"] = "Ders başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // DERS İZLEME PREVIEW
        public IActionResult Watch(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var lesson = _unitOfWork.Lesson.GetFirstOrDefault(x => x.Id == id);
            if (lesson == null)
            {
                _logger.LogWarning("İzlenmek istenen Ders bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            lesson.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == lesson.CourseId);

            var otherLessons = _unitOfWork.Lesson.GetAll(l => l.CourseId == lesson.CourseId)
                                                  .OrderBy(l => l.SortOrder)
                                                  .ToList();

            _logger.LogInformation("Ders izleme (Watch) ekranı açıldı. İzlenen Ders: {Title} (Id: {Id})", lesson.Title, id);

            ViewBag.OtherLessons = otherLessons;

            return View(lesson);
        }
    }
}