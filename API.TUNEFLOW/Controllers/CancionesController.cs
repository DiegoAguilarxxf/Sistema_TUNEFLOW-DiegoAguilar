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

        [HttpGet("Titulo")]
        public IEnumerable<Cancion> GetCancionByTitulo(string titulo)
        {
            var canciones = connection.Query<Cancion>(
            @"SELECT * FROM ""Canciones"" WHERE ""Titulo"" ILIKE @Titulo",  
            new { Titulo = "%" + titulo + "%" }
    );
            return canciones;
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
