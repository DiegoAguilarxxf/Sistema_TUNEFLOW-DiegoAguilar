using Dapper;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Models;
using Npgsql;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADSController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ADSController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/ADS
        [HttpGet]
        public IActionResult Get()
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("TUNEFLOWContext"));
            var sql = "SELECT * FROM \"ADS\" ORDER BY RANDOM()";
            var result = connection.Query<ADS>(sql).ToList();
            return Ok(result);
        }

        // GET: api/ADS/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("TUNEFLOWContext"));
            var sql = "SELECT * FROM \"ADS\" WHERE \"Id\" = @Id";
            var anuncio = connection.QueryFirstOrDefault<ADS>(sql, new { Id = id });

            if (anuncio == null)
                return NotFound();

            return Ok(anuncio);
        }

        // POST: api/ADS
        [HttpPost]
        public IActionResult Post([FromBody] ADS nuevoAnuncio)
        {
            if (nuevoAnuncio == null)
                return BadRequest("Datos inválidos.");

            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("TUNEFLOWContext"));

            var sql = @"
                INSERT INTO ""ADS"" (""Title"", ""Duration"", ""FilePath"", ""ImagePath"")
                VALUES (@Title, @Duration, @FilePath, @ImagePath);
            ";

            var rowsAffected = connection.Execute(sql, nuevoAnuncio);

            if (rowsAffected > 0)
                return Ok(nuevoAnuncio);
            else
                return StatusCode(500, "No se pudo insertar el anuncio.");
        }

        // PUT: api/ADS/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ADS anuncio)
        {
            if (anuncio == null || anuncio.Id != id)
                return BadRequest("Datos inválidos.");

            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("TUNEFLOWContext"));

            var sql = @"
                UPDATE ""ADS""
                SET ""Title"" = @Title,
                    ""Duration"" = @Duration,
                    ""FilePath"" = @FilePath,
                    ""ImagePath"" = @ImagePath
                WHERE ""Id"" = @Id;
            ";

            var rowsAffected = connection.Execute(sql, anuncio);

            if (rowsAffected > 0)
                return Ok(anuncio);
            else
                return NotFound("Anuncio no encontrado.");
        }

        // DELETE: api/ADS/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("TUNEFLOWContext"));

            var sql = "DELETE FROM \"ADS\" WHERE \"Id\" = @Id";
            var rowsAffected = connection.Execute(sql, new { Id = id });

            if (rowsAffected > 0)
                return Ok("Anuncio eliminado.");
            else
                return NotFound("Anuncio no encontrado.");
        }
    }
}
