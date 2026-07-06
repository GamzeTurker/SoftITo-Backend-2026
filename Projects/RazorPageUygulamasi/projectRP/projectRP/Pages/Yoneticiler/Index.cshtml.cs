using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Yoneticiler
{
    public class IndexModel : PageModel
    {
        [BindProperty]//yoneticiler sınıfındaki verilere tek tek öznitelik yani at
        //tanımlamak yere yoneticiler sınıfdaki kolonlara direk erişsin istedim
        public List<Yoneticiler> listele { get; set; } = new List<Yoneticiler>();
        public void OnGet()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;TrustServerCertificate=true";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from Yoneticiler";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Yoneticiler yoneticiler = new Yoneticiler
                                {
                                    ID = reader.GetInt32(0),
                                    AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Eposta = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Telefon = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                  

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
