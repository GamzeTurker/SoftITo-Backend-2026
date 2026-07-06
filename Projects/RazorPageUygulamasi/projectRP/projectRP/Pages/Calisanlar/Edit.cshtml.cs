using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectRP.Pages.Yoneticiler;
using System.Data.SqlClient;

namespace projectRP.Pages.Calisanlar
{
    public class EditModel : PageModel
    {
        public Calisanlar calisanBilgi = new Calisanlar();
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





        public void OnPost()
        {

            calisanBilgi.ID =Convert.ToInt32(Request.Form["ID"]);
            calisanBilgi.AdSoyad = Request.Form["AdSoyad"];
            calisanBilgi.Departman = Request.Form["Departman"];
            calisanBilgi.Pozisyon = Request.Form["Pozisyon"];
            calisanBilgi.Maas = Convert.ToDecimal(Request.Form["Maas"]);
            calisanBilgi.Telefon = Request.Form["Telefon"];


            if (calisanBilgi.AdSoyad.Length == 0 || calisanBilgi.Departman.Length == 0 ||
                calisanBilgi.Pozisyon.Length == 0 || string.IsNullOrWhiteSpace(Request.Form["Maas"]) ||
                calisanBilgi.Telefon.Length == 0
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
                    string Sql = "UPDATE Calisanlar SET " +
                     "AdSoyad=@AdSoyad, " +
                     "Departman=@Departman, " +
                     "Pozisyon=@Pozisyon, " +
                     "Maas=@Maas, " +
                     "Telefon=@Telefon " +
                     "WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdSoyad", calisanBilgi.AdSoyad);
                        command.Parameters.AddWithValue("@Departman", calisanBilgi.Departman);
                        command.Parameters.AddWithValue("@Pozisyon", calisanBilgi.Pozisyon);
                        command.Parameters.AddWithValue("@Maas", calisanBilgi.Maas);
                        command.Parameters.AddWithValue("@Telefon", calisanBilgi.Telefon);

                        command.Parameters.AddWithValue("@ID", calisanBilgi.ID);
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
            Response.Redirect("/Calisanlar/Index");
        }
    }
}
