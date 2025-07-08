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
    public class PlaylistsController : ControllerBase
    {
        /*private readonly TUNEFLOWContext _context;

        public PlaylistsController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public PlaylistsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Playlists
        [HttpGet]
        public IEnumerable<Playlist> GetPlaylist()
        {    var playlist = connection.Query<Playlist>("SELECT * FROM \"Playlists\"");
            return playlist;
        }

        // GET: api/Playlists/5
        [HttpGet("{id}")]
        public ActionResult<Playlist> GetPlaylist(int id)
        {
            var playlist = connection.QuerySingle<Playlist>(@"SELECT * FROM ""Playlists"" WHERE ""Id"" = @Id", new { Id = id });

            if (playlist == null)
            {
                return NotFound();
            }

            return playlist;
        }

        // PUT: api/Playlists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void  PutPlaylist(int id,[FromBody] Playlist playlist)
        {
            connection.Execute(@"UPDATE ""Playlists"" SET
""Titulo"" = @Titulo,
""Descripcion"" = @Descripcion,
""FechaCreacion"" = @FechaCreacion,
""FechaCreacion"" = @FechaCreacion,
""ClienteId"" = @ClienteId", new
            {
                Titulo = playlist.Titulo,
                Descripcion = playlist.Descripcion,
                FechaCreacion = playlist.FechaCreacion,
                ClienteId = playlist.ClienteId
            });

        }

        // POST: api/Playlists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Playlist PostPlaylist([FromBody] Playlist playlist)
        {
            connection.Execute(@"INSERT INTO ""Playlists"" (""Titulo"", ""Descripcion"", ""FechaCreacion"", ""ClienteId"")
VALUES (@Titulo, @Descripcion, @FechaCreacion, @ClienteId)", new
            {
                Titulo = playlist.Titulo,
                Descripcion = playlist.Descripcion,
                FechaCreacion = playlist.FechaCreacion,
                ClienteId = playlist.ClienteId
            });
            return playlist;

        }
          

        // DELETE: api/Playlists/5
        [HttpDelete("{id}")]
        public  void DeletePlaylist(int id)
        { connection.Execute(@"DELETE FROM ""Playlists"" WHERE ""Id"" = @Id", new { Id = id });
           
        }
        /*
        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }*/
    }
}
