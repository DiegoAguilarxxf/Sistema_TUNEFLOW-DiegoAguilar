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
using Modelos.Tuneflow.Usuario.Consumidor;

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
        public ActionResult<Playlist> GetPlaylistById(int id)
        {
            var playlist = connection.QuerySingleOrDefault<Playlist>(@"SELECT * FROM ""Playlists"" WHERE ""Id"" = @Id", new { Id = id });

            if (playlist == null)
            {
                return NotFound();
            }

            return playlist;
        }

        [HttpGet("Cliente/Playlist/{id}")]
        public ActionResult<IEnumerable<Playlist>> GetPlaylistForClienteId(int id)
        {
            var sql = @"SELECT * FROM ""Playlists"" WHERE ""ClienteId"" = @Id";

            var playlists = connection.Query<Playlist>(sql, new {Id = id});

            if (!playlists.Any())
                return NotFound("No se encontraron playlist para ese cliente");

            return Ok(playlists);
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
        public ActionResult<Playlist> PostPlaylist([FromBody] Playlist playlist)
        {
            var sql = @"INSERT INTO ""Playlists"" (""Titulo"", ""Descripcion"", ""FechaCreacion"", ""ClienteId"",""PortadaPlaylist"")
            VALUES (@Titulo, @Descripcion, @FechaCreacion, @ClienteId, @PortadaPlaylist) RETURNING ""Id"";";
            
            var idDevuelto = connection.ExecuteScalar<int>(sql, new
            {
                Titulo = playlist.Titulo,
                Descripcion = playlist.Descripcion,
                FechaCreacion = playlist.FechaCreacion,
                ClienteId = playlist.ClienteId,
                PortadaPlaylist = playlist.PortadaPlaylist
            });
            playlist.Id = idDevuelto;
            
            return CreatedAtAction(nameof(GetPlaylistById), new { id = idDevuelto }, playlist);

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
