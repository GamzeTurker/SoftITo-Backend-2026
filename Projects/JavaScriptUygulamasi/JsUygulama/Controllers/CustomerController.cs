using JsUygulama.Data;
using JsUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace JsUygulama.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext context;
        public CustomerController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult CustomerList()
        {
            var data = context.Customers.ToList();
            return new JsonResult(data);
        }

        [HttpPost]
        public JsonResult AddCustomer(Customer customer)
        {

            var cum = new Customer()
            {
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                City = customer.City,
                Address = customer.Address,
                CreatedDate = DateTime.Now


            };

            context.Customers.Add(cum);
            context.SaveChanges();
            return new JsonResult("Customer added");


        }

        public JsonResult Edit(int id)
        {
            var data = context.Customers.Where(m => m.Id == id).SingleOrDefault();
            return new JsonResult(data);

        }
        [HttpPost]
        public JsonResult Update(Customer customer)
        {
            context.Update(customer);
            context.SaveChanges();
            return new JsonResult("Customer updated");
        }


        public JsonResult Delete(Customer customer)
        {
            
            context.Customers.Remove(customer);
            context.SaveChanges();
            return new JsonResult("Customer deleted");
        }

    }

}
  