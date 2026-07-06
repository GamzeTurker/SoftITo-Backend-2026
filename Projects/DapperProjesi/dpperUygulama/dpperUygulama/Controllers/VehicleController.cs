using Dapper;
using dpperUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace dpperUygulama.Controllers
{
    public class VehicleController : Controller
    {
        public IActionResult Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            ViewBag.Customers = Context.Listeleme<Customer>("CustomerViewAll");

            if (string.IsNullOrEmpty(searchText))
            {
                return View(Context.Listeleme<Vehicle>("VehicleViewAll"));
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                return View(Context.Listeleme<Vehicle>("VehicleSearch", param));
            }
        }

        public IActionResult ExportExcel(string searchText = "")
        {
            IEnumerable<Vehicle> vehicles;
            if (string.IsNullOrEmpty(searchText))
            {
                vehicles = Context.Listeleme<Vehicle>("VehicleViewAll");
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                vehicles = Context.Listeleme<Vehicle>("VehicleSearch", param);
            }

            var customers = Context.Listeleme<Customer>("CustomerViewAll");
            var excelBytes = ExportHelper.ExportVehiclesToExcel(vehicles, customers);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Araclar.xlsx");
        }

        public IActionResult ExportPdf(string searchText = "")
        {
            IEnumerable<Vehicle> vehicles;
            if (string.IsNullOrEmpty(searchText))
            {
                vehicles = Context.Listeleme<Vehicle>("VehicleViewAll");
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                vehicles = Context.Listeleme<Vehicle>("VehicleSearch", param);
            }

            var customers = Context.Listeleme<Customer>("CustomerViewAll");
            var pdfBytes = ExportHelper.ExportVehiclesToPdf(vehicles, customers);
            return File(pdfBytes, "application/pdf", "Araclar.pdf");
        }
        public IActionResult EY(int id = 0)
        {
            ViewBag.Customers = Context.Listeleme<Customer>("CustomerViewAll");

            if (id == 0)
            {
                return View();
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@VehicleId", id);
                return View(Context.Listeleme<Vehicle>("VehicleViewById", param).FirstOrDefault());
            }

        }
        [HttpPost]
        public IActionResult EY(Vehicle vehicle)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@VehicleId", vehicle.VehicleId);
            param.Add("@Brand", vehicle.Brand);
            param.Add("@Model", vehicle.Model);
            param.Add("@Plate", vehicle.Plate);
            param.Add("@CustomerId", vehicle.CustomerId);

            Context.ExecuteReturn("VehicleEY", param);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@VehicleId", id);

            var vehicle = Context
                .Listeleme<Vehicle>("VehicleViewById", param)
                .FirstOrDefault();

            return View(vehicle);
        }

        [HttpPost]
        public IActionResult Delete(Vehicle vehicle)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@VehicleId", vehicle.VehicleId);

            Context.ExecuteReturn("VehicleDelete", param);

            return RedirectToAction("Index");
        }
    }
}
