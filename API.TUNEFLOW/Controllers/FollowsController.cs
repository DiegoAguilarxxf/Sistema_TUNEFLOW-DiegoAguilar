using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Production;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
       /* private readonly TUNEFLOWContext _context;

        public SeguimientosController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
       private DbConnection connection;
        public FollowsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Seguimientos
        [HttpGet]
        public IEnumerable<Follow> GetSeguimiento()
        { var follows = connection.Query<Follow>("SELECT * FROM \"Follows\"");
            return follows;
        }

        // GET: api/Seguimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Follow>> GetSeguimientoById(int id)
        {
            var follow = connection.QuerySingle<Follow>(@"SELECT * FROM ""Follows"" WHERE ""Id"" = @Id", new { Id = id });

            if (follow == null)
            {
                return NotFound();
            }

            return follow;
        }

        [HttpGet("ObtenerIsFollowed/{idClient}/{idArtist}")]
        public async Task<ActionResult<int>> GetIdSeguimientoExist(int idClient, int idArtist)
        {
            var sql = @"SELECT ""Id"" FROM ""Follows"" WHERE ""ClientId"" = @IdClient AND ""ArtistId""=@IdArtist";

            var followId = connection.QuerySingleOrDefault<int?>(sql, new { IdClient = idClient, IdArtist = idArtist });

            if (followId.HasValue)
            {
                return Ok(followId.Value);
            }
            else
            {
                return Ok(0); // o -1, o NotFound() si prefieres
            }

        }

        // PUT: api/Seguimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutSeguimiento(int id,[FromBody] Follow follow)
        {
            connection.Execute(@"UPDATE ""Follows"" SET
                ""ClientId""=@ClientId,
                ""ArtistId""=@ArtistId WERE ""Id""=@Id", new
            {
                ClientId = follow.ClientId,
                ArtistId =  follow.ArtistId
            });

        }

        // POST: api/Seguimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Follow>  PostSeguimiento([FromBody]Follow follow)
        {
            var sql = @"INSERT INTO ""Follows"" (""ClientId"", ""ArtistId"") 
                VALUES (@ClientId, @ArtistId) RETURNING ""Id""";

            var idResult = connection.ExecuteScalar<int>(sql, new
            {
                ClientId = follow.ClientId,
                ArtistId = follow.ArtistId
            });
            follow.Id = idResult;

            return CreatedAtAction(nameof(GetSeguimientoById), new { id = idResult }, follow);
        }

        // DELETE: api/Seguimientos/5
        [HttpDelete("{id}")]
        public void DeleteSeguimiento(int id)
        {connection.Execute(@"DELETE FROM ""Follows"" WHERE ""Id"" = @Id", new { Id = id });
            
        }
        /*
        private bool SeguimientoExists(int id)
        {
            return _context.Seguimientos.Any(e => e.Id == id);
        }*/
    }
}
