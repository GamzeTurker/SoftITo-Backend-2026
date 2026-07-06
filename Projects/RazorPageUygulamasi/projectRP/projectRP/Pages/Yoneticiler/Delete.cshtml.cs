using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Yoneticiler
{
    public class DeleteModel : PageModel
    {
        public Yoneticiler yoneticiBilgi = new Yoneticiler();
        public void OnGet()
        {

            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;" +
                "Integrated Security=true;TrustServerCertificate=true;";

            string ID = Request.Query["ID"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string sql = "select *from Yoneticiler where ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            yoneticiBilgi.ID = reader.GetInt32(0);
                            yoneticiBilgi.AdSoyad = reader.GetString(1);
                            yoneticiBilgi.Eposta = reader.GetString(2);
                            yoneticiBilgi.Telefon = reader.GetString(3);
                      



                        }

                    }
                }
            }
            catch { }
        }
        public void OnPost()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;" +
                        "Integrated Security=true;TrustServerCertificate=true;";

            string ID = Request.Query["ID"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                string sql = "delete from Yoneticiler where ID=@ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);
                    command.ExecuteNonQuery();
                }
            }



            Response.Redirect("/Yoneticiler/Index");
        }

    }
}
