using Microsoft.AspNetCore.Identity;

namespace FitnessHubMVC.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
