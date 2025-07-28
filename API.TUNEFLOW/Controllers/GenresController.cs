using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Modelos.Tuneflow.Models;
using Dapper;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IConfiguration _config;

        public GenresController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/Genres
        [HttpGet]
        public IActionResult GetAll()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            var generos = connection.Query<Genre>("SELECT * FROM \"Genres\" ORDER BY \"Id\" ASC");
            return Ok(generos);
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            var sql = "SELECT * FROM \"Genres\" WHERE \"Id\" = @Id";
            var genero = connection.QueryFirstOrDefault<Genre>(sql, new { Id = id });

            if (genero == null)
                return NotFound();

            return Ok(genero);
        }

        // POST: api/Genres
        [HttpPost]
        public IActionResult Create([FromBody] Genre nuevoGenero)
        {
            if (nuevoGenero == null || string.IsNullOrWhiteSpace(nuevoGenero.Name))
                return BadRequest("Datos inválidos.");

            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));

            var sql = @"
                INSERT INTO ""Genres"" (""Name"", ""FilePath"")
                VALUES (@Name, @FilePath);
            ";

            var filas = connection.Execute(sql, nuevoGenero);

            if (filas > 0)
                return Ok(nuevoGenero);
            else
                return StatusCode(500, "No se pudo insertar el género.");
        }

        // PUT: api/Genres/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Genre generoActualizado)
        {
            if (generoActualizado == null || generoActualizado.Id != id)
                return BadRequest("Datos inválidos.");

            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));

            var sql = @"
                UPDATE ""Genres""
                SET ""Name"" = @Name,
                    ""FilePath"" = @FilePath
                WHERE ""Id"" = @Id;
            ";

            var filas = connection.Execute(sql, generoActualizado);

            if (filas > 0)
                return Ok(generoActualizado);
            else
                return NotFound("Género no encontrado.");
        }

        // DELETE: api/Genres/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));

            var sql = "DELETE FROM \"Genres\" WHERE \"Id\" = @Id";
            var filas = connection.Execute(sql, new { Id = id });

            if (filas > 0)
                return Ok("Género eliminado.");
            else
                return NotFound("Género no encontrado.");
        }
    }
}
