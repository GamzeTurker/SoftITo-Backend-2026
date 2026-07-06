using System.ComponentModel.DataAnnotations;

namespace APIProject.Model
{
    public class Film
    {
        [Key]
        public int FilmId { get; set; }
        public string FilmAd { get; set; }
        public string Tur { get; set; }
        public int Sure { get; set; } // dakika
        public int YapimYili { get; set; }
    }
}
