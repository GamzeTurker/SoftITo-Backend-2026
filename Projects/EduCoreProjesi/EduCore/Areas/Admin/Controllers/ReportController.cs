using EduCore.Data;
using EduCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Admin yetkilendirmesi olanlar erişebilir
    public class ReportController : Controller
    {
        private readonly EducationDbContext _context;

        public ReportController(EducationDbContext context)
        {
            _context = context;
        }

        // RAPORLAR ANA SAYFASI
        public IActionResult Index(int reportId = 1)
        {
            ViewBag.SelectedReportId = reportId;

            // Rapor seçeneklerini ViewBag'e atıyoruz
            ViewBag.ReportList = new List<object>
            {
                new { Id = 1, Name = "1. Kurslar ve Kategori Bilgileri (JOIN)" },
                new { Id = 2, Name = "2. Öğrenci Kayıtları ve İlerlemeleri (JOIN)" },
                new { Id = 3, Name = "3. Kurs Değerlendirmeleri ve Yorumlar (JOIN)" },
                new { Id = 4, Name = "4. Kategorilere Göre Kurs Sayısı ve Ort. Fiyat (GROUP BY)" },
                new { Id = 5, Name = "5. Fiyatı 150 TL Altındaki Kurslar (SELECT WHERE)" },
                new { Id = 6, Name = "6. Kursunu Tamamlayan (%100) Öğrenciler (SELECT WHERE)" }
            };

            object reportData = null;

            switch (reportId)
            {
                case 1:
                    // 1. JOIN: Kurslar ve Kategorileri (new select ile projeksiyon)
                    reportData = (from c in _context.Courses
                                  join cat in _context.Categories on c.CategoryId equals cat.Id
                                  select new ReportItem1
                                  {
                                      CourseId = c.Id,
                                      CourseTitle = c.Title,
                                      CategoryName = cat.Name,
                                      Price = c.Price,
                                      CreatedAt = c.CreatedAt
                                  }).ToList();
                    break;

                case 2:
                    // 2. JOIN: Öğrenci Kayıtları ve Kurs Bilgisi (new select ile projeksiyon)
                    reportData = (from e in _context.Enrollments
                                  join u in _context.Users on e.StudentId equals u.Id
                                  join c in _context.Courses on e.CourseId equals c.Id
                                  select new ReportItem2
                                  {
                                      EnrollmentId = e.Id,
                                      StudentName = u.Name,
                                      CourseTitle = c.Title,
                                      ProgressPercent = e.ProgressPercent,
                                      EnrolledAt = e.EnrolledAt
                                  }).ToList();
                    break;

                case 3:
                    // 3. JOIN: Kurs Değerlendirmeleri ve Öğrenci Bilgisi (new select ile projeksiyon)
                    reportData = (from r in _context.Reviews
                                  join u in _context.Users on r.StudentId equals u.Id
                                  join c in _context.Courses on r.CourseId equals c.Id
                                  select new ReportItem3
                                  {
                                      ReviewId = r.Id,
                                      StudentName = u.Name,
                                      CourseTitle = c.Title,
                                      Rating = r.Rating,
                                      Comment = r.Comment,
                                      CreatedAt = r.CreatedAt
                                  }).ToList();
                    break;

                case 4:
                    // 4. GROUP BY: Kategori bazlı Kurs Miktarı ve Ortalama Fiyat
                    reportData = (from c in _context.Courses
                                  group c by c.Category.Name into g
                                  select new ReportGroupByItem
                                  {
                                      CategoryName = g.Key ?? "Kategorisiz",
                                      CourseCount = g.Count(),
                                      AveragePrice = g.Average(x => x.Price)
                                  }).ToList();
                    break;

                case 5:
                    // 5. SELECT WHERE: Fiyatı 150 TL'den düşük olan kurslar
                    reportData = _context.Courses
                                         .Where(c => c.Price < 150)
                                         .Select(c => new ReportWhereItem1
                                         {
                                             CourseId = c.Id,
                                             CourseTitle = c.Title,
                                             Price = c.Price,
                                             CreatedAt = c.CreatedAt
                                         }).ToList();
                    break;

                case 6:
                    // 6. SELECT WHERE: Tamamlama oranı %100 olan kayıtlar
                    reportData = _context.Enrollments
                                         .Where(e => e.ProgressPercent == 100)
                                         .Select(e => new ReportWhereItem2
                                         {
                                             EnrollmentId = e.Id,
                                             StudentName = e.Student.Name,
                                             CourseTitle = e.Course.Title,
                                             EnrolledAt = e.EnrolledAt
                                         }).ToList();
                    break;
            }

            return View(reportData);
        }
    }

    // --- RAPOR DTO MODELLERİ ---

    public class ReportItem1
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReportItem2
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; }
        public string CourseTitle { get; set; }
        public int ProgressPercent { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    public class ReportItem3
    {
        public int ReviewId { get; set; }
        public string StudentName { get; set; }
        public string CourseTitle { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReportGroupByItem
    {
        public string CategoryName { get; set; }
        public int CourseCount { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class ReportWhereItem1
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReportWhereItem2
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; }
        public string CourseTitle { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
