using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Playlist;
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
                ""Titulo"" = @Titulo, 
                ""FechaLanzamiento"" = @FechaLanzamiento, 
                ""Genero"" = @Genero, 
               ""FechaCreacion"" = @FechaCreacion, 
                ""Descripcion"" = @Descripcion, 
                ""RutaPortada"" = @RutaPortada 
                WHERE ""Id"" = @Id",
                new
                {
                   Titulo= album.Titulo,
                    FechaLanzamiento=album.FechaLanzamiento,
                    Genero = album.Genero,
                    FechaCreacion = album.FechaCreacion,
                    Descripcion = album.Descripcion,
                    RutaPortada = album.RutaPortada,
                    Id = id

                });
        }

        // POST: api/Albums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Album Post([FromBody]Album album)
        {
            connection.Execute(
                @"Insert INTO ""Albums"" (""Titulo"", ""FechaLanzamiento"", ""Genero"", ""FechaCreacion"", ""Descripcion"", ""RutaPortada"")VALUES
                (@Titulo, @FechaLanzamiento, @Genero, @FechaCreacion, @Descripcion, @RutaPortada)",
                new
                {
                    Titulo= album.Titulo,
                    FechaLanzamiento = album.FechaLanzamiento,
                    Genero = album.Genero,
                    FechaCreacion = album.FechaCreacion,
                    Descripcion = album.Descripcion,
                    RutaPortada = album.RutaPortada


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
