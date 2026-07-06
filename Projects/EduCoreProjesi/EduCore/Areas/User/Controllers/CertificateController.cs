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
    public class CertificateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache; // Injected IMemoryCache

        public CertificateController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        // ÖĞRENCİ SERTİFİKALARI LİSTESİ (Index) - Caching uygulandı
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            int studentId = Convert.ToInt32(claim.Value);

            string certsCacheKey = $"student_certificates_{studentId}";

            if (!_cache.TryGetValue(certsCacheKey, out List<Certificate> certificates))
            {
                certificates = _unitOfWork.Certificate.GetAll(
                    c => c.Enrollment.StudentId == studentId,
                    includeProperties: "Enrollment.Course"
                ).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) // 10 dakika sonra silinir
                    .SetSlidingExpiration(TimeSpan.FromMinutes(3)); // 3 dakika boyunca istek gelmezse silinir

                _cache.Set(certsCacheKey, certificates, cacheOptions);
            }

            return View(certificates);
        }
    }
}