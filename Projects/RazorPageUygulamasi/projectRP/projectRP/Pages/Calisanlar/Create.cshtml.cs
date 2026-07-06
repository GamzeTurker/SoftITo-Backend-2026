using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Data.SqlClient;

namespace projectRP.Pages.Calisanlar
{
    public class CreateModel : PageModel
    {
        public Calisanlar calisanBilgi = new Calisanlar();
        public string errorMessage = "";
        public string successMessage = "";


        public void OnGet()
        {
        }

        public void OnPost()
        {
            calisanBilgi.AdSoyad = Request.Form["AdSoyad"];
            calisanBilgi.Departman = Request.Form["Departman"];
            calisanBilgi.Pozisyon = Request.Form["Pozisyon"];
            calisanBilgi.Maas = Convert.ToDecimal(Request.Form["Maas"]);
            calisanBilgi.Telefon = Request.Form["Telefon"];

            if (calisanBilgi.AdSoyad.Length == 0 || calisanBilgi.Departman.Length == 0 ||
                calisanBilgi.Pozisyon.Length == 0 || calisanBilgi.Maas == 0 ||
                calisanBilgi.Telefon.Length == 0)
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
                    string Sql = "insert into Calisanlar (AdSoyad,Departman,Pozisyon,Maas,Telefon)" +
                        "values(@AdSoyad,@Departman,@Pozisyon,@Maas,@Telefon)";
                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdSoyad", calisanBilgi.AdSoyad);
                        command.Parameters.AddWithValue("@Departman", calisanBilgi.Departman);
                        command.Parameters.AddWithValue("@Pozisyon", calisanBilgi.Pozisyon);
                        command.Parameters.AddWithValue("@Maas", calisanBilgi.Maas);
                        command.Parameters.AddWithValue("@Telefon", calisanBilgi.Telefon);
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
            Response.Redirect("/Calisanlar/Index");
        }
    }
}