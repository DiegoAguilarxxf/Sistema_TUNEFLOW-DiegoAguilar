using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Media;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaybacksController : ControllerBase
    {
        /* private readonly TUNEFLOWContext _context;

         public ReproduccionesController(TUNEFLOWContext context)
         {
             _context = context;
         }*/
        private DbConnection connection;
        public PlaybacksController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Reproducciones
        [HttpGet]
        public IEnumerable<Reproduccion> GetReproduccion()
        { var reproducciones = connection.Query<Reproduccion>("SELECT * FROM \"Reproducciones\"");
            return reproducciones;
        }

        // GET: api/Reproducciones/5
        [HttpGet("{id}")]
        public ActionResult<Reproduccion> GetReproduccion(int id)
        {
            var reproduccion = connection.QuerySingle<Reproduccion>(@"SELECT * FROM ""Reproducciones"" WHERE ""Id"" = @Id", new { Id = id });

            if (reproduccion == null)
            {
                return NotFound();
            }

            return reproduccion;
        }

        // PUT: api/Reproducciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void Reproduccion(int id,[FromBody] Reproduccion reproduccion)
        {
            connection.Execute(@"UPDATE ""Reproducciones"" SET 
                ""FechaHora"" = @FechaHora,
                ""ClienteId"" = @ClienteId,
                ""CancionId"" = @CancionId
            WHERE ""Id"" = @Id", new
            {
                FechaHora = reproduccion.FechaHora,
                ClienteId = reproduccion.ClienteId,
                CancionId = reproduccion.CancionId,
                Id = id
            });
        }

        // POST: api/Reproducciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Reproduccion PostReproduccion([FromBody]Reproduccion reproduccion)
        {
            connection.Execute(@"INSERT INTO ""Reproducciones"" (""FechaHora"", ""ClienteId"", ""CancionId"")
VALUES (@FechaHora, @ClienteId, @CancionId)", new
            {
                FechaHora = reproduccion.FechaHora,
                ClienteId = reproduccion.ClienteId,
                CancionId = reproduccion.CancionId
            });
            return reproduccion;
        }

        // DELETE: api/Reproducciones/5
        [HttpDelete("{ id}")]
        public void DeleteReproduccion(int id)
        {
            connection.Execute( @"DELETE FROM ""Reproducciones"" WHERE ""Id"" = @Id", new { Id = id });
        }
/*
        private bool ReproduccionExists(int id)
        {
            return _context.Reproducciones.Any(e => e.Id == id);
        }*/
    }
}
