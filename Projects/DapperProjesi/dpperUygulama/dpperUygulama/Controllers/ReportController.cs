using Dapper;
using dpperUygulama.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace dpperUygulama.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index(string type = "5")
        {
            ViewBag.Type = type;

            // Dropdown için
            ViewBag.ReportTypes = new List<object>
            {
                new { Value = "5", Text = "Tüm Raporlar (Genel Görünüm)" },
                new { Value = "1", Text = "Toplam Servis & Gelir" },
                new { Value = "2", Text = "En Çok Servis Alan Araç" },
                new { Value = "3", Text = "En Aktif Personel" },
                new { Value = "4", Text = "Toplam Gelir" }
            };

            // Tüm rapor verilerini çekelim
            var serviceReport = Context.Listeleme<ReportModel>("ServiceReport").FirstOrDefault() ?? new ReportModel();
            var topVehicleReport = Context.Listeleme<ReportModel>("TopVehicleReport").FirstOrDefault() ?? new ReportModel();
            var topEmployeeReport = Context.Listeleme<ReportModel>("TopEmployeeReport").FirstOrDefault() ?? new ReportModel();
            var totalIncomeReport = Context.Listeleme<ReportModel>("TotalIncomeReport").FirstOrDefault() ?? new ReportModel();

            ViewBag.ServiceReport = serviceReport;
            ViewBag.TopVehicleReport = topVehicleReport;
            ViewBag.TopEmployeeReport = topEmployeeReport;
            ViewBag.TotalIncomeReport = totalIncomeReport;

            ReportModel model = new ReportModel();
            switch (type)
            {
                case "1":
                    model = serviceReport;
                    break;
                case "2":
                    model = topVehicleReport;
                    break;
                case "3":
                    model = topEmployeeReport;
                    break;
                case "4":
                    model = totalIncomeReport;
                    break;
                case "5":
                default:
                    model = serviceReport;
                    break;
            }

            return View(model);
        }
    }
}