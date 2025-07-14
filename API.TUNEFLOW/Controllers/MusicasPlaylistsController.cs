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

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicasPlaylistsController : ControllerBase
    {
        /*private readonly TUNEFLOWContext _context;

        public MusicasPlaylistsController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public MusicasPlaylistsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/MusicasPlaylists
        [HttpGet]
        public IEnumerable<MusicaPlaylist> GetMusicaPlaylist()
        {   var getmusic = connection.Query<MusicaPlaylist>("SELECT * FROM \"MusicasPlaylists\"");
            return getmusic;
        }

        // GET: api/MusicasPlaylists/5
        [HttpGet("{id}")]
        public ActionResult<MusicaPlaylist> GetMusicaPlaylistById(int id)
        {
            var musicaPlaylist = connection.QuerySingle<MusicaPlaylist>(@"SELECT * FROM ""MusicasPlaylists"" WHERE ""Id"" = @Id", new { Id = id });

            if (musicaPlaylist == null)
            {
                return NotFound();
            }

            return musicaPlaylist;
        }

        [HttpGet("ExistMusicaPlaylist/{CancionId}/{PlaylistId}")]
        public ActionResult<int> GetCancionFavoritaPorIdEIdCliente(int CancionId, int PlaylistId)
        {
            var sql = @"SELECT ""Id"" FROM ""MusicasPlaylists"" WHERE ""CancionId"" = @IdCancion AND ""PlaylistId"" = @IdPlaylist";

            var existeId = connection.ExecuteScalar<int?>(sql, new { IdCancion = CancionId, IdPlaylist = PlaylistId });

            if (existeId.HasValue)
                return Ok(new { id = existeId.Value });  // Devuelve el Id encontrado
            else
                return NotFound();  // O puedes devolver algo distinto si no existe


        }

        // PUT: api/MusicasPlaylists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutMusicaPlaylist(int id,[FromBody] MusicaPlaylist musicaPlaylist)
        {
            connection.Execute(@"UPDATE ""MusicasPlaylists"" SET 
                ""PlaylistId"" = @PlaylistId,
                ""MusicaId"" = @MusicaId
                WHERE ""Id"" = @Id", new
            {
                Id = id,
                PlaylistId = musicaPlaylist.PlaylistId,
                CancionId = musicaPlaylist.CancionId
            });
        }

        // POST: api/MusicasPlaylists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<MusicaPlaylist> PostMusicaPlaylist([FromBody]MusicaPlaylist musicaPlaylist)
        {
            var idDevuelto = connection.ExecuteScalar<int>(@"INSERT INTO ""MusicasPlaylists"" (""CancionId"", ""PlaylistId"") 
                VALUES (@CancionId, @PlaylistId) RETURNING ""Id"";", new
            {
                CancionId = musicaPlaylist.CancionId,
                PlaylistId = musicaPlaylist.PlaylistId
            });

            musicaPlaylist.Id = idDevuelto;

            return CreatedAtAction(nameof(GetMusicaPlaylistById), new { id = idDevuelto }, musicaPlaylist);
        }

        // DELETE: api/MusicasPlaylists/5
        [HttpDelete("{id}")]
        public ActionResult DeleteMusicaPlaylist(int id)
        {
            connection.Execute(@"DELETE FROM ""MusicasPlaylists"" WHERE ""Id"" = @Id", new { Id = id });
            return NoContent(); // Código 204
        }

    }
}
