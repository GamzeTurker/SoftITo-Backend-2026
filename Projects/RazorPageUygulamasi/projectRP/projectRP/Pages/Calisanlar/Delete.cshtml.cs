using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Calisanlar
{
    public class DeleteModel : PageModel
    {
        public Calisanlar calisanBilgi = new Calisanlar();
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
                    string sql = "select *from Calisanlar where ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            calisanBilgi.ID = reader.GetInt32(0);
                            calisanBilgi.AdSoyad = reader.GetString(1);
                            calisanBilgi.Departman = reader.GetString(2);
                            calisanBilgi.Pozisyon = reader.GetString(3);
                            calisanBilgi.Maas = reader.GetDecimal(4);
                            calisanBilgi.Telefon = reader.GetString(5);



                        }

                    }
                }
            }
            catch { }
        }
        public void OnPost() {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;" +
                        "Integrated Security=true;TrustServerCertificate=true;";

            string ID = Request.Query["ID"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                string sql = "delete from Calisanlar where ID=@ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);
                    command.ExecuteNonQuery();
                }
            }



            Response.Redirect("/Calisanlar/Index");
        }

    }
}
