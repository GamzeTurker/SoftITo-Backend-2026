using System.ComponentModel.DataAnnotations;

namespace FitnessHubMVC.Models
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }

        [Required]
        public string ExerciseName { get; set; }

        public int SetCount { get; set; }
        public int RepeatCount { get; set; }

        // FK -> WorkoutPlan
        public int WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }
    }
}
