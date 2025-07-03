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
    public class SuscripcionesController : ControllerBase
    {
        private DbConnection connection;
        /*private readonly TUNEFLOWContext _context;

        public SuscripcionesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        public SuscripcionesController(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Suscripciones
        [HttpGet]
        public IEnumerable<Suscripcion> GetSuscripcion()
        { var suscripciones = connection.Query<Suscripcion>("SELECT * FROM \"Suscripciones\"");
            return suscripciones;
        }

        // GET: api/Suscripciones/5
        [HttpGet("{id}")]
        public ActionResult<Suscripcion> GetSuscripcion(int id)
        {
            var suscripcion = connection.QuerySingle<Suscripcion>(@"SELECT * FROM ""Suscripciones"" WHERE ""Id"" = @Id", new { Id = id });

            if (suscripcion == null)
            {
                return NotFound();
            }

            return suscripcion;
        }

        // PUT: api/Suscripciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutSuscripcion(int id, [FromBody] Suscripcion suscripcion)
        {
            connection.Execute( @"UPDATE ""Suscripciones"" SET 
                ""FechaInicio"" = @FechaInicio,
                ""FechaFin"" = @FechaFin,
                ""CodigoUnion"" = @CodigoUnion,
                ""TipoSuscripcion"" = @TipoSuscripcion,
            WHERE ""Id"" = @Id",
             new
             {
                 Id = id,
                 FechaInicio = suscripcion.FechaInicio,
                 FechaFin = suscripcion.FechaFin,
                 CodigoUnion = suscripcion.CodigoUnion,
                 TipoSuscripcion = suscripcion.TipoSuscripcion
             }

  );

           }

        // POST: api/Suscripciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Suscripcion PostSuscripcion([FromBody] Suscripcion suscripcion)
        {
            connection.Execute(@"INSERT INTO ""Suscripciones"" (""FechaInicio"", ""FechaFin"", ""CodigoUnion"", ""TipoSuscripcion"") VALUES (@FechaInicio, @FechaFin, @CodigoUnion, @TipoSuscripcion)", new
            {
                FechaInicio = suscripcion.FechaInicio,
                FechaFin = suscripcion.FechaFin,
                CodigoUnion = suscripcion.CodigoUnion,
                TipoSuscripcion = suscripcion.TipoSuscripcion
            });

            return suscripcion;

        }
        // DELETE: api/Suscripciones/5
        [HttpDelete("{id}")]
        public void DeleteSuscripcion(int id)
        {
          connection.Execute(@"DELETE FROM ""Suscripciones"" WHERE ""Id"" = @Id", new { Id = id });
        }
/*
        private bool SuscripcionExists(int id)
        {
            return _context.Suscripciones.Any(e => e.Id == id);
        }*/
    }
}
