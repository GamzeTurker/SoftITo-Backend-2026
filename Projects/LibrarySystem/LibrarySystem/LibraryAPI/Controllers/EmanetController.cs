using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmanetController : ControllerBase
    {
        private readonly string _connectionString;

        public EmanetController(IConfiguration configuration)
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

                var emanetler = await connection.QueryAsync<Emanet>(
                    "sp_GetEmanetler", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );
                return Ok(emanetler);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var emanet = await connection.QueryFirstOrDefaultAsync<Emanet>(
                    "sp_GetEmanetById", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                if (qp_is_null(emanet))
                    return NotFound();

                return Ok(emanet);
            }
        }

        private bool qp_is_null(object? obj) => obj == null;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Emanet model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@KitapId", model.KitapId);
                parameters.Add("@UyeId", model.UyeId);
                parameters.Add("@EmanetTarihi", model.EmanetTarihi);
                parameters.Add("@TeslimTarihi", model.TeslimTarihi);
                parameters.Add("@Durum", model.Durum);

                var id = await connection.ExecuteScalarAsync<int>(
                    "sp_InsertEmanet", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                model.Id = id;
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Emanet model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@KitapId", model.KitapId);
                parameters.Add("@UyeId", model.UyeId);
                parameters.Add("@EmanetTarihi", model.EmanetTarihi);
                parameters.Add("@TeslimTarihi", model.TeslimTarihi);
                parameters.Add("@Durum", model.Durum);

                await connection.ExecuteAsync(
                    "sp_UpdateEmanet", 
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
                    "sp_DeleteEmanet", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                return NoContent();
            }
        }
    }
}
