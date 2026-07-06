using APIProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiziController : ControllerBase
    {
        private readonly AppDbContext context;
        public DiziController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("GetDizi")]
        public async Task<IEnumerable<Dizi>> GetDizi()
        {
            return await context.Diziler.ToListAsync();
        }

        [HttpPost]
        [Route("AddDizi")]

        public async Task<Dizi> AddDizi(Dizi dizi)
        {
            context.Diziler.Add(dizi);
            await context.SaveChangesAsync();
            return dizi;
        }
        [HttpPut]
        [Route("UpdateDizi/{id}")]
        public async Task<Dizi> UpdateDizi(Dizi dizi, int id)
        {
            context.Entry(dizi).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return dizi;


        }

        [HttpDelete]
        [Route("DeleteDizi/{id}")]
        public async Task<Dizi> DeleteDizi(Dizi dizi, int id)
        {
            context.Entry(dizi).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return dizi;
        }
    }
}
