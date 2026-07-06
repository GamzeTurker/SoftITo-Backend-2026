using System.ComponentModel.DataAnnotations;

namespace FitnessHubMVC.Models
{
    public class WorkoutPlan
    {
        [Key]
        public int WorkoutPlanId { get; set; }

        [Required]
        public string PlanName { get; set; }

        public string Goal { get; set; }

        public DateTime CreatedDate { get; set; }


        public List<Exercise>? Exercises { get; set; }
    }
}
