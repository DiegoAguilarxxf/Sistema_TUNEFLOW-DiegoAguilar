using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Usuario.Administracion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsStatisticsController : ControllerBase
    {
        //private readonly TUNEFLOWContext _context;
        private DbConnection connection;

        /*public EstadisticasArtistasController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        public ArtistsStatisticsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/EstadisticasArtistas
        [HttpGet]
        public IEnumerable<EstadisticasArtista> GetEstadisticasArtista()
        { var estadisticasartistas = connection.Query<EstadisticasArtista>("SELECT * FROM \"EstadisticasArtistas\"");
            return estadisticasartistas;
        }

        // GET: api/EstadisticasArtistas/5
        [HttpGet("{id}")]
        public ActionResult<EstadisticasArtista> GetEstadisticasArtista(int id)
        {
            var estadisticasArtista = connection.QuerySingle<EstadisticasArtista>(@"SELECT * FROM ""EstadisticasArtistas"" WHERE ""Id"" = @Id", new { Id = id });

            if (estadisticasArtista == null)
            {
                return NotFound();
            }

            return estadisticasArtista;
        }

        // PUT: api/EstadisticasArtistas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutEstadisticasArtista(int id, [FromBody] EstadisticasArtista estadisticasArtista)
        {
           connection.Execute(@"UPDATE ""EstadisticasArtistas"" SET 
                ""ArtistaId"" = @ArtistaId,
                ""ReproduccionesTotales"" = @ReproduccionesTotales,
                ""SeguidoresTotales"" = @SeguidoresTotales,
                ""CancionesPublicadas"" = @CancionesPublicadas,
                ""AlbumesPublicados"" = @AlbumesPublicados
                WHERE ""Id"" = @Id", new
           {
               Id = id,
               ArtistaId = estadisticasArtista.ArtistaId,
               ReproduccionesTotales = estadisticasArtista.ReproduccionesTotales,
               SeguidoresTotales = estadisticasArtista.SeguidoresTotales,
               CancionesPublicadas = estadisticasArtista.CancionesPublicadas,
               AlbumesPublicados = estadisticasArtista.AlbumesPublicados
           });
            
        }

        // POST: api/EstadisticasArtistas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public EstadisticasArtista PostEstadisticasArtista([FromBody]EstadisticasArtista estadisticasArtista)
        {
            connection.Execute(@"INSERT INTO ""EstadisticasArtistas"" 
                (""ArtistaId"", ""ReproduccionesTotales"", ""SeguidoresTotales"", ""CancionesPublicadas"", ""AlbumesPublicados"") 
                VALUES (@ArtistaId, @ReproduccionesTotales, @SeguidoresTotales, @CancionesPublicadas, @AlbumesPublicados)", new
            {
                ArtistaId = estadisticasArtista.ArtistaId,
                ReproduccionesTotales = estadisticasArtista.ReproduccionesTotales,
                SeguidoresTotales = estadisticasArtista.SeguidoresTotales,
                CancionesPublicadas = estadisticasArtista.CancionesPublicadas,
                AlbumesPublicados = estadisticasArtista.AlbumesPublicados
            });

            return estadisticasArtista;
        }

        // DELETE: api/EstadisticasArtistas/5
        [HttpDelete("{id}")]
        public void DeleteEstadisticasArtista(int id)
        {
            connection.Execute(@"DELETE FROM ""EstadisticasArtistas"" WHERE ""Id"" = @Id", new { Id = id });

           
        }
        /*
        private bool EstadisticasArtistaExists(int id)
        {
            return _context.EstadisticasArtistas.Any(e => e.Id == id);
        }*/
    }
}
