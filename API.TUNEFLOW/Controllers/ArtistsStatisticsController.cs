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
        public IEnumerable<ArtistStatistics> GetEstadisticasArtista()
        { var artistStatistics = connection.Query<ArtistStatistics>("SELECT * FROM \"ArtistsStatistics\"");
            return artistStatistics;
        }

        // GET: api/EstadisticasArtistas/5
        [HttpGet("{id}")]
        public ActionResult<ArtistStatistics> GetEstadisticasArtista(int id)
        {
            var artistStatistics = connection.QuerySingle<ArtistStatistics>(@"SELECT * FROM ""ArtistsStatistics"" WHERE ""Id"" = @Id", new { Id = id });

            if (artistStatistics == null)
            {
                return NotFound();
            }

            return artistStatistics;
        }

        // PUT: api/EstadisticasArtistas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutEstadisticasArtista(int id, [FromBody] ArtistStatistics artistStatistics)
        {
           connection.Execute(@"UPDATE ""ArtistsStatistics"" SET 
                ""ArtistId"" = @ArtistId,
                ""TotalPlays"" = @TotalPlays,
                ""STotalFollowers"" = @TotalFollowers,
                ""PublishedSongs"" = @PublishedSongs,
                ""PublishedAlbums"" = @APublishedAlbums
                WHERE ""Id"" = @Id", new
           {
               Id = id,
               ArtistId = artistStatistics.ArtistId,
               TotalPlays = artistStatistics.TotalPlays,
               TotalFollowers = artistStatistics.TotalFollowers,
               PublishedSongs = artistStatistics.PublishedSongs,
               PublishedAlbums = artistStatistics.PublishedAlbums
           });
            
        }

        // POST: api/EstadisticasArtistas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ArtistStatistics PostEstadisticasArtista([FromBody]ArtistStatistics artistStatistics)
        {
            connection.Execute(@"INSERT INTO ""ArtistsStatistics"" 
                (""ArtistaId"", ""ReproduccionesTotales"", ""SeguidoresTotales"", ""CancionesPublicadas"", ""AlbumesPublicados"") 
                VALUES (@ArtistaId, @ReproduccionesTotales, @SeguidoresTotales, @CancionesPublicadas, @AlbumesPublicados)", new
            {
                ArtistId = artistStatistics.ArtistId,
                TotalPlays = artistStatistics.TotalPlays,
                TotalFollowers = artistStatistics.TotalFollowers,
                PublishedSongs = artistStatistics.PublishedSongs,
                PublishedAlbums = artistStatistics.PublishedAlbums
            });

            return artistStatistics;
        }

        // DELETE: api/EstadisticasArtistas/5
        [HttpDelete("{id}")]
        public void DeleteEstadisticasArtista(int id)
        {
            connection.Execute(@"DELETE FROM ""ArtistsStatistics"" WHERE ""Id"" = @Id", new { Id = id });

           
        }
        /*
        private bool EstadisticasArtistaExists(int id)
        {
            return _context.EstadisticasArtistas.Any(e => e.Id == id);
        }*/
    }
}
