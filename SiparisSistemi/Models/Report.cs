namespace SiparisSistemi.Models
{
    public class Report
    {
        // Sipariş ve Müşteri Alanları
        public int SiparisId { get; set; }
        public string MusteriAdSoyad { get; set; }
        public string Telefon { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string SiparisDurumu { get; set; }
        // Ürün Alanları
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
        public int Stok { get; set; }
        // Group By Analiz Alanları
        public int ToplamSatisAdeti { get; set; }
        public decimal ToplamCiro { get; set; }
    }
}
