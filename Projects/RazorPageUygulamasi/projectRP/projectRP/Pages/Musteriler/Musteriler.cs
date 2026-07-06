using System.ComponentModel.DataAnnotations;

namespace projectRP.Pages.Musteriler
{
    public class Musteriler
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

        [Required]
        [StringLength(50)]
        public string Sehir { get; set; }

        [StringLength(255)]
        public string Notlar { get; set; }
    }
}
