using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Modelos.Tuneflow.Models;
using Dapper;
using Modelos.Tuneflow.User.Production;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<GenresController>
        [HttpGet]
        public IEnumerable<Genre> GetGenerosMusicales()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();

            var generos= connection.Query<Genre>("SELECT * FROM \"Genres\"");

            return generos;
        }

        // GET api/<GenresController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<GenresController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GenresController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GenresController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
