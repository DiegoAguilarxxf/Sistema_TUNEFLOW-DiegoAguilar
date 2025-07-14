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
using Modelos.Tuneflow.Usuario.Perfiles;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancionesFavoritasController : ControllerBase
    {
        /* private readonly TUNEFLOWContext _context;

         public CancionesFavoritasController(TUNEFLOWContext context)
         {
             _context = context;
         }
        */
        private DbConnection connection;

        public CancionesFavoritasController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }
        // GET: api/CancionesFavoritas
        [HttpGet]
        public IEnumerable<CancionFavorita> GetCancionFavorita()
        { var cancionesfav = connection.Query<CancionFavorita>("SELECT * FROM \"CancionesFavoritas\"");
            return cancionesfav;
        }

        // GET: api/CancionesFavoritas/5
        [HttpGet("{id}")]
        public ActionResult<CancionFavorita> GetCancionFavoritaById(int id)
        {
            var cancionFavorita = connection.QuerySingle<CancionFavorita>(@"SELECT * FROM ""CancionesFavoritas"" WERE ""Id""= @Id", new { Id = id });

            if (cancionFavorita == null)
            {
                return NotFound();
            }

            return cancionFavorita;
        }

        [HttpGet("IsFavorita/{id}/{idCliente}")]
        public ActionResult<int> GetCancionFavoritaPorIdEIdCliente(int id, int idCliente)
        {
            var sql = @"SELECT ""Id"" FROM ""CancionesFavoritas"" WHERE ""ClienteId"" = @IdCliente AND ""CancionId"" = @Id";

            var existeId = connection.ExecuteScalar<int?>(sql, new { IdCliente = idCliente, Id = id });

            if (existeId.HasValue)
                return Ok(new { id = existeId.Value });  // Devuelve el Id encontrado
            else
                return NotFound();  // O puedes devolver algo distinto si no existe


        }

        // PUT: api/CancionesFavoritas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutCancionFavorita(int id,[FromBody] CancionFavorita cancionFavorita)
        {
            connection.Execute(@"UPDATE ""CancionesFavoritas"" SET " +
                "\"ClienteId\" = @ClienteId, " +
                "\"CancionId\" = @CancionId, " +
                "\"FechaAgregado\" = @FechaAgregado " +
                "WHERE \"Id\" = @Id",new
                {
                    ClienteId=cancionFavorita.ClienteId,
                    CancionId=cancionFavorita.CancionId,
                    FechaAgregado = cancionFavorita.FechaAgregado,
                    Id = id
                } );
        }

        // POST: api/CancionesFavoritas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<CancionFavorita> PostCancionFavorita([FromBody]CancionFavorita cancionfavorita)
        {   
            
            var idDevuelto = connection.ExecuteScalar<int>(@"INSERT INTO ""CancionesFavoritas"" (""ClienteId"", ""CancionId"", ""FechaAgregado"") 
                                                VALUES (@ClienteId, @CancionId, @FechaAgregado) RETURNING ""Id"";",
                new
                {
                    ClienteId = cancionfavorita.ClienteId,
                    CancionId = cancionfavorita.CancionId,
                    FechaAgregado = cancionfavorita.FechaAgregado
                });
            cancionfavorita.Id = idDevuelto;

            return CreatedAtAction(nameof(GetCancionFavoritaById), new { id = idDevuelto }, cancionfavorita);
        }

        // DELETE: api/CancionesFavoritas/5
        [HttpDelete("{id}")]
        public ActionResult DeleteCancionFavorita(int id)
        {
           connection.Execute(@"DELETE FROM ""CancionesFavoritas"" WHERE ""Id"" = @Id", new { Id = id });
            return NoContent(); // Código 204
        }

       /* private bool CancionFavoritaExists(int id)
        {
            return _context.CancionesFavoritas.Any(e => e.Id == id);
        }*/
    }
}
