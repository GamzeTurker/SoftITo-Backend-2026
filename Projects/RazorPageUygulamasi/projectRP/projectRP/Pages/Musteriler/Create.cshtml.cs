using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Musteriler
{
    public class CreateModel : PageModel
    {
        public Musteriler musteriBilgi = new Musteriler();
        public string errorMessage = "";
        public string successMessage = "";


        public void OnGet()
        {
        }

        public void OnPost()
        {
            musteriBilgi.AdSoyad = Request.Form["AdSoyad"];
            musteriBilgi.Eposta = Request.Form["Eposta"];
            musteriBilgi.Telefon = Request.Form["Telefon"];
            musteriBilgi.Sehir = Request.Form["Sehir"];
            musteriBilgi.Notlar = Request.Form["Notlar"];

            if (musteriBilgi.AdSoyad.Length == 0 || musteriBilgi.Eposta.Length == 0 ||
                musteriBilgi.Telefon.Length == 0 || musteriBilgi.Sehir.Length == 0 ||
                musteriBilgi.Notlar.Length == 0)
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
                    string Sql = "insert into Musteriler (AdSoyad,Eposta,Telefon,Sehir,Notlar)" +
                        "values(@AdSoyad,@Eposta,@Telefon,@Sehir,@Notlar)";
                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdSoyad", musteriBilgi.AdSoyad);
                        command.Parameters.AddWithValue("@Eposta", musteriBilgi.Eposta);
                        command.Parameters.AddWithValue("@Telefon", musteriBilgi.Telefon);
                        command.Parameters.AddWithValue("@Sehir", musteriBilgi.Sehir);
                        command.Parameters.AddWithValue("@Notlar", musteriBilgi.Notlar);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Kayıt başarılı";
            Response.Redirect("/Musteriler/Index");
        }
    }
}