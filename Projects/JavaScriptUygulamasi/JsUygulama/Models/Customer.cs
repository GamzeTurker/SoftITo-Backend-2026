using System.ComponentModel.DataAnnotations;

namespace JsUygulama.Models
{
    public class Customer
    {
        
            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "İsim Giriniz")]
            [StringLength(50)]
            public string Name { get; set; }


            [Required(ErrorMessage = "Telefon Giriniz")]
            [StringLength(15)]
            public string Phone { get; set; }

           
            [StringLength(100)]
            public string? Email { get; set; }
            [Required(ErrorMessage = "Şehir Giriniz")]
            [StringLength(50)]
            public string City { get; set; }
            [Required(ErrorMessage = "Adres Giriniz")]
            [StringLength(200)]
            public string Address { get; set; }

            public DateTime CreatedDate { get; set; } = DateTime.Now;
        }
    }


