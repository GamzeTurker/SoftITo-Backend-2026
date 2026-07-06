using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace dpperUygulama.Models
{
    public class Context
    {
        public static string connectionstring = @"Server=(localdb)\MSSQLLocalDB;Database=AracServis;Integrated Security=true;";
        public static void ExecuteReturn(string procadi, DynamicParameters param = null)
        {

            using (SqlConnection db = new SqlConnection(connectionstring))
            {

                db.Open();
                db.Execute(procadi, param, commandType: System.Data.CommandType.StoredProcedure);


            }



        }


        public static IEnumerable<T> Listeleme<T>(string procadi, DynamicParameters param = null)
        {

            using (SqlConnection db = new SqlConnection(connectionstring))
            {

                db.Open();
                return db.Query<T>(procadi, param, commandType: CommandType.StoredProcedure);


            }



        }









    }
}
