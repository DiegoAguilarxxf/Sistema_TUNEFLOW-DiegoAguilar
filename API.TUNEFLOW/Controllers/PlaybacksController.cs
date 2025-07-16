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

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaybacksController : ControllerBase
    {
        /* private readonly TUNEFLOWContext _context;

         public ReproduccionesController(TUNEFLOWContext context)
         {
             _context = context;
         }*/
        private DbConnection connection;
        public PlaybacksController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Reproducciones
        [HttpGet]
        public IEnumerable<Playback> GetReproduccion()
        { var playbacks = connection.Query<Playback>("SELECT * FROM \"Playbacks\"");
            return playbacks;
        }

        // GET: api/Reproducciones/5
        [HttpGet("{id}")]
        public ActionResult<Playback> GetReproduccion(int id)
        {
            var playback = connection.QuerySingle<Playback>(@"SELECT * FROM ""Playbacks"" WHERE ""Id"" = @Id", new { Id = id });

            if (playback == null)
            {
                return NotFound();
            }

            return playback;
        }

        // PUT: api/Reproducciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void Reproduccion(int id,[FromBody] Playback playback)
        {
            connection.Execute(@"UPDATE ""Playbacks"" SET 
                ""DateTime"" = @DateTime,
                ""ClientId"" = @ClientId,
                ""SongId"" = @SongId
            WHERE ""Id"" = @Id", new
            {
                PlaybackDate = playback.PlaybackDate,
                ClientId = playback.ClientId,
                SongId = playback.SongId,
                Id = id
            });
        }

        // POST: api/Reproducciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Playback PostReproduccion([FromBody]Playback playback)
        {
            connection.Execute(@"INSERT INTO ""PLaybacks"" (""DateTime"", ""ClientId"", ""SongId"")
VALUES (@DateTime, @ClientId, @SongId)", new
            {
                PlaybackDate = playback.PlaybackDate,
                ClientId = playback.ClientId,
                SongId = playback.SongId
            });
            return playback;
        }

        // DELETE: api/Reproducciones/5
        [HttpDelete("{ id}")]
        public void DeleteReproduccion(int id)
        {
            connection.Execute( @"DELETE FROM ""Playbacks"" WHERE ""Id"" = @Id", new { Id = id });
        }
/*
        private bool ReproduccionExists(int id)
        {
            return _context.Reproducciones.Any(e => e.Id == id);
        }*/
    }
}
