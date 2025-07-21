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
using Npgsql;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaybacksController : ControllerBase
    {
        
        private readonly IConfiguration _config;
        public PlaybacksController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/Playbacks
        [HttpGet]
        public IEnumerable<Playback> GetPlaybacks()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            return connection.Query<Playback>("SELECT * FROM \"Playbacks\"");
        }

        // GET: api/Playbacks/5
        [HttpGet("{id}")]
        public ActionResult<Playback> GetPlayback(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
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
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            connection.Execute(
                @"INSERT INTO ""Playbacks"" (""PlaybackDate"", ""ClientId"", ""SongId"") 
              VALUES (@PlaybackDate, @ClientId, @SongId)", playback);

            return Ok(playback);
        }

        // PUT: api/Playbacks/5
        [HttpPut("{id}")]
        public IActionResult PutPlayback(int id, [FromBody] Playback playback)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
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
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            var rowsAffected = connection.Execute(
                @"DELETE FROM ""Playbacks"" WHERE ""Id"" = @Id", new { Id = id });

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }

}
