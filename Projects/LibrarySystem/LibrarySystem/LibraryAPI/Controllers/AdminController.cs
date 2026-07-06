using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly string _connectionString;

        public AdminController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Admin loginModel)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Username", loginModel.Username);
                parameters.Add("@Password", loginModel.Password);

                var admin = await connection.QueryFirstOrDefaultAsync<Admin>(
                    "sp_ValidateAdmin", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                if (admin != null)
                {
                    return Ok(admin);
                }
            }

            return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı." });
        }
    }
}
