using System.ComponentModel.DataAnnotations;

namespace FitnessHubMVC.Models
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
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? PurchaseDate { get; set; }
    }
}
