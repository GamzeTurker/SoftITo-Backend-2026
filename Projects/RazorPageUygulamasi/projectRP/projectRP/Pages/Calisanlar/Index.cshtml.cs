using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Calisanlar
{
    public class IndexModel : PageModel
    {
        [BindProperty]//yoneticiler sınıfındaki verilere tek tek öznitelik yani at
        //tanımlamak yere yoneticiler sınıfdaki kolonlara direk erişsin istedim
        public List<Calisanlar> listele { get; set; } = new List<Calisanlar>();
        public void OnGet()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;TrustServerCertificate=true";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from Calisanlar";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Calisanlar yoneticiler = new Calisanlar
                                {
                                    ID = reader.GetInt32(0),
                                    AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Departman = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Pozisyon = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Maas = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                                    Telefon = reader.IsDBNull(5) ? "" : reader.GetString(5)

                                };
                                listele.Add(yoneticiler);
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
