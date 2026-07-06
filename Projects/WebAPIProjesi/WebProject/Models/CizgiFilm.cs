namespace WebProject.Models
{
    public class CizgiFilm
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Tur { get; set; } = string.Empty;
        public int BolumSayisi { get; set; }
        public string YasAraligi { get; set; } = string.Empty;
    }
}
