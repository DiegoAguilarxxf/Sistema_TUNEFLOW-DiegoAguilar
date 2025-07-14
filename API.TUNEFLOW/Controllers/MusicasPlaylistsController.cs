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
        public void DeleteMusicaPlaylist(int id)
        {
            connection.Execute(@"DELETE FROM ""MusicasPlaylists"" WHERE ""Id"" = @Id", new { Id = id });
        }
        /*
        private bool MusicaPlaylistExists(int id)
        {
            return _context.MusicasPlaylists.Any(e => e.Id == id);
        }*/
    }
}
