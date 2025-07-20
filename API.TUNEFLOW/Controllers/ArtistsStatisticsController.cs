using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.User.Administration;
using Modelos.Tuneflow.User.Consumer;

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
        public ActionResult<ArtistStatistics> GetEstadisticasArtistaById(int id)
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
                ""TotalFollowers"" = @TotalFollowers,
                ""PublishedSongs"" = @PublishedSongs,
                ""PublishedAlbums"" = @PublishedAlbums
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
        public ActionResult<ArtistStatistics> PostEstadisticasArtista([FromBody]ArtistStatistics artistStatistics)
        {
            var sql = @"INSERT INTO ""ArtistsStatistics"" 
                (""ArtistId"", ""TotalPlays"", ""TotalFollowers"", ""PublishedSongs"", ""PublishedAlbums"") 
                VALUES (@ArtistId, @TotalPlays, @TotalFollowers, @PublishedSongs, @PublishedAlbums) RETURNING ""Id"";";
            var idReturned = connection.ExecuteScalar<int>(sql, new
            {
                ArtistId = artistStatistics.ArtistId,
                TotalPlays = artistStatistics.TotalPlays,
                TotalFollowers = artistStatistics.TotalFollowers,
                PublishedSongs = artistStatistics.PublishedSongs,
                PublishedAlbums = artistStatistics.PublishedAlbums
            });
            artistStatistics.Id = idReturned;

            return CreatedAtAction(nameof(GetEstadisticasArtistaById), new { id = idReturned }, artistStatistics);
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
