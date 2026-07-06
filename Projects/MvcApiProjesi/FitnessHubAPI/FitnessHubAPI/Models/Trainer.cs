using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessHubAPI.Models
{
    public class Trainer
    {
        [Key]
        public int TrainerId { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Range(0, 60)]
        public int ExperienceYear { get; set; }
    }
}
