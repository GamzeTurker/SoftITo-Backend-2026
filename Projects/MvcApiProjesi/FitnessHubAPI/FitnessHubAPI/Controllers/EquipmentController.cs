using FitnessHubAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public EquipmentController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("GetEquipments")]
        public async Task<IEnumerable<Equipment>> GetEquipments()
        {
            return await context.Equipments.ToListAsync();
        }
        [HttpGet]
        [Route("GetEquipmentsById/{id}")]
        public async Task<Equipment> GetEquipmentsById(int id)
        {
            return await context.FindAsync<Equipment>(id);
        }

        [HttpPost]
        [Route("AddEquipments")]
        public async Task<Equipment> AddEquipments(Equipment equipment)
        {
            context.Add(equipment);
            await context.SaveChangesAsync();
            return equipment;
        }

        
        [HttpPut]
        [Route("UpdateEquipments/{id}")]
        public async Task<Equipment> UpdateEquipments(Equipment equipment)
        {
            context.Update(equipment);
            await context.SaveChangesAsync();
            return equipment;
        }

        [HttpDelete]
        [Route("DeleteEquipments/{id}")]
        public bool DeleteEquipments(int id)
        {
            var proc = false;
            var result = context.Equipments.Find(id);
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
