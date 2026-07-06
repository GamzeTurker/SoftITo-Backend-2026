using Dapper;
using dpperUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace dpperUygulama.Controllers
{
    public class ServiceRecordController : Controller
    {
        public IActionResult Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            if (string.IsNullOrEmpty(searchText))
            {
                return View(Context.Listeleme<ServiceRecord>("ServiceViewAll"));
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                return View(Context.Listeleme<ServiceRecord>("ServiceSearch", param));
            }
        }

        public IActionResult ExportExcel(string searchText = "")
        {
            IEnumerable<ServiceRecord> services;
            if (string.IsNullOrEmpty(searchText))
            {
                services = Context.Listeleme<ServiceRecord>("ServiceViewAll");
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                services = Context.Listeleme<ServiceRecord>("ServiceSearch", param);
            }

            var excelBytes = ExportHelper.ExportServicesToExcel(services);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ServisKayitlari.xlsx");
        }

        public IActionResult ExportPdf(string searchText = "")
        {
            IEnumerable<ServiceRecord> services;
            if (string.IsNullOrEmpty(searchText))
            {
                services = Context.Listeleme<ServiceRecord>("ServiceViewAll");
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                services = Context.Listeleme<ServiceRecord>("ServiceSearch", param);
            }

            var pdfBytes = ExportHelper.ExportServicesToPdf(services);
            return File(pdfBytes, "application/pdf", "ServisKayitlari.pdf");
        }

        public IActionResult EY(int id = 0)
        {
            // Dropdown verileri
            ViewBag.Vehicles = Context.Listeleme<Vehicle>("VehicleViewAll");
            ViewBag.Employees = Context.Listeleme<Employee>("EmployeeViewAll");

            if (id == 0)
            {
                return View();
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ServiceId", id);

                var data = Context
                    .Listeleme<ServiceRecord>("ServiceViewById", param)
                    .FirstOrDefault();

                return View(data);
            }
        }

        [HttpPost]
        public IActionResult EY(ServiceRecord service)
        {
            DynamicParameters param = new DynamicParameters();

            param.Add("@ServiceId", service.ServiceId);
            param.Add("@VehicleId", service.VehicleId);
            param.Add("@EmployeeId", service.EmployeeId);
            param.Add("@ServiceDate", service.ServiceDate);
            param.Add("@Description", service.Description);
            param.Add("@Cost", service.Cost);

            Context.ExecuteReturn("ServiceEY", param);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            // Dropdown verileri
            ViewBag.Vehicles = Context.Listeleme<Vehicle>("VehicleViewAll");
            ViewBag.Employees = Context.Listeleme<Employee>("EmployeeViewAll");
            DynamicParameters param = new DynamicParameters();
            param.Add("@ServiceId", id);

            var data = Context
                .Listeleme<ServiceRecord>("ServiceViewById", param)
                .FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        public IActionResult Delete(ServiceRecord service)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@ServiceId", service.ServiceId);

            Context.ExecuteReturn("ServiceDelete", param);

            return RedirectToAction("Index");
        }
    }
}