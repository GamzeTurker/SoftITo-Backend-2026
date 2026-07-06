using Dapper;
using dpperUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace dpperUygulama.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            if (string.IsNullOrEmpty(searchText))
            {
                return View(Context.Listeleme<Customer>("CustomerViewAll"));
            }
            else
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchText", searchText);
                return View(Context.Listeleme<Customer>("CustomerSearch", param));
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
                param.Add("@CustomerId", id);
                return View(Context.Listeleme<Customer>("CustomerViewById", param).FirstOrDefault());
            }

        }
        [HttpPost]
        public IActionResult EY(Customer customer)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CustomerId", customer.CustomerId);
            param.Add("@FullName", customer.FullName);
            param.Add("@Phone", customer.Phone);
            param.Add("@Address", customer.Address);
         
            Context.ExecuteReturn("CustomerEY", param);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CustomerId", id);

            var customer = Context
                .Listeleme<Customer>("CustomerViewById", param)
                .FirstOrDefault();

            return View(customer);
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CustomerId", customer.CustomerId);

            Context.ExecuteReturn("CustomerDelete", param);

            return RedirectToAction("Index");
        }
    }
}
