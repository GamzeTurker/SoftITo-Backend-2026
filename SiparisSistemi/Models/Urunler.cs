using System.ComponentModel.DataAnnotations;

namespace SiparisSistemi.Models
{
    public class Urunler
    {
        [Key]
        public int UrunId { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
    }
}
