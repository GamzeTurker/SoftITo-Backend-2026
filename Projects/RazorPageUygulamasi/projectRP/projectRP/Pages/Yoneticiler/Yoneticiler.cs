using System.ComponentModel.DataAnnotations;

namespace projectRP.Pages.Yoneticiler
{
    public class Yoneticiler
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required]
        [StringLength(100)]
        public string Eposta { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefon { get; set; }

      
    }
}
