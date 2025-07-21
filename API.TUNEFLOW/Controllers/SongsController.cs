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
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.User.Production;
using Npgsql;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {

        private readonly IConfiguration _config;
        public SongsController(IConfiguration config)
        {
            _config = config;   
        }
         
        // GET: api/Canciones
        [HttpGet]
        public ActionResult<IEnumerable<Song>> GetCancion()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            string sql = @"
                            SELECT 
                                c.""Id"", c.""Title"", c.""Duration"", c.""Genre"", c.""FilePath"", c.""ExplicitContent"", c.""ImagePath"",
                                al.""Title"" AS AlbumTitle,
                                ar.""Id"" AS ArtistId,
                                ar.""StageName"" AS StageName
                            FROM ""Songs"" c
                            LEFT JOIN ""Albums"" al ON c.""AlbumId"" = al.""Id""
                            LEFT JOIN ""Artists"" ar ON c.""ArtistId"" = ar.""Id""";

            var songs = connection.Query<Song, string, int, string, Song>(
                sql,
                (song, albumTitle, artistId, stageName) =>
                {
                    song.Album = new Album { Title = albumTitle };
                    song.Artist = new Artist { Id = artistId, StageName = stageName };
                    return song;
                },
                splitOn: "AlbumTitle,ArtistId,StageName"
            ).ToList();

            if (!songs.Any())
                return NotFound("No se encontraron canciones con ese título.");

            return Ok(songs);
        }

        // GET: api/Canciones/5
        [HttpGet("{id}")]
        public ActionResult<Song> GetCancion(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            var song = connection.QuerySingle<Song>(@"SELECT * FROM ""Songs"" WHERE ""Id"" = @Id", new { Id = id });
            return song;
        }

        [HttpGet("Title/{title}")]
        public ActionResult<IEnumerable<Song>> GetCancionByTitulo(string title)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            string sql = @"
                            SELECT 
                                c.""Id"", c.""Title"", c.""Duration"", c.""Genre"", c.""FilePath"", c.""ExplicitContent"", c.""ImagePath"",
                                al.""Title"" AS AlbumTitle,
                                ar.""Id"" AS Id, ar.""StageName""
                            FROM ""Songs"" c
                            LEFT JOIN ""Albums"" al ON c.""AlbumId"" = al.""Id""
                            LEFT JOIN ""Artists"" ar ON c.""ArtistId"" = ar.""Id""
                            WHERE c.""Title"" ILIKE @Title";

            var songs = connection.Query<Song, string, Artist, Song>(
                sql,
                (song, albumTitle, artist) =>
                {
                    song.Album = new Album { Title = albumTitle };
                    song.Artist = artist;
                    return song;
                },
                new { Title = $"%{title}%" },
                splitOn: "AlbumTitle,Id"
            ).ToList();

            if (!songs.Any())
                return NotFound("No se encontraron canciones con ese título.");

            return Ok(songs);
        }

        [HttpGet("PorArtista/{id}")]
        public ActionResult<IEnumerable<Song>> ObtenerCancionesPorArtista(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            string sql = @"SELECT * FROM ""Songs"" WHERE ""ArtistId"" = @ArtistId";

            var songs = connection.Query<Song>(sql, new { ArtistId = id }).ToList();

            if (!songs.Any())
                return NotFound("No hay canciones para ese artista");

            return Ok(songs);
        }

        [HttpGet("PorGenero/{genero}")]
        public ActionResult<IEnumerable<Song>> ObtenerCancionesPorGenero(string genero)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            string sql = @"SELECT * FROM ""Songs"" WHERE ""Genre"" = @Genre";

            var songs = connection.Query<Song>(sql, new { Genre = genero }).ToList();

            if (!songs.Any())
                return NotFound("No hay canciones para ese artista");

            return Ok(songs);
        }

        // PUT: api/Canciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutCancion(int id,[FromBody]Song song)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
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
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
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
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            connection.Execute(@"DELETE FROM ""Songs"" WHERE ""Id"" = @Id", new { Id = id });    
        }
/*
        private bool CancionExists(int id)
        {
            return _context.Canciones.Any(e => e.Id == id);
        }*/
    }//
}
