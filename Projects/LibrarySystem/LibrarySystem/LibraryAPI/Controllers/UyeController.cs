using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UyeController : ControllerBase
    {
        private readonly string _connectionString;

        public UyeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SearchTerm", search);

                var uyeler = await connection.QueryAsync<Uye>(
                    "sp_GetUyeler", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );
                return Ok(uyeler);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var uye = await connection.QueryFirstOrDefaultAsync<Uye>(
                    "sp_GetUyeById", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                if (uye == null)
                    return NotFound();

                return Ok(uye);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Uye model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AdSoyad", model.AdSoyad);
                parameters.Add("@Eposta", model.Eposta);
                parameters.Add("@Telefon", model.Telefon);
                parameters.Add("@KayitTarihi", model.KayitTarihi);

                var id = await connection.ExecuteScalarAsync<int>(
                    "sp_InsertUye", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                model.Id = id;
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Uye model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@AdSoyad", model.AdSoyad);
                parameters.Add("@Eposta", model.Eposta);
                parameters.Add("@Telefon", model.Telefon);
                parameters.Add("@KayitTarihi", model.KayitTarihi);

                await connection.ExecuteAsync(
                    "sp_UpdateUye", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                await connection.ExecuteAsync(
                    "sp_DeleteUye", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                return NoContent();
            }
        }
    }
}
