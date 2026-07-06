using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // 1. KATEGORİLERİ LİSTELEME
        public IActionResult Index()
        {
            _logger.LogInformation("Kategori listeleme sayfası çağrıldı.");
            IEnumerable<Category> categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        // 2. YENİ KATEGORİ EKLEME (GET)
        public IActionResult Create()
        {
            _logger.LogInformation("Yeni kategori ekleme formu açıldı.");
            return View();
        }

        // 3. YENİ KATEGORİ EKLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();

                _logger.LogInformation("Yeni kategori başarıyla oluşturuldu. Kategori Adı: {Name}", category.Name);
                TempData["success"] = "Kategori başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Kategori oluşturma işlemi geçersiz model nedeniyle iptal edildi.");
            return View(category);
        }

        // 4. KATEGORİ DÜZENLEME (GET)
        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Kategori düzenleme formu geçersiz Id ile çağrıldı. Id: {Id}", id);
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                _logger.LogWarning("Düzenlenmek istenen kategori bulunamadı. Aranan Id: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Kategori düzenleme formu açıldı. Düzenlenen Kategori: {Name} (Id: {Id})", category.Name, id);
            return View(category);
        }

        // 5. KATEGORİ DÜZENLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();

                _logger.LogInformation("Kategori başarıyla güncellendi. Güncellenen Kategori: {Name} (Id: {Id})", category.Name, category.Id);
                TempData["success"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Kategori güncelleme işlemi geçersiz model nedeniyle iptal edildi. Kategori Id: {Id}", category.Id);
            return View(category);
        }

        // 6. KATEGORİ SİLME EKRANI (GET - Silme Onay Sayfası Açar)
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Kategori silme onay formu geçersiz Id ile çağrıldı. Id: {Id}", id);
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                _logger.LogWarning("Silinmek istenen kategori bulunamadı. Aranan Id: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Kategori silme onay sayfası açıldı. Silinmek istenen: {Name} (Id: {Id})", category.Name, id);
            return View(category);
        }

        // 7. KATEGORİ SİLME İŞLEMİ (POST - Silme Onaylandıktan Sonra Çalışır)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                _logger.LogWarning("Silinme aşamasında olan kategori veritabanında bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();

            _logger.LogInformation("Kategori veritabanından başarıyla silindi. Silinen Kategori: {Name} (Id: {Id})", category.Name, id);

            TempData["success"] = "Kategori başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // 8. KATEGORİ DETAY/BAĞLI KURSLAR LİSTESİ
        public IActionResult Details(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Detayı açılmak istenen Kategori geçersiz Id ile çağrıldı. Id: {Id}", id);
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                _logger.LogWarning("Detayı açılmak istenen Kategori veritabanında bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            // Bu kategoriye bağlı tüm kursları veritabanından çekiyoruz
            var courses = _unitOfWork.Course.GetAll(c => c.CategoryId == id).ToList();

            _logger.LogInformation("Kategori Detay (Kurslar) sayfası görüntülendi. Kategori: {Name} (Toplam Kurs: {Count})", category.Name, courses.Count);

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = category.Id;

            return View(courses); // View'a bağlı kursların listesini gönderiyoruz
        }
    }
}