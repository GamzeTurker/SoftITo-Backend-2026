using APIProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CizgiFilmController : ControllerBase
    {
        private readonly AppDbContext context;
        public CizgiFilmController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("GetCFilm")]
        public async Task<IEnumerable<CizgiFilm>> GetCFilm()
        {
            return await context.CizgiFilmler.ToListAsync();
        }

        [HttpPost]
        [Route("AddCFilm")]

        public async Task<CizgiFilm> AddCFilm(CizgiFilm cizgiFilm)
        {
            context.CizgiFilmler.Add(cizgiFilm);
            await context.SaveChangesAsync();
            return cizgiFilm;
        }
        [HttpPut]
        [Route("UpdateCFilm/{id}")]
        public async Task<CizgiFilm> UpdateCFilm(CizgiFilm cizgiFilm, int id)
        {
            context.Entry(cizgiFilm).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return cizgiFilm;


        }

        [HttpDelete]
        [Route("DeleteCFilm/{id}")]
        public async Task<CizgiFilm> DeleteCFilm(CizgiFilm cizgiFilm, int id)
        {
            context.Entry(cizgiFilm).State = EntityState.Deleted;
                
            await context.SaveChangesAsync();
            return cizgiFilm;


        }
    }
}
