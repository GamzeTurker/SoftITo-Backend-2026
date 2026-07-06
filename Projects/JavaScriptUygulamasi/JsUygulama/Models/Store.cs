using System.ComponentModel.DataAnnotations;

namespace JsUygulama.Models
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mağaza İsmini Giriniz")]
        [StringLength(100)]
        public string StoreName { get; set; }

        [Required(ErrorMessage = "Şehri Giriniz")]
        [StringLength(50)]
        public string City { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        public int? EmployeeCapacity { get; set; }
    }


}

