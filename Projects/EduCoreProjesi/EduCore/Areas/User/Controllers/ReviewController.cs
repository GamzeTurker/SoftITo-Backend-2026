using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // Added for IMemoryCache
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EduCore.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache; // Injected IMemoryCache

        public ReviewController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        private int GetLoggedInUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return Convert.ToInt32(claim.Value);
        }

        // 1. DEĞERLENDİRİLMELERİM LİSTESİ (Index) - Caching uygulandı
        public IActionResult Index()
        {
            int studentId = GetLoggedInUserId();
            string reviewsCacheKey = $"student_reviews_{studentId}";

            if (!_cache.TryGetValue(reviewsCacheKey, out List<Review> reviews))
            {
                reviews = _unitOfWork.Review.GetAll(r => r.StudentId == studentId, includeProperties: "Course").ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)) // 5 dakika sonra silinir
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2)); // 2 dakika boyunca istek gelmezse silinir

                _cache.Set(reviewsCacheKey, reviews, cacheOptions);
            }

            return View(reviews);
        }

        // 2. YORUM VE PUAN EKLEME METODU
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int courseId, int rating, string comment)
        {
            int studentId = GetLoggedInUserId();

            if (rating < 1 || rating > 5)
            {
                TempData["error"] = "Lütfen 1 ile 5 arasında bir puan seçin.";
                return RedirectToAction("Details", "Student", new { area = "", id = courseId });
            }

            // Öğrencinin bu kursa daha önce yorum yapıp yapmadığını sorguluyoruz
            var existingReview = _unitOfWork.Review.GetFirstOrDefault(r =>
                r.StudentId == studentId && r.CourseId == courseId);

            if (existingReview != null)
            {
                // Varsa eskisini güncelle
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.UtcNow;
                _unitOfWork.Review.Update(existingReview);
                TempData["success"] = "Değerlendirmeniz başarıyla güncellendi!";
            }
            else
            {
                // Yoksa yenisini oluştur
                Review review = new()
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.Review.Add(review);
                TempData["success"] = "Değerlendirmeniz başarıyla kaydedildi!";
            }

            _unitOfWork.Save();

            // İlgili cache alanlarını temizliyoruz (Invalidation)
            _cache.Remove($"student_reviews_{studentId}");
            _cache.Remove($"course_reviews_{courseId}"); // HomeController'da kullanılan kurs detay yorumları cache'i

            return RedirectToAction("Details", "Home", new { area = "User", id = courseId });
        }

        // 3. YORUM SİLME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            int studentId = GetLoggedInUserId();
            var review = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == id && r.StudentId == studentId);
            
            if (review != null)
            {
                int courseId = review.CourseId;
                _unitOfWork.Review.Remove(review);
                _unitOfWork.Save();
                
                // İlgili cache alanlarını temizliyoruz (Invalidation)
                _cache.Remove($"student_reviews_{studentId}");
                _cache.Remove($"course_reviews_{courseId}"); // HomeController'da kullanılan kurs detay yorumları cache'i

                TempData["success"] = "Değerlendirmeniz silindi.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}