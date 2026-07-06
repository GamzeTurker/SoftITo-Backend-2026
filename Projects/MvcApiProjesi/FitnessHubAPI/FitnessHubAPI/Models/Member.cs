using System.ComponentModel.DataAnnotations;

namespace FitnessHubAPI.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Range(10, 100)]
        public int Age { get; set; }

        [Required]
        [StringLength(50)]
        public string MembershipType { get; set; }

        public DateTime JoinDate { get; set; }
    }
}
