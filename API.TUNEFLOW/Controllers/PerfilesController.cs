using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Usuario.Perfiles;
using Modelos.Tuneflow.Usuario.Produccion;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilesController : ControllerBase
    {
        /*private readonly TUNEFLOWContext _context;

        public PerfilesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/

        private DbConnection connection;    

        public PerfilesController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Perfiles
        [HttpGet]
        public IEnumerable<Perfil> GetPerfil()
        {
            var perfiles = connection.Query<Perfil>("SELECT * FROM \"Perfiles\"");
            return perfiles;
        }

        // GET: api/Perfiles/5
        [HttpGet("{id}")]
        public ActionResult<Perfil> GetPerfilById(int id)
        {
            var perfil = connection.QuerySingleOrDefault<Perfil>(@"SELECT * FROM ""Perfiles"" WHERE ""Id"" = @Id", new { Id = id });

            if (perfil == null)
            {
                return NotFound();
            }

            return perfil;
        }

        [HttpGet("Usuario/Obtencion/{idAmbos}")]
        public ActionResult<Perfil> ObtenerPerfilPorClienteId(int idAmbos)
        {
            var sql = @"SELECT * FROM ""Perfiles"" 
                WHERE ""ClienteId"" = @Id OR ""ArtistaId"" = @Id";

            var perfil = connection.QueryFirstOrDefault<Perfil>(sql, new { Id = idAmbos });

            if (perfil == null)
                return NotFound();

            // Si tiene ClienteId, obtener Cliente
            if (perfil.ClienteId != null && perfil.ClienteId != 0)
            {
                var clienteSql = @"SELECT ""Id"", ""Nombre"", ""Apellido"" FROM ""Clientes"" WHERE ""Id"" = @Id";
                perfil.Cliente = connection.QueryFirstOrDefault<Cliente>(clienteSql, new { Id = perfil.ClienteId });
            }

            // Si tiene ArtistaId, obtener Artista
            if (perfil.ArtistaId != null && perfil.ArtistaId != 0)
            {
                var artistaSql = @"SELECT ""Id"", ""NombreArtistico"" FROM ""Artistas"" WHERE ""Id"" = @Id";
                perfil.Artista = connection.QueryFirstOrDefault<Artista>(artistaSql, new { Id = perfil.ArtistaId });
            }

            return perfil;
        }

        // PUT: api/Perfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult PutPerfil(int id,[FromBody] Perfil perfil)
        {
            if (perfil == null || perfil.Id != id)
                return BadRequest("ID en URL no coincide con el perfil.");

            connection.Execute(@"UPDATE ""Perfiles"" SET 
                            ""ImagenPerfil"" = @ImagenPerfil,
                            ""Biografia"" = @Biografia
                         WHERE ""Id"" = @Id", new
            {
                ImagenPerfil = perfil.ImagenPerfil,
                Biografia = perfil.Biografia,
                Id = id
            });

            return NoContent(); // HTTP 204
        }

        // POST: api/Perfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Perfil> PostPerfil([FromBody] Perfil perfil)
        { 
            var idDevuelto = 0;

            if (perfil.ArtistaId == 0)
            {
                var sql = @"INSERT INTO ""Perfiles"" (""ClienteId"", ""ImagenPerfil"", ""Biografia"", ""FechaCreacion"")
                            VALUES
                            (@ClienteId,@ImagenPerfil,@Biografia,@FechaCreacion) RETURNING ""Id"";";

                idDevuelto = connection.ExecuteScalar<int>(sql, new
                {
                    ClienteId = perfil.ClienteId,
                    ImagenPerfil = perfil.ImagenPerfil,
                    Biografia = perfil.Biografia,
                    FechaCreacion = perfil.FechaCreacion
                });
                perfil.Id = idDevuelto;

            }
            else if(perfil.ClienteId == 0)
            {
                var sql = @"INSERT INTO ""Perfiles"" (""ArtistaId"", ""ImagenPerfil"", ""Biografia"", ""FechaCreacion"")
                            VALUES
                            (@ArtistaId,@ImagenPerfil,@Biografia,@FechaCreacion) RETURNING ""Id"";";
                idDevuelto = connection.ExecuteScalar<int>(sql, new
                {
                    ArtistaId = perfil.ArtistaId,
                    ImagenPerfil = perfil.ImagenPerfil,
                    Biografia = perfil.Biografia,
                    FechaCreacion = perfil.FechaCreacion
                });
                perfil.Id = idDevuelto;
            }

                return CreatedAtAction(nameof(GetPerfilById), new { id = idDevuelto }, perfil);
        }

        // DELETE: api/Perfiles/5
        [HttpDelete("{id}")]
        public void DeletePerfil(int id)
        {
            connection.Execute(@"DELETE FROM ""Perfiles"" WHERE ""Id"" = @Id", new { Id = id });
        }
        /*
        private bool PerfilExists(int id)
        {
            return _context.Perfiles.Any(e => e.Id == id);
        }*/
    }
}
