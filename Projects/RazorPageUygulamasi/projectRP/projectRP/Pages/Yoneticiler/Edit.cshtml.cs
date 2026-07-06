using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Yoneticiler
{
    public class EditModel : PageModel
    {
        public Yoneticiler yoneticiBilgi = new Yoneticiler();
        public string errorMessage = "";
        public string successMessage = "";
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

            yoneticiBilgi.ID = Convert.ToInt32(Request.Form["ID"]);
            yoneticiBilgi.AdSoyad = Request.Form["AdSoyad"];
            yoneticiBilgi.Eposta = Request.Form["Eposta"];
            yoneticiBilgi.Telefon = Request.Form["Telefon"];
         

            if (yoneticiBilgi.AdSoyad.Length == 0 || yoneticiBilgi.Eposta.Length == 0 ||
                yoneticiBilgi.Telefon.Length == 0
            )
            {

                errorMessage = "tüm alanlar zorunludur";
                return;

            }
            try
            {

                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Sistemi;" +
                   "Integrated Security=true;TrustServerCertificate=true;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string Sql = "UPDATE Yoneticiler SET " +
                     "AdSoyad=@AdSoyad, " +
                     "Eposta=@Eposta, " +
                     "Telefon=@Telefon, " +
                     "WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdSoyad", yoneticiBilgi.AdSoyad);
                        command.Parameters.AddWithValue("@Eposta", yoneticiBilgi.Eposta);
                        command.Parameters.AddWithValue("@Telefon", yoneticiBilgi.Telefon);
                      

                        command.Parameters.AddWithValue("@ID", yoneticiBilgi.ID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Kayıut başarılı";
            Response.Redirect("/Yoneticiler/Index");
        }
    }
}
