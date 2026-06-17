using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiparisSistemi.Models
{
    public class Siparisler
    {
        [Key]
        public int SiparisId { get; set; }
        [ForeignKey("musteriler")]
        public int MusteriId { get; set; }

        [ForeignKey("urunler")]
        public int UrunId { get; set; }
        public int Adet { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string SiparisDurumu { get; set; }
        public virtual Musteriler musteriler { get; set; }
        public virtual Urunler urunler { get; set; }

    }
}
