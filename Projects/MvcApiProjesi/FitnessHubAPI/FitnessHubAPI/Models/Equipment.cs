using System.ComponentModel.DataAnnotations;

namespace FitnessHubAPI.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(150)]
        public string EquipmentName { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime? PurchaseDate { get; set; }
    }
}
