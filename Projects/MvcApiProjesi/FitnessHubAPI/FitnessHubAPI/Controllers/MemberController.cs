using FitnessHubAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public MemberController(ApplicationDbContext context) {
            this.context = context;
        }
        [HttpGet]
        [Route("GetMembers")]
        public async Task<IEnumerable<Member>> GetMembers() {
            return await context.Members.ToListAsync();
        }
        [HttpGet]
        [Route("GetMembersById/{id}")]
        public async Task<Member> GetMembersById(int id)
        {
            return await context.FindAsync<Member>(id);
        }

        [HttpPost]
        [Route("AddMembers")]
        public async Task<Member> AddMembers(Member member)
        {
            context.Add(member);
            await context.SaveChangesAsync();
            return member;
        }

        [HttpPut]
        [Route("UpdateMembers/{id}")]
        public async Task<Member> UpdateMembers(Member member)
        {
            context.Update(member);
            await context.SaveChangesAsync();
            return member;
        }

        [HttpDelete]
        [Route("DeleteMembers/{id}")]
        public bool DeleteMembers(int id) {
            var proc = false;
            var result = context.Members.Find(id);
            if(result != null)
            {
                proc =true;
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
