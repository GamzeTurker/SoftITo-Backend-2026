using System.ComponentModel.DataAnnotations;

namespace projectRP.Pages.Calisanlar
{
    public class Calisanlar
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required]
        [StringLength(100)]
        public string Departman { get; set; }

        [Required]
        [StringLength(100)]
        public string Pozisyon { get; set; }

        [Required]
        public decimal Maas { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefon { get; set; }
    }
}
