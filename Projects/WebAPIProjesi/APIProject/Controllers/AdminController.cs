using APIProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext context;
        public AdminController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("GetAdmin")]
        public async Task<IEnumerable<Admin>> GetAdmin()
        {
            return await context.Adminler.ToListAsync();
        }
        [HttpPost]
        [Route("AddAdmin")]
        public async Task<Admin> AddAdmin(Admin admin)
        {
            context.Adminler.Add(admin);
            await context.SaveChangesAsync();
            return admin;
        }
    }
}
