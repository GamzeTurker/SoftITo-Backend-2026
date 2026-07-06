using Dapper;
using dpperUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace dpperUygulama.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            if (string.IsNullOrEmpty(searchText))
            {
                return View(Context.Listeleme<Employee>("EmployeeViewAll"));
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                return View(Context.Listeleme<Employee>("EmployeeSearch", param));
            }
        }
        public IActionResult EY(int id = 0)
        {
            if (id == 0)
            {
                return View();
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@EmployeeId", id);
                return View(Context.Listeleme<Employee>("EmployeeViewById", param).FirstOrDefault());
            }

        }
        [HttpPost]
        public IActionResult EY(Employee employee)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@EmployeeId", employee.EmployeeId);
            param.Add("@EmployeeName", employee.EmployeeName);
            param.Add("@JobTitle", employee.JobTitle);

            Context.ExecuteReturn("EmployeeEY", param);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@EmployeeId", id);

            var employee = Context
                .Listeleme<Employee>("EmployeeViewById", param)
                .FirstOrDefault();

            return View(employee);
        }

        [HttpPost]
        public IActionResult Delete(Employee employee)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@EmployeeId", employee.EmployeeId);

            Context.ExecuteReturn("EmployeeDelete", param);

            return RedirectToAction("Index");
        }
    }
}
