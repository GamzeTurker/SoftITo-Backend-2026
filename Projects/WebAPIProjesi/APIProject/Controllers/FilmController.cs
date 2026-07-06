using APIProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly AppDbContext context;
        public FilmController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("GetFilm")]
        public async Task<IEnumerable<Film>> GetFilm()
        {
            return await context.Filmler.ToListAsync();
        }

        [HttpPost]
        [Route("AddFilm")]

        public async Task<Film> AddFilm(Film film)
        {
            context.Filmler.Add(film);
            await context.SaveChangesAsync();
            return film;
        }
        [HttpPut]
        [Route("UpdateFilm/{id}")]
        public async Task<Film> UpdateFilm(Film film,int id)
        {
            context.Entry(film).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return film;


        }

        [HttpDelete]
        [Route("DeleteFilm/{id}")]
        public async Task<Film> DeleteFilm(Film film, int id)
        {
            context.Entry(film).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return film;


        }
    }
}
