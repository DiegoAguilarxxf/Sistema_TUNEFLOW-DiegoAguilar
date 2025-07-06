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
using Modelos.Tuneflow.Playlist;
using Modelos.Tuneflow.Usuario.Produccion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancionesController : ControllerBase
    {
        /*rivate readonly TUNEFLOWContext _context;

        public CancionesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public CancionesController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Canciones
        [HttpGet]
        public IEnumerable<Cancion> GetCancion()
        {   var cancion= connection.Query<Cancion>("SELECT * FROM \"Canciones\"");
            return cancion;
        }

        // GET: api/Canciones/5
        [HttpGet("{id}")]
        public ActionResult<Cancion> GetCancion(int id)
        {   var cancion = connection.QuerySingle<Cancion>(@"SELECT * FROM ""Canciones"" WHERE ""Id"" = @Id", new { Id = id });
            return cancion;
        }

        [HttpGet("Titulo/{titulo}")]
        public ActionResult<IEnumerable<Cancion>> GetCancionByTitulo(string titulo)
        {
            string sql = @"
                            SELECT 
                                c.""Id"", c.""Titulo"", c.""Duracion"", c.""Genero"", c.""RutaArchivo"", c.""ContenidoExplicito"",
                                al.""Titulo"" AS AlbumTitulo,
                                ar.""NombreArtistico"" AS NombreArtistico
                            FROM ""Canciones"" c
                            LEFT JOIN ""Albums"" al ON c.""AlbumId"" = al.""Id""
                            LEFT JOIN ""Artistas"" ar ON c.""ArtistaId"" = ar.""Id""
                            WHERE c.""Titulo"" ILIKE @Titulo";

            var canciones = connection.Query<Cancion, string, string, Cancion>(
                sql,
                (cancion, albumTitulo, nombreArtistico) =>
                {
                    cancion.Album = new Album { Titulo = albumTitulo };
                    cancion.Artista = new Artista { NombreArtistico = nombreArtistico };
                    return cancion;
                },
                new { Titulo = $"%{titulo}%" },
                splitOn: "AlbumTitulo,NombreArtistico"
            ).ToList();

            if (!canciones.Any())
                return NotFound("No se encontraron canciones con ese título.");

            return Ok(canciones);
        }


        // PUT: api/Canciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutCancion(int id,[FromBody]Cancion cancion)
        {
            connection.Execute(@"UPDATE ""Canciones"" SET 
                ""Titulo"" = @Titulo, 
                ""Duracion"" = @Duracion, 
                ""Genero"" = @Genero, 
                ""FechaLanzamiento"" = @FechaLanzamiento, 
                ""ArtistaId"" = @ArtistaId, 
                ""AlbumId"" = @AlbumId, 
                ""RutaArchivo"" = @RutaArchivo, 
                ""ContenidoExplicito"" = @ContenidoExplicito
            WHERE ""Id"" = @Id", new
            {
                                   Id = id,
                Titulo = cancion.Titulo,
                Duracion = cancion.Duracion,
                Genero = cancion.Genero,
                ArtistaId = cancion.ArtistaId,
                AlbumId = cancion.AlbumId,
                RutaArchivo = cancion.RutaArchivo,
                ContenidoExplicito = cancion.ContenidoExplicito
            });
        }

        // POST: api/Canciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Cancion PostCancion([FromBody]Cancion cancion)
        {
            connection.Execute(@"INSERT INTO ""Canciones"" 
                (""Titulo"", ""Duracion"", ""Genero"", ""FechaLanzamiento"", ""ArtistaId"", ""AlbumId"", ""RutaArchivo"", ""ContenidoExplicito"") 
                VALUES (@Titulo, @Duracion, @Genero, @FechaLanzamiento, @ArtistaId, @AlbumId, @RutaArchivo, @ContenidoExplicito)", new
            {
               Titulo= cancion.Titulo,
                Duracion= cancion.Duracion,
               Genero= cancion.Genero,
               ArtistaId= cancion.ArtistaId,
               AlbumId= cancion.AlbumId,
               RutaArchivo= cancion.RutaArchivo,
               ContenidoExplicito= cancion.ContenidoExplicito
            });

            return cancion;
        }

        // DELETE: api/Canciones/5
        [HttpDelete("{id}")]
        public void DeleteCancion(int id)
        {
           connection.Execute(@"DELETE FROM ""Canciones"" WHERE ""Id"" = @Id", new { Id = id });    
        }
/*
        private bool CancionExists(int id)
        {
            return _context.Canciones.Any(e => e.Id == id);
        }*/
    }
}
