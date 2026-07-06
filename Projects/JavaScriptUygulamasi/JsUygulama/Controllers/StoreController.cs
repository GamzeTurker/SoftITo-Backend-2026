using JsUygulama.Data;
using JsUygulama.Models;
using Microsoft.AspNetCore.Mvc;

namespace JsUygulama.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult StoreList()
        {
            var data = context.Stores.ToList();
            return new JsonResult(data);
        }

        [HttpPost]
        public JsonResult AddStore(Store store
            )
        {

            var cum = new Store()
            {
                StoreName = store.StoreName,
                City = store.City,
                Phone = store.Phone,
                EmployeeCapacity = store.EmployeeCapacity

            };

            context.Stores.Add(store);
            context.SaveChanges();
            return new JsonResult("Store added");


        }

        public JsonResult Edit(int id)
        {
            var data = context.Stores.Where(m => m.Id == id).SingleOrDefault();
            return new JsonResult(data);

        }
        [HttpPost]
        public JsonResult Update(Store store)
        {
            context.Update(store);
            context.SaveChanges();
            return new JsonResult("Store updated");
        }


        public JsonResult Delete(Store store)
        {

            context.Stores.Remove(store);
            context.SaveChanges();
            return new JsonResult("Store deleted");
        }

    }
}
