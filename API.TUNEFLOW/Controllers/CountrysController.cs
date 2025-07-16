using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Modelos;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountrysController : ControllerBase
    {
       /* private readonly TUNEFLOWContext _context;

        public PaisesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
       private DbConnection connection;
        public CountrysController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Paises
        [HttpGet]
        public IEnumerable<Pais> GetPais()
        {
            var paises = connection.Query<Pais>("SELECT * FROM \"Paises\"");
            return paises;
        }

        // GET: api/Paises/5
        [HttpGet("{id}")]
        public ActionResult<Pais> GetPais(int id)
        {
            var pais = connection.QuerySingle<Pais>(@"SELECT * FROM ""Paises"" WHERE ""Id"" = @Id", new { Id = id });

            if (pais == null)
            {
                return NotFound();
            }

            return pais;
        }

        // PUT: api/Paises/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutPais(int id,[FromBody] Pais pais)
        {
            connection.Execute(@"UPDATE ""Paises"" SET 
              ""Name""= @Name,
              "" Continente""= @Continente,
              ""Moneda""= @Moneda
            WHERE ""Id"" = @Id", new { Id = id, Name = pais.Name, Continente = pais.Continente, Moneda=pais.Moneda });
        }

        // POST: api/Paises
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Pais PostPais([FromBody]Pais pais)
        {
           connection.Execute(@"INSERT INTO ""Paises"" (""Name"", ""Continente"", ""Moneda"") 
            VALUES (@Name, @Continente, @Moneda)", new { Name = pais.Name, Continente = pais.Continente, Moneda = pais.Moneda });
            return pais;
        }

        // DELETE: api/Paises/5
        [HttpDelete("{id}")]
        public void DeletePais(int id)
        {
            connection.Execute(@"DELETE FROM ""Paises"" WHERE ""Id"" = @Id", new { Id = id });
        }
        /*
        private bool PaisExists(int id)
        {
            return _context.Paises.Any(e => e.Id == id);
        }*/
    }
}
