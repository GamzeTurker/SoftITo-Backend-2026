using System;

namespace LibraryAPI.Models
{
    public class Yazar
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string? Biyografi { get; set; }
        public DateTime? DogumTarihi { get; set; }
    }
}
