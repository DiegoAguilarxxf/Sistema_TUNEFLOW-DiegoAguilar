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
    public class ArtistsController : ControllerBase
    {
        private DbConnection connection;
        /*private readonly TUNEFLOWContext _context;

        public ArtistasController(TUNEFLOWContext context)
        {
            _context = context;
        }*/

        // GET: api/Artistas
        public ArtistsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }
        [HttpGet]
        public IEnumerable<Artist> GetArtista()
        {
           var artistas = connection.Query<Artist>("SELECT * FROM \"Artists\"");
            return artistas;
        }

        // GET: api/Artistas/5
        [HttpGet("{id}")]
        public ActionResult<Artist> GetArtistaById(int id)
        {
            var artista = connection.QuerySingleOrDefault<Artist>(@"SELECT * FROM ""Artists"" WHERE ""Id"" = @Id", new { Id = id });

            if (artista == null)
            {
                return NotFound();
            }

            return artista;
        }

        [HttpGet("UsuarioArtista/{idUser}")]
        public ActionResult<Artist> GetArtistaByUsuarioId(string idUser)
        {
            var artista = connection.QuerySingleOrDefault<Artist>(@"SELECT * FROM ""Artists"" WHERE ""UserId"" = @UserId", new { UserId = idUser });
            if (artista == null)
            {
                return NotFound();
            }
            return artista;
        }

        // PUT: api/Artistas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void  PutArtista(int id,[FromBody] Artist artist)
        {
           connection.Execute(@"UPDATE ""Artistas"" SET 
                ""StageName"" = @StageName, 
                ""MusicGenre"" = @MusicGenre,  
                ""CountryId"" = @CountryId, 
                ""Verified"" = @Verified
                WHERE ""Id"" = @Id", new
           {
               artist.StageName,
               artist.MusicGenre,
               artist.CountryId,
               artist.Verified,
               Id = id
           });
        }

        // POST: api/Artistas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Artist> PostArtista([FromBody]Artist artist)
        {

            var sql = @"INSERT INTO ""Artists"" 
                (""StageName"",""MusicGenre"",""CountryId"",""Verified"",""UserId"",""FirstName"",""LastName"",""Email"",""Password"",""Phone"",""BirthDate"",""AccountType"",""IsActive"",""RegistrationDate"") 
                VALUES (@StageName, @MusicGenre, @CountryId, @Verified,@UserId, @FirstName, @LastName, @Email, @Password, @Phone, @BirthDate, @AccountType, @IsActive, @RegistrationDate) RETURNING ""Id"";";
                
            var idreturned = connection.ExecuteScalar<int>(sql,new
                {
                    StageName = artist.StageName,
                    MusicGenre = artist.MusicGenre,
                    CountryId = artist.CountryId,
                    Verified = artist.Verified,
                    UserId = artist.UserId,
                    FirstName = artist.FirstName,
                    LastName= artist.LastName,
                    Email = artist.Email,
                    Password = artist.Password,
                    Phone = artist.Phone,
                    BirthDate = artist.BirthDate,
                    AccountType = artist.AccountType,
                    IsActive = artist.IsActive,
                    RegistrationDate = artist.RegistrationDate
            });

            artist.Id = idreturned;

            return CreatedAtAction(nameof(GetArtistaById), new { id = idreturned }, artist);
        }

        // DELETE: api/Artistas/5
        [HttpDelete("{id}")]
        public void DeleteArtista(int id)
        { connection.Execute(@"DELETE FROM ""Artistas"" WHERE ""Id"" = @Id", new { Id = id });

        }
/*
        private bool ArtistaExists(int id)
        {
            return _context.Artistas.Any(e => e.Id == id);
        }*/
    }
}
