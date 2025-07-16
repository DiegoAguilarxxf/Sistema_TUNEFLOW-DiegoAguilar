using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Playlist;
using Modelos.Tuneflow.Media;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Modelos.Tuneflow.Usuario.Produccion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicsPlaylistsController : ControllerBase
    {
        /*private readonly TUNEFLOWContext _context;

        public MusicasPlaylistsController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public MusicsPlaylistsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/MusicasPlaylists
        [HttpGet]
        public IEnumerable<MusicPlaylist> GetMusicaPlaylist()
        {   var getmusic = connection.Query<MusicPlaylist>("SELECT * FROM \"MusicsPlaylists\"");
            return getmusic;
        }

        // GET: api/MusicasPlaylists/5
        [HttpGet("{id}")]
        public ActionResult<MusicPlaylist> GetMusicaPlaylistById(int id)
        {
            var musicPlaylist = connection.QuerySingle<MusicPlaylist>(@"SELECT * FROM ""MusicsPlaylists"" WHERE ""Id"" = @Id", new { Id = id });

            if (musicPlaylist == null)
            {
                return NotFound();
            }

            return musicPlaylist;
        }

        [HttpGet("ExistMusicPlaylist/{SongId}/{PlaylistId}")]
        public ActionResult<int> GetCancionFavoritaPorIdEIdCliente(int SongId, int PlaylistId)
        {
            var sql = @"SELECT ""Id"" FROM ""MusicsPlaylists"" WHERE ""SongId"" = @IdSong AND ""PlaylistId"" = @IdPlaylist";

            var existId = connection.ExecuteScalar<int?>(sql, new { IdSong = SongId, IdPlaylist = PlaylistId });

            if (existId.HasValue)
                return Ok(new { id = existId.Value });  // Devuelve el Id encontrado
            else
                return NotFound();  // O puedes devolver algo distinto si no existe


        }

        [HttpGet("SongsForPlaylist/{idPlaylist}")]
        public ActionResult<IEnumerable<Song>> ObtenerCancionesPorPlaylist(int idPlaylist)
        {
            var sql = @"
                            SELECT 
                                c.""Id"", c.""Title"", c.""Duration"", c.""Genere"", c.""ArtistId"", c.""AlbumId"",
                                c.""FilePath"", c.""ExplicitContent"", c.""ImagePath"",

                                a.""Id"", a.""StageName"", a.""MusicGenre"", a.""Biography"",
                                a.""CountryId"", a.""Virified"", a.""UserId"",

                                al.""Id"", al.""Title"", al.""ReleaseDate"", al.""Genre"",
                                al.""CreationDate"", al.""Description"", al.""CoverPath""
                            FROM ""MusicsPlaylists"" mp
                            INNER JOIN ""Songs"" c ON mp.""SongId"" = c.""Id""
                            INNER JOIN ""Artists"" a ON c.""ArtistId"" = a.""Id""
                            LEFT JOIN ""Albums"" al ON c.""AlbumId"" = al.""Id""
                            WHERE mp.""PlaylistId"" = @IdPlaylist";

            var songs = connection.Query<Song, Artist, Album, Song>(
                sql,
                (song, artist, album) =>
                {
                    song.Artist = artist;
                    song.Album = album;
                    return song;
                },
                new { IdPlaylist = idPlaylist },
                splitOn: "Id,Id" // importante para Dapper
            ).ToList();

            if (!songs.Any())
            {
                return NotFound($"No se encontraron canciones para la playlist con ID {idPlaylist}.");
            }

            return Ok(songs);
        }

        // PUT: api/MusicasPlaylists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutMusicaPlaylist(int id,[FromBody] MusicPlaylist musicPlaylist)
        {
            connection.Execute(@"UPDATE ""MusicasPlaylists"" SET 
                ""PlaylistId"" = @PlaylistId,
                ""MusicaId"" = @MusicaId
                WHERE ""Id"" = @Id", new
            {
                Id = id,
                PlaylistId = musicPlaylist.PlaylistId,
                SongId = musicPlaylist.SongId
            });
        }

        // POST: api/MusicasPlaylists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<MusicPlaylist> PostMusicaPlaylist([FromBody]MusicPlaylist musicPlaylist)
        {
            var idReturned = connection.ExecuteScalar<int>(@"INSERT INTO ""MusicsPlaylists"" (""SongId"", ""PlaylistId"") 
                VALUES (@SongId, @PlaylistId) RETURNING ""Id"";", new
            {
                SongId = musicPlaylist.SongId,
                PlaylistId = musicPlaylist.PlaylistId
            });

            musicPlaylist.Id = idReturned;

            return CreatedAtAction(nameof(GetMusicaPlaylistById), new { id = idReturned }, musicPlaylist);
        }

        // DELETE: api/MusicasPlaylists/5
        [HttpDelete("{id}")]
        public ActionResult DeleteMusicaPlaylist(int id)
        {
            connection.Execute(@"DELETE FROM ""MusicsPlaylists"" WHERE ""Id"" = @Id", new { Id = id });
            return NoContent(); // Código 204
        }

    }
}
