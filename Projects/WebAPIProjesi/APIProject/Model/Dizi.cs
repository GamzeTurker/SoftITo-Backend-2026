using System.ComponentModel.DataAnnotations;

namespace APIProject.Model
{
    public class Dizi
    {
        [Key]
        public int DiziId { get; set; }
        public string DiziAd { get; set; }
        public string Tur { get; set; }
        public int BolumSayisi { get; set; }
        public int YapimYili { get; set; }
    }
}
