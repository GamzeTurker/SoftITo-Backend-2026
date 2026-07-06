using System.ComponentModel.DataAnnotations;

namespace SiparisSistemi.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public string KullaniciAdi{ get; set; }
        public string Sifre { get; set; }
    }
}
