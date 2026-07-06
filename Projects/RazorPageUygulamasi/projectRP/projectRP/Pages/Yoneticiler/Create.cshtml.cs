using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace projectRP.Pages.Yoneticiler
{
    public class CreateModel : PageModel
    {
        public Yoneticiler yoneticiBilgi = new Yoneticiler();
        public string errorMessage = "";
        public string successMessage = "";


        public void OnGet()
        {
        }

        public void OnPost()
        {
            yoneticiBilgi.AdSoyad = Request.Form["AdSoyad"];
            yoneticiBilgi.Eposta = Request.Form["Eposta"];
            yoneticiBilgi.Telefon = Request.Form["Telefon"];
          

            if (yoneticiBilgi.AdSoyad.Length == 0 || yoneticiBilgi.Eposta.Length == 0 ||
                yoneticiBilgi.Telefon.Length == 0 )
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
                    string Sql = "insert into Yoneticiler (AdSoyad,Eposta,Telefon)" +
                        "values(@AdSoyad,@Eposta,@Telefon)";
                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdSoyad", yoneticiBilgi.AdSoyad);
                        command.Parameters.AddWithValue("@Eposta", yoneticiBilgi.Eposta);
                        command.Parameters.AddWithValue("@Telefon", yoneticiBilgi.Telefon);
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
            Response.Redirect("/Yoneticiler/Index");
        }
    }
}