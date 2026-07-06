using FitnessHubAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public TrainerController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("GetTrainers")]
        public async Task<IEnumerable<Trainer>> GetTrainers()
        {
            return await context.Trainers.ToListAsync();
        }
        [HttpGet]
        [Route("GetTrainersById/{id}")]
        public async Task<Trainer> GetTrainersById(int id)
        {
            return await context.FindAsync<Trainer>(id);
        }

        [HttpPost]
        [Route("AddTrainers")]
        public async Task<Trainer> AddTrainers(Trainer trainer)
        {
            context.Add(trainer);
            await context.SaveChangesAsync();
            return trainer;
        }

        [HttpPut]
        [Route("UpdateTrainers/{id}")]
        public async Task<Trainer> UpdateTrainers(Trainer trainer)
        {
            context.Update(trainer);
            await context.SaveChangesAsync();
            return trainer;
        }

        [HttpDelete]
        [Route("DeleteTrainers/{id}")]
        public bool DeleteTrainers(int id)
        {
            var proc = false;
            var result = context.Trainers.Find(id);

            if (result != null)
            {
                proc = true;
                context.Remove(result);
                context.SaveChanges();
                return proc;

            }
            else
            {
                return proc;
            }
            return proc;
        }
    }
}
