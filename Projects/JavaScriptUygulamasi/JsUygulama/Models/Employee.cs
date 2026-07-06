using System.ComponentModel.DataAnnotations;

namespace JsUygulama.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim Giriniz")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pozisyon Giriniz")]
        [StringLength(50)]
        public string Position { get; set; }

        [Required]
        public decimal? Salary { get; set; }
        [Required(ErrorMessage = "Telefon Giriniz")]
        [StringLength(15)]
        public string Phone { get; set; }
    }


}

