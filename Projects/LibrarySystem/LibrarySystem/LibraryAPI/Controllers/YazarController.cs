using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YazarController : ControllerBase
    {
        private readonly string _connectionString;

        public YazarController(IConfiguration configuration)
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

                var yazarlar = await connection.QueryAsync<Yazar>(
                    "sp_GetYazarlar", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );
                return Ok(yazarlar);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var yazar = await connection.QueryFirstOrDefaultAsync<Yazar>(
                    "sp_GetYazarById", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                if (yazar == null)
                    return NotFound();

                return Ok(yazar);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Yazar model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AdSoyad", model.AdSoyad);
                parameters.Add("@Biyografi", model.Biyografi);
                parameters.Add("@DogumTarihi", model.DogumTarihi);

                var id = await connection.ExecuteScalarAsync<int>(
                    "sp_InsertYazar", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                model.Id = id;
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Yazar model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@AdSoyad", model.AdSoyad);
                parameters.Add("@Biyografi", model.Biyografi);
                parameters.Add("@DogumTarihi", model.DogumTarihi);

                await connection.ExecuteAsync(
                    "sp_UpdateYazar", 
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
                    "sp_DeleteYazar", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                return NoContent();
            }
        }
    }
}
