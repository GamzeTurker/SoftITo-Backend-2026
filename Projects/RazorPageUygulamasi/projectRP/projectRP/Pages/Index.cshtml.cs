using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages
{
    public class IndexModel : PageModel
    {
        public int YoneticiSayisi { get; set; }
        public int CalisanSayisi { get; set; }
        public int MusteriSayisi { get; set; }

        public void OnGet()
        {
            string connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;Integrated Security=true;TrustServerCertificate=true;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Yönetici sayısı
                SqlCommand komut1 = new SqlCommand("SELECT COUNT(*) FROM Yoneticiler", connection);
                YoneticiSayisi = (int)komut1.ExecuteScalar();

                // Çalışan sayısı
                SqlCommand komut2 = new SqlCommand("SELECT COUNT(*) FROM Calisanlar", connection);
                CalisanSayisi = (int)komut2.ExecuteScalar();

                // Müşteri sayısı
                SqlCommand komut3 = new SqlCommand("SELECT COUNT(*) FROM Musteriler", connection);
                MusteriSayisi = (int)komut3.ExecuteScalar();
            }
        }
    }
}
