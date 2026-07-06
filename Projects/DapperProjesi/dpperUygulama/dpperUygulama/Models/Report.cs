namespace dpperUygulama.Models
{
    public class ReportModel
    {
        // 1 - Toplam servis + gelir
        public int TotalService { get; set; }
        public decimal TotalIncome { get; set; }

        // 2 - En çok servis alan araç
        public string Brand { get; set; }
        public string Plate { get; set; }
        public int ServiceCount { get; set; }

        // 3 - En aktif personel
        public string EmployeeName { get; set; }

        // 4 - Müşteri bazlı (istersen kullan)
        public string FullName { get; set; }
    }
}
