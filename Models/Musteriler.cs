using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace SiparisSistemi.Models
{
    public class Musteriler
    {
        [Key]
        public int MusteriId { get; set; }
        public string MusteriAdSoyad { get; set; }
       
        public string Telefon { get; set; }
        public DateTime DogumTarihi { get; set; }

    }
}
