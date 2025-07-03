using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Usuario.Consumidor;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposSuscripcionesController : ControllerBase
    {
        /* private readonly TUNEFLOWContext _context;

         public TiposSuscripcionesController(TUNEFLOWContext context)
         {
             _context = context;
         }
        */
        private DbConnection connection;
        public TiposSuscripcionesController(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }
        // GET: api/TiposSuscripciones
        [HttpGet]
        public IEnumerable<TipoSuscripcion> GetTipoSuscripcion()
        {
            var tiposSuscripciones = connection.Query<TipoSuscripcion>("SELECT * FROM \"TiposSuscripciones\"");
            return tiposSuscripciones;
        }

        // GET: api/TiposSuscripciones/5
        [HttpGet("{id}")]
        public ActionResult<TipoSuscripcion> GetTipoSuscripcion(int id)
        {
            var tipoSuscripcion = connection.QuerySingle<TipoSuscripcion>(@"SELECT * FROM ""TiposSuscripciones"" WHERE ""Id"" = @Id", new { Id = id });

            if (tipoSuscripcion == null)
            {
                return NotFound();
            }

            return tipoSuscripcion;
        }

        // PUT: api/TiposSuscripciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutTipoSuscripcion(int id,[FromBody] TipoSuscripcion tipoSuscripcion)
        {
            connection.Execute(@"UPDATE ""TiposSuscripciones"" SET 
                ""Nombre"" = @Nombre,
                ""Precio"" = @Precio,
                ""LimiteMiembros"" = @LimiteMiembros,
                WHERE ""Id"" = @Id", new
            {
                Id = id,
                Nombre = tipoSuscripcion.Nombre,
                Precio = tipoSuscripcion.Precio,
                LimiteMiembros= tipoSuscripcion.LimiteMiembros
            });
        }

        // POST: api/TiposSuscripciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public TipoSuscripcion PostTipoSuscripcion([FromBody]TipoSuscripcion tipoSuscripcion)
        {
           connection.Execute(@"INSERT INTO ""TiposSuscripciones"" (""Nombre"", ""Precio"", ""LimiteMiembros"") 
                VALUES (@Nombre, @Precio, @LimiteMiembros)", new
           {
               Nombre = tipoSuscripcion.Nombre,
               Precio = tipoSuscripcion.Precio,
               LimiteMiembros = tipoSuscripcion.LimiteMiembros
           });
            return tipoSuscripcion;
        }

        // DELETE: api/TiposSuscripciones/5
        [HttpDelete("{id}")]
        public void DeleteTipoSuscripcion(int id)
        {
           connection.Execute(@"DELETE FROM ""TiposSuscripciones"" WHERE ""Id"" = @Id", new { Id = id });
         
        }
        /*
        private bool TipoSuscripcionExists(int id)
        {
            return _context.TiposSuscripciones.Any(e => e.Id == id);
        }*/
    }
}
