using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Musteriler
{
    public class IndexModel : PageModel
    {
        [BindProperty]//yoneticiler sınıfındaki verilere tek tek öznitelik yani at
        //tanımlamak yere yoneticiler sınıfdaki kolonlara direk erişsin istedim
        public List<Musteriler> listele { get; set; } = new List<Musteriler>();
        public void OnGet()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;TrustServerCertificate=true";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from Musteriler";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Musteriler musteriler = new Musteriler
                                {
                                    ID = reader.GetInt32(0),
                                    AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Eposta = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Telefon = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Sehir = reader.IsDBNull(3) ? "" : reader.GetString(4),
                                    Notlar = reader.IsDBNull(5) ? "" : reader.GetString(5)

                                };
                                listele.Add(musteriler);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
