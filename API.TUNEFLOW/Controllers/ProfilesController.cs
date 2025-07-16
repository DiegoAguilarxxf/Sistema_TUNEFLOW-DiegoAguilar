using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Usuario.Perfiles;
using Modelos.Tuneflow.Usuario.Produccion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        /*private readonly TUNEFLOWContext _context;

        public PerfilesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/

        private DbConnection connection;    

        public ProfilesController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Perfiles
        [HttpGet]
        public IEnumerable<Profile> GetPerfil()
        {
            var perfiles = connection.Query<Profile>("SELECT * FROM \"Profiles\"");
            return perfiles;
        }

        // GET: api/Perfiles/5
        [HttpGet("{id}")]
        public ActionResult<Profile> GetPerfilById(int id)
        {
            var profile = connection.QuerySingleOrDefault<Profile>(@"SELECT * FROM ""Profiles"" WHERE ""Id"" = @Id", new { Id = id });

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }

        [HttpGet("User/Obtainingn/{idClient}")]
        public ActionResult<Profile> ObtenerPerfilPorClienteId(int idClient)
        {
            var sql = @"SELECT * FROM ""Profiles"" 
                WHERE ""ClientId"" = @Id";

            var profile = connection.QueryFirstOrDefault<Profile>(sql, new { Id = idClient });

            if (profile == null)
                return NotFound();

            // Si tiene ClienteId, obtener Cliente con sus relaciones
            if (profile.ClientId != null && profile.ClientId != 0)
            {
                var clientSql = @"SELECT ""FirstName"", ""LastName"" FROM ""Clients"" WHERE ""Id"" = @Id";
                profile.Client = connection.QueryFirstOrDefault<Client>(clientSql, new { Id = profile.ClientId });
            }

            return profile;
        }

        [HttpGet("User/Obtainingn/{idArtist}")]
        public ActionResult<Profile> ObtenerPerfilPorArtistaId(int idArtist)
        {
            var sql = @"SELECT * FROM ""Profiles"" 
                WHERE ""ArtistId"" = @Id";

            var profile = connection.QueryFirstOrDefault<Profile>(sql, new { Id = idArtist });

            if (profile == null)
                return NotFound();


            if (profile.ArtistId != null && profile.ArtistId != 0)
            {
                var artistSql = @"SELECT ""StageName"" FROM ""Artists"" WHERE ""Id"" = @Id";
                profile.Artist = connection.QueryFirstOrDefault<Artist>(artistSql, new { Id = profile.ArtistId });
            }

            return profile;
        }


            // PUT: api/Perfiles/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut("{id}")]
        public ActionResult PutPerfil(int id,[FromBody] Profile profile)
        {
            if (profile == null || profile.Id != id)
                return BadRequest("ID en URL no coincide con el perfil.");

            connection.Execute(@"UPDATE ""Profiles"" SET 
                            ""ProfileImage"" = @ProfileImage,
                            ""Biography"" = @Biography
                         WHERE ""Id"" = @Id", new
            {
                ProfileImage = profile.ProfileImage,
                Biography = profile.Biography,
                Id = id
            });

            return NoContent(); // HTTP 204
        }

        // POST: api/Perfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Profile> PostPerfil([FromBody] Profile profile)
        { 
            var idDevuelto = 0;

            if (profile.ArtistId == 0)
            {
                var sql = @"INSERT INTO ""Profiles"" (""ClientId"", ""ProfileImage"", ""Biography"", ""CreationDate"")
                            VALUES
                            (@ClientId,@ProfileImage,@Biography,@CreationDate) RETURNING ""Id"";";

                idDevuelto = connection.ExecuteScalar<int>(sql, new
                {
                    ClientId = profile.ClientId,
                    ProfileImage = profile.ProfileImage,
                    Biography = profile.Biography,
                    CreationDate = profile.CreationDate
                });
                profile.Id = idDevuelto;

            }
            else if(profile.ClientId == 0)
            {
                var sql = @"INSERT INTO ""Profiles"" (""ArtistId"", ""ProfileImage"", ""Biography"", ""CreationDate"")
                            VALUES
                            (@ArtistId,@ProfileImage,@Biography,@CreationDate) RETURNING ""Id"";";
                idDevuelto = connection.ExecuteScalar<int>(sql, new
                {
                    ArtistId = profile.ArtistId,
                    ProfileImage = profile.ProfileImage,
                    Biography = profile.Biography,
                    CreationDate = profile.CreationDate
                });
                profile.Id = idDevuelto;
            }

                return CreatedAtAction(nameof(GetPerfilById), new { id = idDevuelto }, profile);
        }

        // DELETE: api/Perfiles/5
        [HttpDelete("{id}")]
        public void DeletePerfil(int id)
        {
            connection.Execute(@"DELETE FROM ""Profiles"" WHERE ""Id"" = @Id", new { Id = id });
        }
        /*
        private bool PerfilExists(int id)
        {
            return _context.Perfiles.Any(e => e.Id == id);
        }*/
    }
}
