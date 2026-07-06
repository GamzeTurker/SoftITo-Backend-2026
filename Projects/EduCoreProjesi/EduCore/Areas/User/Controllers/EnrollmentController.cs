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
    public class EnrollmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache; // Injected IMemoryCache

        public EnrollmentController(IUnitOfWork unitOfWork, IMemoryCache cache)
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

        // 1. KURSLARIM EKRANI (Index) - Caching uygulandı
        public IActionResult Index()
        {
            int studentId = GetLoggedInUserId();
            string enrollmentsCacheKey = $"student_enrollments_{studentId}";

            if (!_cache.TryGetValue(enrollmentsCacheKey, out List<Enrollment> enrollments))
            {
                enrollments = _unitOfWork.Enrollment.GetAll(e => e.StudentId == studentId, includeProperties: "Course.Category").ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)) // 5 dakika sonra silinir
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2)); // 2 dakika boyunca istek gelmezse silinir

                _cache.Set(enrollmentsCacheKey, enrollments, cacheOptions);
            }

            return View(enrollments);
        }

        // 2. DERS İZLEME ODASI (Watch) - Caching uygulandı
        public IActionResult Watch(int id) // id = LessonId
        {
            int studentId = GetLoggedInUserId();

            // Dersi cache'ten çekiyoruz
            string lessonCacheKey = $"lesson_{id}";
            if (!_cache.TryGetValue(lessonCacheKey, out Lesson lesson))
            {
                lesson = _unitOfWork.Lesson.GetFirstOrDefault(x => x.Id == id);
                if (lesson != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                    
                    _cache.Set(lessonCacheKey, lesson, cacheOptions);
                }
            }

            if (lesson == null) return NotFound();

            // Öğrenci bu kursa kayıtlı mı kontrolü (Öğrenci enrollments cache'ini kullanıyoruz)
            string enrollmentsCacheKey = $"student_enrollments_{studentId}";
            if (!_cache.TryGetValue(enrollmentsCacheKey, out List<Enrollment> enrollments))
            {
                enrollments = _unitOfWork.Enrollment.GetAll(e => e.StudentId == studentId).ToList();
            }

            var enrollment = enrollments.FirstOrDefault(e => e.CourseId == lesson.CourseId);
            if (enrollment == null)
            {
                TempData["error"] = "Bu dersi izlemek için önce kursa kaydolmalısınız.";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            // Kurs bilgisini cache'ten veya DB'den bağlıyoruz
            string courseCacheKey = $"course_detail_{lesson.CourseId}";
            if (!_cache.TryGetValue(courseCacheKey, out Course course))
            {
                course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == lesson.CourseId);
            }
            lesson.Course = course;

            // Diğer dersleri cache'ten çekiyoruz
            string lessonsCacheKey = $"course_lessons_{lesson.CourseId}";
            if (!_cache.TryGetValue(lessonsCacheKey, out List<Lesson> otherLessons))
            {
                otherLessons = _unitOfWork.Lesson.GetAll(l => l.CourseId == lesson.CourseId).OrderBy(l => l.SortOrder).ToList();
                
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                
                _cache.Set(lessonsCacheKey, otherLessons, cacheOptions);
            }

            ViewBag.OtherLessons = otherLessons;
            ViewBag.CurrentEnrollment = enrollment;

            return View(lesson);
        }

        // 3. DERS TAMAMLAMA (İlerleme Yüzdesi ve Sertifika Üretimi)
        [HttpPost]
        public IActionResult CompleteLesson(int lessonId)
        {
            int studentId = GetLoggedInUserId();
            var lesson = _unitOfWork.Lesson.GetFirstOrDefault(x => x.Id == lessonId);
            if (lesson == null) return NotFound();

            var enrollment = _unitOfWork.Enrollment.GetFirstOrDefault(e => e.StudentId == studentId && e.CourseId == lesson.CourseId);
            if (enrollment == null) return BadRequest("Kurs kaydı bulunamadı.");

            int totalLessons = _unitOfWork.Lesson.GetAll(l => l.CourseId == lesson.CourseId).Count();
            if (totalLessons > 0)
            {
                int calculatedProgress = (lesson.SortOrder * 100) / totalLessons;
                if (calculatedProgress > enrollment.ProgressPercent)
                {
                    enrollment.ProgressPercent = calculatedProgress;
                }

                // İlerleme %100 ise Sertifika oluştur
                if (enrollment.ProgressPercent >= 100)
                {
                    var existingCert = _unitOfWork.Certificate.GetFirstOrDefault(c => c.EnrollmentId == enrollment.Id);
                    if (existingCert == null)
                    {
                        Certificate certificate = new()
                        {
                            EnrollmentId = enrollment.Id,
                            CertificateImageUrl = @"\img\certificates\default-certificate.jpg",
                            IssuedAt = DateTime.UtcNow
                        };
                        _unitOfWork.Certificate.Add(certificate);
                        
                        // Sertifikalar listesi cache'ini sıfırlıyoruz (Yeni sertifika eklendiği için)
                        _cache.Remove($"student_certificates_{studentId}");
                    }
                }
                _unitOfWork.Save();

                // Kayıt ilerleme durumu değiştiği için kayıt listesi cache'ini sıfırlıyoruz
                _cache.Remove($"student_enrollments_{studentId}");
            }

            // Bir sonraki dersi bul
            var nextLesson = _unitOfWork.Lesson.GetAll(l => l.CourseId == lesson.CourseId)
                                               .Where(l => l.SortOrder > lesson.SortOrder)
                                               .OrderBy(l => l.SortOrder)
                                               .FirstOrDefault();

            if (nextLesson != null)
            {
                return RedirectToAction(nameof(Watch), new { id = nextLesson.Id });
            }

            TempData["success"] = "Tebrikler! Kursu tamamladınız. Başarı sertifikanız oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // 4. KURS KAYDINI SİLME / İPTAL ETME
        [HttpPost]
        public IActionResult Cancel(int id)
        {
            int studentId = GetLoggedInUserId();
            var enrollment = _unitOfWork.Enrollment.GetFirstOrDefault(e => e.Id == id && e.StudentId == studentId);
            if (enrollment != null)
            {
                _unitOfWork.Enrollment.Remove(enrollment);
                _unitOfWork.Save();
                
                // İptal yapıldığından kayıt cache'ini sıfırlıyoruz
                _cache.Remove($"student_enrollments_{studentId}");
                _cache.Remove($"student_certificates_{studentId}"); // Sertifika varsa o da iptal olabilir

                TempData["success"] = "Kurs kaydınız silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}