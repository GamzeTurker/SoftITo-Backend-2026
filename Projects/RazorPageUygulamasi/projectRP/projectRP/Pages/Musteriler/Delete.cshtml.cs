using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Musteriler
{
    public class DeleteModel : PageModel
    {
        public Musteriler musteriBilgi = new Musteriler();
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
                    string sql = "select *from Musteriler where ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            musteriBilgi.ID = reader.GetInt32(0);
                            musteriBilgi.AdSoyad = reader.GetString(1);
                            musteriBilgi.Eposta = reader.GetString(2);
                            musteriBilgi.Telefon = reader.GetString(3);
                            musteriBilgi.Sehir = reader.GetString(4);
                            musteriBilgi.Notlar = reader.GetString(5);



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
                string sql = "delete from Musteriler where ID=@ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);
                    command.ExecuteNonQuery();
                }
            }



            Response.Redirect("/Musteriler/Index");
        }

    }
}
