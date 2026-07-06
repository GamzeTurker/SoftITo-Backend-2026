using System.ComponentModel.DataAnnotations;

namespace APIProject.Model
{
    public class Admin
    {
        [Key]
        public int AminId { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Email { get; set; }
    }
}
