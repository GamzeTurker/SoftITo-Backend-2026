namespace OkulNotSistemi.Models
{
    public class Reports
    {
        // JOIN RAPOR
        public int NotId { get; set; }
        public string OgrenciAdSoyad { get; set; }
        public string DersAd { get; set; }
        public decimal? Vize { get; set; }
        public decimal? Final { get; set; }
        public decimal? Ortalama { get; set; }
        public string Durum { get; set; }

        // GROUP BY
        public string Sinif { get; set; }
        public int OgrenciSayisi { get; set; }
        public decimal? SinifOrtalamasi { get; set; }
    }
}