using System;
using System.Data.Common;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Modelos.Tuneflow.Media;
using Npgsql;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReproductorController : ControllerBase
    {
        
        private readonly IConfiguration _config;

        public ReproductorController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("play")]
        public IActionResult ReproducirCancion([FromQuery] int songId, [FromQuery] int clientId)
        {

            try
            {
                using var _connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
                _connection.Open();
                var playback = new Playback
                {
                    PlaybackDate = DateTime.UtcNow,
                    ClientId = clientId,
                    SongId = songId
                };

                const string sql = @"INSERT INTO ""Playbacks"" (""PlaybackDate"", ""ClientId"", ""SongId"")
                     VALUES (@PlaybackDate, @ClientId, @SongId)";


                _connection.Execute(sql, new
                {
                    PlaybackDate = playback.PlaybackDate,
                    ClientId = playback.ClientId,
                    SongId = playback.SongId
                });


                return Ok(new { message = "🎵 Reproducción registrada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "❌ Error al registrar la reproducción", details = ex.Message });
            }
        }
    }
}
