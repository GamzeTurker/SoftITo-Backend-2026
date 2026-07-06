using JsUygulama.Data;
using Microsoft.AspNetCore.Mvc;

namespace JsUygulama.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;

        public UserController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // SADECE STORE GÖRÜNTÜLEME
        public JsonResult StoreList()
        {
            return Json(context.Stores.ToList());
        }
    }
}
