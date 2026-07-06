using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Musteriler
{
    public class EditModel : PageModel
    {
        public Musteriler musteriBilgi = new Musteriler();
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

            musteriBilgi.ID = Convert.ToInt32(Request.Form["ID"]);
            musteriBilgi.AdSoyad = Request.Form["AdSoyad"];
            musteriBilgi.Eposta = Request.Form["Eposta"];
            musteriBilgi.Telefon = Request.Form["Telefon"];
            musteriBilgi.Sehir = Request.Form["Sehir"];
            musteriBilgi.Notlar = Request.Form["Notlar"];


            if (musteriBilgi.AdSoyad.Length == 0 || musteriBilgi.Eposta.Length == 0 ||
                musteriBilgi.Telefon.Length == 0 || musteriBilgi.Sehir.Length == 0 ||
                musteriBilgi.Notlar.Length == 0
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
                    string Sql = "UPDATE Musteriler SET " +
                     "AdSoyad=@AdSoyad, " +
                     "Eposta=@Eposta, " +
                     "Telefon=@Telefon, " +
                     "Sehir=@Sehir, " +
                     "Notlar=@Notlar " +
                     "WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdSoyad", musteriBilgi.AdSoyad);
                        command.Parameters.AddWithValue("@Eposta", musteriBilgi.Eposta);
                        command.Parameters.AddWithValue("@Telefon", musteriBilgi.Telefon);
                        command.Parameters.AddWithValue("@Sehir", musteriBilgi.Sehir);
                        command.Parameters.AddWithValue("@Notlar", musteriBilgi.Notlar);

                        command.Parameters.AddWithValue("@ID", musteriBilgi.ID);
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
            Response.Redirect("/Musteriler/Index");
        }
    }
}
