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
    public class CancionesFavoritasController : ControllerBase
    {
        /* private readonly TUNEFLOWContext _context;

         public CancionesFavoritasController(TUNEFLOWContext context)
         {
             _context = context;
         }
        */
        private DbConnection connection;
        // GET: api/CancionesFavoritas
        [HttpGet]
        public IEnumerable<CancionFavorita> GetCancionFavorita()
        { var cancionesfav = connection.Query<CancionFavorita>("SELECT * FROM \"CancionesFavoritas\"");
            return cancionesfav;
        }

        // GET: api/CancionesFavoritas/5
        [HttpGet("{id}")]
        public ActionResult<CancionFavorita> GetCancionFavorita(int id)
        {
            var cancionFavorita = connection.QuerySingle<CancionFavorita>(@"SELECT * FROM ""CancionesFavoritas"" WERE ""Id""= @Id", new { Id = id });

            if (cancionFavorita == null)
            {
                return NotFound();
            }

            return cancionFavorita;
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
        public CancionFavorita PostCancionFavorita([FromBody]CancionFavorita cancionfavorita)
        {   connection.Execute(@"INSERT INTO ""CancionesFavoritas"" (""ClienteId"", ""CancionId"", ""FechaAgregado"") " +
                "VALUES (@ClienteId, @CancionId, @FechaAgregado) RETURNING *",
                new
                {
                    ClienteId = cancionfavorita.ClienteId,
                    CancionId = cancionfavorita.CancionId,
                    FechaAgregado = cancionfavorita.FechaAgregado
                });


            return cancionfavorita;
        }

        // DELETE: api/CancionesFavoritas/5
        [HttpDelete("{id}")]
        public void DeleteCancionFavorita(int id)
        {
           connection.Execute(@"DELETE FROM ""CancionesFavoritas"" WHERE ""Id"" = @Id", new { Id = id });
        }

       /* private bool CancionFavoritaExists(int id)
        {
            return _context.CancionesFavoritas.Any(e => e.Id == id);
        }*/
    }
}
