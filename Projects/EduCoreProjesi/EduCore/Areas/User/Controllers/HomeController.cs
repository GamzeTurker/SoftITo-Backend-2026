using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory; // Added for IMemoryCache
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using QRCoder; // Added for QR Code generation

namespace EduCore.Areas.User.Controllers
{
    [Area("User")]
    [Authorize] // Öğrenci paneline sadece giriş yapmış öğrenciler erişebilir
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache; // Injected IMemoryCache

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        // 1. ÖĞRENCİ ANA SAYFASI / DASHBOARD (HomePage)
        public IActionResult HomePage(int? categoryId)
        {
            // Giriş yapan öğrencinin ID'sini claims üzerinden alıyoruz
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            int studentId = claim != null ? Convert.ToInt32(claim.Value) : 0;

            _logger.LogInformation("Öğrenci ana sayfası (Dashboard) çağrıldı. Öğrenci Id: {StudentId}", studentId);

            // A) Öğrencinin aktif kayıtlarını (Kurslarım listesini) çekiyoruz - Caching uygulandı
            string enrollmentsCacheKey = $"student_enrollments_{studentId}";
            if (!_cache.TryGetValue(enrollmentsCacheKey, out List<Enrollment> enrollments))
            {
                enrollments = _unitOfWork.Enrollment.GetAll(
                    e => e.StudentId == studentId,
                    includeProperties: "Course.Category"
                ).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)) // Tam 5 dakika sonra silinir
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2)); // 2 dakika boyunca istek gelmezse silinir

                _cache.Set(enrollmentsCacheKey, enrollments, cacheOptions);
            }

            // B) İstatistik verilerini hesaplayıp ViewBag'e atıyoruz
            ViewBag.ActiveCoursesCount = enrollments.Count(e => e.ProgressPercent < 100);
            ViewBag.CompletedCoursesCount = enrollments.Count(e => e.ProgressPercent >= 100);
            ViewBag.CertificatesCount = _unitOfWork.Certificate.GetAll(c => c.Enrollment.StudentId == studentId).Count();
            ViewBag.Enrollments = enrollments;

            // C) Kategorileri çekip Dropdown listesine hazırlıyoruz - Caching uygulandı
            string categoriesCacheKey = "all_categories";
            if (!_cache.TryGetValue(categoriesCacheKey, out IEnumerable<Category> categories))
            {
                categories = _unitOfWork.Category.GetAll();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(categoriesCacheKey, categories, cacheOptions);
            }

            var categoryList = categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = categoryId.HasValue && c.Id == categoryId.Value
            }).ToList();

            categoryList.Insert(0, new SelectListItem { Text = "Tüm Kategoriler / Kurslar", Value = "0" });
            ViewBag.CategoryList = categoryList;
            ViewBag.SelectedCategoryId = categoryId ?? 0;

            // D) Öğrencinin henüz kayıt olmadığı (Keşfedeceği) kursları ayıklıyoruz - Caching uygulandı
            string allCoursesCacheKey = "all_courses";
            if (!_cache.TryGetValue(allCoursesCacheKey, out List<Course> allCourses))
            {
                allCourses = _unitOfWork.Course.GetAll(includeProperties: "Category").ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(allCoursesCacheKey, allCourses, cacheOptions);
            }

            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
            var availableCourses = allCourses.Where(c => !enrolledCourseIds.Contains(c.Id)).ToList();

            // Eğer kategori filtresi seçilmişse catalog listesini filtrele
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                availableCourses = availableCourses.Where(c => c.CategoryId == categoryId.Value).ToList();
            }

            // E) %10 İNDİRİM KAREKOD OLUŞTURMA (QRCoder)
            try
            {
                string indirimLinki = $"https://educore.com/discount?code=EDU10&studentId={studentId}";
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(indirimLinki, QRCodeGenerator.ECCLevel.Q))
                    {
                        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                        {
                            byte[] qrCodeBytes = qrCode.GetGraphic(20);
                            string base64Gorsel = Convert.ToBase64String(qrCodeBytes);
                            ViewBag.KareKodGorseli = $"data:image/png;base64,{base64Gorsel}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Karekod oluşturulurken bir hata oluştu.");
                ViewBag.KareKodGorseli = null;
            }

            return View(availableCourses); // Index.cshtml / HomePage.cshtml dosyasına kayıt olunmamış kurslar gider
        }

        // 2. KURS MÜFREDATI VE DETAY SAYFASI
        public IActionResult Details(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Geçersiz Kurs Id ile detay istendi. Id: {Id}", id);
                return NotFound();
            }

            // Kurs detay verisini cache'ten çekiyoruz
            string courseCacheKey = $"course_detail_{id}";
            if (!_cache.TryGetValue(courseCacheKey, out Course course))
            {
                course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == id);
                if (course != null)
                {
                    if (course.CategoryId.HasValue)
                    {
                        course.Category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == course.CategoryId.Value);
                    }

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set(courseCacheKey, course, cacheOptions);
                }
            }

            if (course == null)
            {
                _logger.LogWarning("Detayı istenen kurs bulunamadı. Id: {Id}", id);
                return NotFound();
            }

            // Kursa ait ders müfredatını cache'ten çekiyoruz
            string lessonsCacheKey = $"course_lessons_{id}";
            if (!_cache.TryGetValue(lessonsCacheKey, out List<Lesson> lessons))
            {
                lessons = _unitOfWork.Lesson.GetAll(l => l.CourseId == id).OrderBy(l => l.SortOrder).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(lessonsCacheKey, lessons, cacheOptions);
            }

            // Kursa ait yapılan değerlendirmeleri cache'ten çekiyoruz
            string reviewsCacheKey = $"course_reviews_{id}";
            if (!_cache.TryGetValue(reviewsCacheKey, out List<Review> reviews))
            {
                reviews = _unitOfWork.Review.GetAll(r => r.CourseId == id, includeProperties: "Student").ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(reviewsCacheKey, reviews, cacheOptions);
            }

            // Giriş yapan öğrencinin bu kursa önceden kayıt olup olmadığını sorguluyoruz
            bool isEnrolled = false;
            int progressPercent = 0;
            bool hasReviewed = false;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                int studentId = Convert.ToInt32(claim.Value);
                
                // Öğrencinin aktif kayıt listesi cache'ten alınır, yoksa veritabanından çekilir
                string enrollmentsCacheKey = $"student_enrollments_{studentId}";
                if (!_cache.TryGetValue(enrollmentsCacheKey, out List<Enrollment> studentEnrollments))
                {
                    studentEnrollments = _unitOfWork.Enrollment.GetAll(e => e.StudentId == studentId).ToList();
                }

                var enrollment = studentEnrollments.FirstOrDefault(e => e.CourseId == id);
                if (enrollment != null)
                {
                    isEnrolled = true;
                    progressPercent = enrollment.ProgressPercent;
                }
                hasReviewed = reviews.Any(r => r.StudentId == studentId);
            }

            ViewBag.Lessons = lessons;
            ViewBag.Reviews = reviews;
            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.ProgressPercent = progressPercent;
            ViewBag.HasReviewed = hasReviewed;

            return View(course); // Views/Home/Details.cshtml sayfasını açar
        }

        // 3. KURSA KAYDOLMA (ENROLLMENT EKLEME)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(int courseId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            int studentId = Convert.ToInt32(claim.Value);

            var existingEnrollment = _unitOfWork.Enrollment.GetFirstOrDefault(e =>
                e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null)
            {
                TempData["info"] = "Bu kursa zaten kayıtlısınız.";
                return RedirectToAction("Index", "Enrollment", new { area = "User" });
            }

            Enrollment newEnrollment = new Enrollment()
            {
                StudentId = studentId,
                CourseId = courseId,
                ProgressPercent = 0,
                EnrolledAt = DateTime.UtcNow
            };

            try
            {
                _unitOfWork.Enrollment.Add(newEnrollment);
                _unitOfWork.Save();

                // Kayıt başarılı olduğunda öğrencinin kayıtlı olduğu kursların cache verisini temizliyoruz (Invalidation)
                _cache.Remove($"student_enrollments_{studentId}");

                _logger.LogInformation("Öğrenci kursa başarıyla kaydoldu. Öğrenci Id: {StudentId}, Kurs Id: {CourseId}", studentId, courseId);
                TempData["success"] = "Kursa başarıyla kaydoldunuz! Öğrenmeye hemen başlayabilirsiniz.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kursa kaydolurken hata oluştu.");
                TempData["error"] = "Kayıt sırasında teknik bir hata oluştu.";
            }

            return RedirectToAction("Index", "Enrollment", new { area = "User" });
        }

        // 4. DEĞERLENDİRME & YORUM GÖNDERME (POST)
        [HttpPost]
        [Authorize]
        public IActionResult SubmitReview(int courseId, int rating, string comment)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return Challenge();
            }

            int studentId = Convert.ToInt32(claim.Value);

            if (rating < 1 || rating > 5)
            {
                TempData["error"] = "Lütfen 1 ile 5 arasında bir puan seçin.";
                return RedirectToAction("Details", new { id = courseId });
            }

            var existingReview = _unitOfWork.Review.GetFirstOrDefault(r =>
                r.StudentId == studentId && r.CourseId == courseId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.Now;
                _unitOfWork.Review.Update(existingReview);

                TempData["success"] = "Değerlendirmeniz başarıyla güncellendi!";
            }
            else
            {
                Review review = new()
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                };
                _unitOfWork.Review.Add(review);

                TempData["success"] = "Değerlendirmeniz için teşekkür ederiz!";
            }

            _unitOfWork.Save();

            // Yorum eklendiğinde veya güncellendiğinde ilgili kursun yorum cache verisini temizliyoruz (Invalidation)
            _cache.Remove($"course_reviews_{courseId}");

            return RedirectToAction("Details", new { id = courseId });
        }
    }
}