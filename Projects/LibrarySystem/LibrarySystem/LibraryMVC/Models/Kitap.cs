namespace LibraryMVC.Models
{
    public class Kitap
    {
        public int Id { get; set; }
        public int YazarId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int BasimYili { get; set; }
        public int SayfaSayisi { get; set; }
        
        // Join properties
        public string? YazarAdSoyad { get; set; }
    }
}
