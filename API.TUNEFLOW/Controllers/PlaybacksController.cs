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
        private readonly DbConnection connection;

        public PlaybacksController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Playbacks
        [HttpGet]
        public IEnumerable<Playback> GetPlaybacks()
        {
            return connection.Query<Playback>("SELECT * FROM \"Playbacks\"");
        }

        // GET: api/Playbacks/5
        [HttpGet("{id}")]
        public ActionResult<Playback> GetPlayback(int id)
        {
            var playback = connection.QueryFirstOrDefault<Playback>(
                @"SELECT * FROM ""Playbacks"" WHERE ""Id"" = @Id", new { Id = id });

            if (playback == null)
                return NotFound();

            return playback;
        }

        // POST: api/Playbacks
        [HttpPost]
        public ActionResult<Playback> PostPlayback([FromBody] Playback playback)
        {
            connection.Execute(
                @"INSERT INTO ""Playbacks"" (""PlaybackDate"", ""ClientId"", ""SongId"") 
              VALUES (@PlaybackDate, @ClientId, @SongId)", playback);

            return Ok(playback);
        }

        // PUT: api/Playbacks/5
        [HttpPut("{id}")]
        public IActionResult PutPlayback(int id, [FromBody] Playback playback)
        {
            var rowsAffected = connection.Execute(
                @"UPDATE ""Playbacks"" SET 
                ""PlaybackDate"" = @PlaybackDate,
                ""ClientId"" = @ClientId,
                ""SongId"" = @SongId
              WHERE ""Id"" = @Id", new
                {
                    playback.PlaybackDate,
                    playback.ClientId,
                    playback.SongId,
                    Id = id
                });

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Playbacks/5
        [HttpDelete("{id}")]
        public IActionResult DeletePlayback(int id)
        {
            var rowsAffected = connection.Execute(
                @"DELETE FROM ""Playbacks"" WHERE ""Id"" = @Id", new { Id = id });

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }

}
