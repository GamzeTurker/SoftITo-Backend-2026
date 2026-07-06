using Microsoft.AspNetCore.Identity;

namespace WebProject.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
