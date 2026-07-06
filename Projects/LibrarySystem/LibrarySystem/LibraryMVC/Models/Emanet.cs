using System;

namespace LibraryMVC.Models
{
    public class Emanet
    {
        public int Id { get; set; }
        public int KitapId { get; set; }
        public int UyeId { get; set; }
        public DateTime EmanetTarihi { get; set; } = DateTime.Now;
        public DateTime? TeslimTarihi { get; set; }
        public string Durum { get; set; } = "Emanette"; // 'Emanette', 'Teslim Edildi', 'Gecikmiş'

        // Join properties
        public string? KitapBaslik { get; set; }
        public string? UyeAdSoyad { get; set; }
    }
}
