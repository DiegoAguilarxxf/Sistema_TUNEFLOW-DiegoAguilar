using System;
using System.Collections.Generic;
using System.Data.Common;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Modelos.Tuneflow.Models;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Profiles;
using Modelos.Tuneflow.User.Production;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly DbConnection connection;

        public ProfilesController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Profiles
        [HttpGet]
        public IEnumerable<Profile> GetPerfiles()
        {
            return connection.Query<Profile>("SELECT * FROM \"Profiles\"");
        }

        // GET: api/Profiles/5
        [HttpGet("{id}")]
        public ActionResult<Profile> GetPerfilById(int id)
        {
            var profile = connection.QuerySingleOrDefault<Profile>(
                @"SELECT * FROM ""Profiles"" WHERE ""Id"" = @Id",
                new { Id = id });

            if (profile == null)
                return NotFound();

            return profile;
        }

        // GET: api/Profiles/User/ByClient/5
        [HttpGet("User/ByClient/{idClient}")]
        public ActionResult<Profile> ObtenerPerfilPorClienteId(int idClient)
        {
            var sql = @"SELECT * FROM ""Profiles"" WHERE ""ClientId"" = @Id";
            var profile = connection.QueryFirstOrDefault<Profile>(sql, new { Id = idClient });

            if (profile == null)
                return NotFound();

            if (profile.ClientId != null && profile.ClientId != 0)
            {
                var clientSql = @"SELECT ""FirstName"", ""LastName"" FROM ""Clients"" WHERE ""Id"" = @Id";
                profile.Client = connection.QueryFirstOrDefault<Client>(clientSql, new { Id = profile.ClientId });
            }

            return profile;
        }

        // GET: api/Profiles/User/ByArtist/5
        [HttpGet("User/ByArtist/{idArtist}")]
        public ActionResult<Profile> ObtenerPerfilPorArtistaId(int idArtist)
        {
            var sql = @"SELECT * FROM ""Profiles"" WHERE ""ArtistId"" = @Id";
            var profile = connection.QueryFirstOrDefault<Profile>(sql, new { Id = idArtist });

            if (profile == null)
                return NotFound();

            if (profile.ArtistId != null && profile.ArtistId != 0)
            {
                var artistSql = @"SELECT * FROM ""Artists"" WHERE ""Id"" = @Id";
                profile.Artist = connection.QueryFirstOrDefault<Artist>(artistSql, new { Id = profile.ArtistId });
            }

            return profile;
        }

        // PUT: api/Profiles/5
        [HttpPut("{id}")]
        public ActionResult PutPerfil(int id, [FromBody] Profile profile)
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

            return NoContent();
        }

        // POST: api/Profiles
        [HttpPost]
        public ActionResult<Profile> PostPerfil([FromBody] Profile profile)
        {
            if (profile == null)
                return BadRequest("Datos inválidos.");

            string sql;
            object parametros;

            if (profile.ArtistId == 0 && profile.ClientId != 0)
            {
                sql = @"INSERT INTO ""Profiles"" (""ClientId"", ""ProfileImage"", ""Biography"", ""CreationDate"")
                        VALUES (@ClientId, @ProfileImage, @Biography, @CreationDate)
                        RETURNING ""Id"";";

                parametros = new
                {
                    ClientId = profile.ClientId,
                    ProfileImage = profile.ProfileImage,
                    Biography = profile.Biography,
                    CreationDate = profile.CreationDate
                };
            }
            else if (profile.ClientId == 0 && profile.ArtistId != 0)
            {
                sql = @"INSERT INTO ""Profiles"" (""ArtistId"", ""ProfileImage"", ""Biography"", ""CreationDate"")
                        VALUES (@ArtistId, @ProfileImage, @Biography, @CreationDate)
                        RETURNING ""Id"";";

                parametros = new
                {
                    ArtistId = profile.ArtistId,
                    ProfileImage = profile.ProfileImage,
                    Biography = profile.Biography,
                    CreationDate = profile.CreationDate
                };
            }
            else
            {
                return BadRequest("Debe especificar solo un tipo de perfil (Cliente o Artista).");
            }

            var id = connection.ExecuteScalar<int>(sql, parametros);
            profile.Id = id;

            return CreatedAtAction(nameof(GetPerfilById), new { id = id }, profile);
        }

        // DELETE: api/Profiles/5
        [HttpDelete("{id}")]
        public ActionResult DeletePerfil(int id)
        {
            var filasAfectadas = connection.Execute(@"DELETE FROM ""Profiles"" WHERE ""Id"" = @Id", new { Id = id });

            if (filasAfectadas == 0)
                return NotFound();

            return NoContent();
        }
    }
}
