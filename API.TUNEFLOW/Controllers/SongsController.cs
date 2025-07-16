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
using Modelos.Tuneflow.Playlist;
using Modelos.Tuneflow.Usuario.Produccion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        /*rivate readonly TUNEFLOWContext _context;

        public CancionesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public SongsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }
         
        // GET: api/Canciones
        [HttpGet]
        public ActionResult<IEnumerable<Song>> GetCancion()
        {
            string sql = @"
                            SELECT 
                                c.""Id"", c.""Title"", c.""Duration"", c.""Genre"", c.""FilePath"", c.""ExplicitContent"", c.""ImagePath"",
                                al.""Title"" AS AlbumTitle,
                                ar.""StageName"" AS StageName
                            FROM ""Songs"" c
                            LEFT JOIN ""Albums"" al ON c.""AlbumId"" = al.""Id""
                            LEFT JOIN ""Artists"" ar ON c.""ArtistId"" = ar.""Id""";

            var songs = connection.Query<Song, string, string, Song>(
                sql,
                (song, albumTitle, stageName) =>
                {
                    song.Album = new Album { Title = albumTitle };
                    song.Artist = new Artist { StageName = stageName };
                    return song;
                },

                splitOn: "AlbumTitle,StageName"
            ).ToList();

            if (!songs.Any())
                return NotFound("No se encontraron canciones con ese título.");

            return Ok(songs);
        }

        // GET: api/Canciones/5
        [HttpGet("{id}")]
        public ActionResult<Song> GetCancion(int id)
        {   var song = connection.QuerySingle<Song>(@"SELECT * FROM ""Songs"" WHERE ""Id"" = @Id", new { Id = id });
            return song;
        }

        [HttpGet("Title/{title}")]
        public ActionResult<IEnumerable<Song>> GetCancionByTitulo(string title)
        {
            string sql = @"
                            SELECT 
                                c.""Id"", c.""Title"", c.""Duration"", c.""Genre"", c.""FilePath"", c.""ExplictContent"", c.""ImagePath"",
                                al.""Title"" AS AlbumTitle,
                                ar.""StageName"" AS StageName
                            FROM ""Songs"" c
                            LEFT JOIN ""Albums"" al ON c.""AlbumId"" = al.""Id""
                            LEFT JOIN ""Artists"" ar ON c.""ArtistId"" = ar.""Id""
                            WHERE c.""Title"" ILIKE @Title";

            var songs = connection.Query<Song, string, string, Song>(
                sql,
                (song, albumTitle, stageName) =>
                {
                    song.Album = new Album { Title = albumTitle };
                    song.Artist = new Artist { StageName = stageName };
                    return song;
                },
                new { Title = $"%{title}%" },
                splitOn: "AlbumTitle,StageName"
            ).ToList();

            if (!songs.Any())
                return NotFound("No se encontraron canciones con ese título.");

            return Ok(songs);
        }


        // PUT: api/Canciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutCancion(int id,[FromBody]Song song)
        {
            connection.Execute(@"UPDATE ""Songs"" SET 
                ""Title"" = @Title, 
                ""Duration"" = @Duration, 
                ""Genre"" = @Genre, 
                ""ArtistId"" = @ArtistId, 
                ""AlbumId"" = @AlbumId, 
                ""FilePath"" = @FilePath, 
                ""ExplicitContent"" = @ExplicitContent,
                ""ImagePath"" = @ImagePath
            WHERE ""Id"" = @Id", new
            {
                                   Id = id,
                Title = song.Title,
                Duration = song.Duration,
                Genre = song.Genre,
                ArtistId = song.ArtistId,
                AlbumId = song.AlbumId,
                FilePath = song.FilePath,
                ExplicitContent = song.ExplicitContent,
                ImagePath = song.ImagePath
            });
        }

        // POST: api/Canciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Song PostCancion([FromBody]Song song)
        {
            connection.Execute(@"INSERT INTO ""Songs"" 
                (""Title"", ""Duration"", ""Genre"", ""ArtistId"", ""AlbumId"", ""FilePath"", ""ExplicitContent"", ""ImagePath"") 
                VALUES (@Title, @Duration, @Genre, @ArtistId, @AlbumId, @FilePath, @ExplicitContent, @ImagePath)", new
            {
               Title= song.Title,
                Duration= song.Duration,
               Genre= song.Genre,
               ArtistId= song.ArtistId,
               AlbumId= song.AlbumId,
               FilePath= song.FilePath,
               ExplicitContent= song.ExplicitContent,
               ImagePath=song.ImagePath
            });

            return song;
        }

        // DELETE: api/Canciones/5
        [HttpDelete("{id}")]
        public void DeleteCancion(int id)
        {
           connection.Execute(@"DELETE FROM ""Songs"" WHERE ""Id"" = @Id", new { Id = id });    
        }
/*
        private bool CancionExists(int id)
        {
            return _context.Canciones.Any(e => e.Id == id);
        }*/
    }//
}
