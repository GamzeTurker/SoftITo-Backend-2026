using System.ComponentModel.DataAnnotations;

namespace APIProject.Model
{
    public class CizgiFilm
    {
        [Key]
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Tur { get; set; }
        public int BolumSayisi { get; set; }
        public string YasAraligi { get; set; }
    }
}
