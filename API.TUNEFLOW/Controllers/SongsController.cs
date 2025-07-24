using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.User.Consumer;
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
        public ActionResult<Song> GetCancionById(int id)
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
        public void PutCancion(int id, [FromBody] Song song)
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
        public ActionResult<Song> PostCancion([FromBody] Song song)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            var idDevuelto = connection.ExecuteScalar<int>(@"INSERT INTO ""Songs"" 
                (""Title"", ""Duration"", ""Genre"", ""ArtistId"", ""AlbumId"", ""FilePath"", ""ExplicitContent"", ""ImagePath"",""ReleaseDate"",""Available"") 
                VALUES (@Title, @Duration, @Genre, @ArtistId, @AlbumId, @FilePath, @ExplicitContent, @ImagePath, @ReleaseDate, @Available) RETURNING ""Id"";", new
            {
                Title = song.Title,
                Duration = song.Duration,
                Genre = song.Genre,
                ArtistId = song.ArtistId,
                AlbumId = song.AlbumId,
                FilePath = song.FilePath,
                ExplicitContent = song.ExplicitContent,
                ImagePath = song.ImagePath,
                ReleaseDate = song.ReleaseDate,
                Available = song.Available
            });
            song.Id = idDevuelto;

            return CreatedAtAction(nameof(GetCancionById), new { id = idDevuelto }, song);
        }

        // DELETE: api/Canciones/5
        [HttpDelete("{id}")]
        public void DeleteCancion(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            connection.Execute(@"DELETE FROM ""Songs"" WHERE ""Id"" = @Id", new { Id = id });
        }

        [HttpGet("Random")]
        public ActionResult<Song> GetCancionAleatoria()
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
        LEFT JOIN ""Artists"" ar ON c.""ArtistId"" = ar.""Id""
        ORDER BY RANDOM()
        LIMIT 1";

            var resultado = connection.Query<Song, string, int, string, Song>(
                sql,
                (song, albumTitle, artistId, stageName) =>
                {
                    song.Album = new Album { Title = albumTitle };
                    song.Artist = new Artist { Id = artistId, StageName = stageName };
                    return song;
                },
                splitOn: "AlbumTitle,ArtistId,StageName"
            ).FirstOrDefault();

            if (resultado == null)
                return NotFound("No se encontró ninguna canción aleatoria.");

            return Ok(resultado);
        }

        [HttpGet("Letra/{songId}")]
        public async Task<IActionResult> ObtenerLetraPorCancion(int songId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();

            string sql = @"
        SELECT 
            c.""Title"",
            ar.""StageName""
        FROM ""Songs"" c
        LEFT JOIN ""Artists"" ar ON c.""ArtistId"" = ar.""Id""
        WHERE c.""Id"" = @SongId";

            var resultado = connection.QuerySingleOrDefault<(string Title, string StageName)>(sql, new { SongId = songId });

            if (resultado == default)
            {
                Console.WriteLine($"No se encontró la canción con ID {songId}");
                return NotFound("Canción no encontrada.");
            }

            var artista = resultado.StageName ?? "Desconocido";
            var titulo = resultado.Title;

            Console.WriteLine($"Buscando letra para artista: {artista}, título: {titulo}");

            try
            {
                using var httpClient = new HttpClient();
                var url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(artista)}/{Uri.EscapeDataString(titulo)}";

                Console.WriteLine($"URL para la API de letras: {url}");

                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"No se encontró la letra. Código HTTP: {response.StatusCode}");
                    return NotFound("No se encontró la letra.");
                }

                var data = await response.Content.ReadFromJsonAsync<JsonElement>();
                var letra = data.GetProperty("lyrics").GetString();

                Console.WriteLine("Letra obtenida con éxito.");

                return Ok(new { artista, titulo, letra });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la letra: {ex.Message}");
                return StatusCode(500, "Error al obtener la letra.");
            }
        }
        [HttpGet("Artista/{id}")]
        public ActionResult<string> ObtenerNombreArtistaPorCancion(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();

            string sql = @"
        SELECT ar.""StageName""
        FROM ""Songs"" s
        LEFT JOIN ""Artists"" ar ON s.""ArtistId"" = ar.""Id""
        WHERE s.""Id"" = @Id";

            var nombreArtista = connection.QuerySingleOrDefault<string>(sql, new { Id = id });

            if (string.IsNullOrEmpty(nombreArtista))
                return NotFound($"No se encontró artista para la canción con ID {id}");

            return Ok(nombreArtista);
        }


    }
}
