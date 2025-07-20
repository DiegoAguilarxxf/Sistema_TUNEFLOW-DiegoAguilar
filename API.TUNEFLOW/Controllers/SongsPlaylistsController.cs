using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.Media;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Modelos.Tuneflow.User.Production;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsPlaylistsController : ControllerBase
    {
        /*private readonly TUNEFLOWContext _context;

        public MusicasPlaylistsController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public SongsPlaylistsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/MusicasPlaylists
        [HttpGet]
        public IEnumerable<SongPlaylist> GetMusicaPlaylist()
        {   var getmusic = connection.Query<SongPlaylist>("SELECT * FROM \"SongsPlaylists\"");
            return getmusic;
        }

        // GET: api/MusicasPlaylists/5
        [HttpGet("{id}")]
        public ActionResult<SongPlaylist> GetMusicaPlaylistById(int id)
        {
            var songPlaylist = connection.QuerySingle<SongPlaylist>(@"SELECT * FROM ""SongsPlaylists"" WHERE ""Id"" = @Id", new { Id = id });

            if (songPlaylist == null)
            {
                return NotFound();
            }

            return songPlaylist;
        }

        [HttpGet("ExistSongPlaylist/{SongId}/{PlaylistId}")]
        public ActionResult<int> GetCancionFavoritaPorIdEIdCliente(int SongId, int PlaylistId)
        {
            var sql = @"SELECT ""Id"" FROM ""SongsPlaylists"" WHERE ""SongId"" = @IdSong AND ""PlaylistId"" = @IdPlaylist";

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
                                c.""Id"", c.""Title"", c.""Duration"", c.""Genre"", c.""ArtistId"", c.""AlbumId"",
                                c.""FilePath"", c.""ExplicitContent"", c.""ImagePath"",c.""Available"",

                                a.""Id"", a.""StageName"", a.""MusicGenre"",
                                a.""CountryId"", a.""Verified"", a.""UserId"",

                                al.""Id"", al.""Title"", al.""ReleaseDate"", al.""Genre"",
                                al.""CreationDate"", al.""Description"", al.""CoverPath""
                            FROM ""SongsPlaylists"" mp
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
        public void PutMusicaPlaylist(int id,[FromBody] SongPlaylist musicPlaylist)
        {
            connection.Execute(@"UPDATE ""SongsPlaylists"" SET 
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
        public ActionResult<SongPlaylist> PostMusicaPlaylist([FromBody]SongPlaylist musicPlaylist)
        {
            var idReturned = connection.ExecuteScalar<int>(@"INSERT INTO ""SongsPlaylists"" (""SongId"", ""PlaylistId"") 
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
            connection.Execute(@"DELETE FROM ""SongsPlaylists"" WHERE ""Id"" = @Id", new { Id = id });
            return NoContent(); // Código 204
        }

    }
}
