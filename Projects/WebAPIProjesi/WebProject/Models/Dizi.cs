namespace WebProject.Models
{
    public class Dizi
    {
        public int DiziId { get; set; }
        public string DiziAd { get; set; } = string.Empty;
        public string Tur { get; set; } = string.Empty;
        public int BolumSayisi { get; set; }
        public int YapimYili { get; set; }
    }
}
