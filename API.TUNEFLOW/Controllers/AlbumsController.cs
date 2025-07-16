using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.Usuario.Administracion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private DbConnection connection;

       public AlbumsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Albums
        [HttpGet]
        public IEnumerable<Album> Get()
        {
            var albums = connection.Query<Album>("SELECT * FROM \"Albums\"");
            return albums;
        }

        // GET: api/Albums/5
        [HttpGet("{id}")]
        public ActionResult<Album> Get(int id)
        {
            var albums = connection.QuerySingle<Album>(@"SELECT * FROM ""Albums"" WHERE ""Id""=@Id", new {Id= id });

            if (albums == null)
            {
                return NotFound();
            }

            return albums;
        }

        // PUT: api/Albums/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void Put(int id,[FromBody] Album album)
        {
            connection.Execute(
                @"UPDATE ""Albums"" SET 
                ""Title"" = @Title, 
                ""ReleaseDate"" = @ReleaseDate, 
                ""Genre"" = @Genre, 
               ""CreationDate"" = @CreationDate, 
                ""Descripction"" = @Description, 
                ""CoverPath"" = @CoverPath
                WHERE ""Id"" = @Id",
                new
                {
                    Titulo= album.Title,
                    FechaLanzamiento=album.ReleaseDate,
                    Genero = album.Genre,
                    FechaCreacion = album.CreationDate,
                    Descripcion = album.Description,
                    RutaPortada = album.CoverPath,
                    Id = id

                });
        }

        // POST: api/Albums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Album Post([FromBody]Album album)
        {
            connection.Execute(
                @"Insert INTO ""Albums"" (""Title"", ""ReleaseDate"", ""Genre"", ""FechaCreacion"", ""Description"", ""CoverPath"")VALUES
                (@Title, @ReleaseDate, @Genre, @CreationDate, @Description, @CoverPath",
                new
                {
                    Title= album.Title,
                    ReleaseDate = album.ReleaseDate,
                    Genre = album.Genre,
                    CreationDate = album.CreationDate,
                    Description = album.Description,
                    CoverPath = album.CoverPath


                });
            return album;
        }

        // DELETE: api/Albums/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
           connection.Execute(@"DELETE FROM ""Albums"" WHERE ""Id"" = @Id", new { Id = id });
        }

       /* private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }*/
    }
}
