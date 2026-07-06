using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitapController : ControllerBase
    {
        private readonly string _connectionString;

        public KitapController(IConfiguration configuration)
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

                var kitaplar = await connection.QueryAsync<Kitap>(
                    "sp_GetKitaplar", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );
                return Ok(kitaplar);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var kitap = await connection.QueryFirstOrDefaultAsync<Kitap>(
                    "sp_GetKitapById", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                if (kitap == null)
                    return NotFound();

                return Ok(kitap);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Kitap model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@YazarId", model.YazarId);
                parameters.Add("@Baslik", model.Baslik);
                parameters.Add("@ISBN", model.ISBN);
                parameters.Add("@BasimYili", model.BasimYili);
                parameters.Add("@SayfaSayisi", model.SayfaSayisi);

                var id = await connection.ExecuteScalarAsync<int>(
                    "sp_InsertKitap", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                model.Id = id;
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Kitap model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@YazarId", model.YazarId);
                parameters.Add("@Baslik", model.Baslik);
                parameters.Add("@ISBN", model.ISBN);
                parameters.Add("@BasimYili", model.BasimYili);
                parameters.Add("@SayfaSayisi", model.SayfaSayisi);

                await connection.ExecuteAsync(
                    "sp_UpdateKitap", 
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
                    "sp_DeleteKitap", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                return NoContent();
            }
        }
    }
}
