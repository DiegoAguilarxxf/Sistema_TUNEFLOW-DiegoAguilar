using Dapper;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Models;
using Npgsql;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // GET: api/<ADSController>
        [HttpGet]
        public IEnumerable<ADS> Get()
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("TUNEFLOWContext"));
            connection.Open();

            var sql = "SELECT * FROM \"ADS\" ORDER BY RANDOM()";
            var anuncios = connection.Query<ADS>(sql).ToList();
            return anuncios;
        }
    }
}
