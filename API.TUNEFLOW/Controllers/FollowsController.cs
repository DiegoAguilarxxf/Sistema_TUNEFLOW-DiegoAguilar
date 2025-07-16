using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Usuario.Produccion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
       /* private readonly TUNEFLOWContext _context;

        public SeguimientosController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
       private DbConnection connection;
        public FollowsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Seguimientos
        [HttpGet]
        public IEnumerable<Seguimiento> GetSeguimiento()
        { var seguimientos = connection.Query<Seguimiento>("SELECT * FROM \"Seguimientos\"");
            return seguimientos;
        }

        // GET: api/Seguimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Seguimiento>> GetSeguimiento(int id)
        {
            var seguimiento = connection.QuerySingle<Seguimiento>(@"SELECT * FROM ""Seguimientos"" WHERE ""Id"" = @Id", new { Id = id });

            if (seguimiento == null)
            {
                return NotFound();
            }

            return seguimiento;
        }

        // PUT: api/Seguimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutSeguimiento(int id,[FromBody] Seguimiento seguimiento)
        {
            connection.Execute(@"UPDATE ""Seguimientos"" SET
                ""ClienteId""=@ClienteId,
                ""ArtistaId""=@ArtistaId WERE ""Id""=@Id", new
            {
                ClienteId = seguimiento.ClienteId,
                ArtistaId = seguimiento.ArtistaId
            });

        }

        // POST: api/Seguimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Seguimiento PostSeguimiento([FromBody]Seguimiento seguimiento)
        {
            connection.Execute(@"INSERT INTO ""Seguimientos"" (""ClienteId"", ""ArtistaId"") VALUES (@ClienteId, @ArtistaId)", new
            {
                ClienteId = seguimiento.ClienteId,
                ArtistaId = seguimiento.ArtistaId
            });

            return seguimiento;
        }

        // DELETE: api/Seguimientos/5
        [HttpDelete("{id}")]
        public void DeleteSeguimiento(int id)
        {connection.Execute(@"DELETE FROM ""Seguimientos"" WHERE ""Id"" = @Id", new { Id = id });
            
        }
        /*
        private bool SeguimientoExists(int id)
        {
            return _context.Seguimientos.Any(e => e.Id == id);
        }*/
    }
}
