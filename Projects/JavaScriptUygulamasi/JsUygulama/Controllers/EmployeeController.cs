using JsUygulama.Data;
using JsUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace JsUygulama.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext context;
        public EmployeeController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult EmployeeList()
        {
            var data = context.Employees.ToList();
            return new JsonResult(data);
        }

        [HttpPost]
        public JsonResult AddEmployee(Employee employee)
        {

            var cum = new Employee()
            {
                Name = employee.Name,
                Position = employee.Position,
                Salary = employee.Salary,
                Phone = employee.Phone

            };

            context.Employees.Add(employee);
            context.SaveChanges();
            return new JsonResult("Employee added");


        }

        public JsonResult Edit(int id)
        {
            var data = context.Employees.Where(m => m.Id == id).SingleOrDefault();
            return new JsonResult(data);

        }
        [HttpPost]
        public JsonResult Update(Employee employee)
        {
            context.Update(employee);
            context.SaveChanges();
            return new JsonResult("Employee updated");
        }


        public JsonResult Delete(Employee employee)
        {

            context.Employees.Remove(employee);
            context.SaveChanges();
            return new JsonResult("Employee deleted");
        }

    }

}
