using System;

namespace LibraryAPI.Models
{
    public class Uye
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
    }
}
