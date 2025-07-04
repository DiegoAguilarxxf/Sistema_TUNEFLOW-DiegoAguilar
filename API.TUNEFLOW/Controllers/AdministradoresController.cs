using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Usuario.Administracion;
using Dapper;


namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradoresController : ControllerBase
    {   private DbConnection connection;
        
        public AdministradoresController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }
       

        // GET: api/Administradores
        [HttpGet]
        public IEnumerable<Administrador> Get()
        {
            var administradores = connection.Query<Administrador>("SELECT * FROM \"Administradores\"");
            return administradores;
        }

        // GET: api/Administradores/5
        [HttpGet("{id}")]
        public ActionResult <Administrador> Get(int id)
        {
            var administrador = connection.QuerySingle<Administrador>(@"SELECT * FROM ""Administradores"" WHERE ""Id"" = @Id", new { Id=id });
          

            if (administrador == null)
            {
                return NotFound();
            }

            return administrador;
        }

        // PUT: api/Administradores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void  Put(int id,[FromBody] Administrador administrador)
        {
            connection.Execute(@"UPDATE ""Administradores"" SET 
             ""Descripcion"" = @Descripcion,
            ""Nombre""= @Nombre,
            ""Apellido"" = @Apellido,
            ""Email"" = @Email,
            ""Password"" = @Password,
            ""Telefono"" = @Telefono,
            ""FechaNacimiento"" = @FechaNacimiento,
            ""TipoCuenta"" = @TipoCuenta,
            ""Activo"" = @Activo,
            ""FechaRegistro"" = @FechaRegistro
        WHERE ""Id"" = @Id",
            new
                { Descripcion=administrador.Descripcion,
                Nombre = administrador.Nombre,
                Apellido = administrador.Apellido,
                Email = administrador.Email,
                Password = administrador.Password,
                Telefono = administrador.Telefono,
                FechaNacimiento = administrador.FechaNacimiento,
                TipoCuenta = administrador.TipoCuenta,
                Activo = administrador.Activo,
                FechaRegistro = administrador.FechaRegistro,
                Id = id


            });
        }
        // POST: api/Administradores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Administrador Post([FromBody]Administrador administrador)
        {
       
   connection.Execute(
    @"INSERT INTO ""Administradores""
     (""Descripcion"", ""Nombre"", ""Apellido"", ""Email"", ""Password"", ""Telefono"", ""FechaNacimiento"", ""TipoCuenta"", ""Activo"", ""FechaRegistro"")
    VALUES 
     (@Descripcion, @Nombre, @Apellido, @Email, @Password, @Telefono, @FechaNacimiento, @TipoCuenta, @Activo, @FechaRegistro)",
    new
    {
    Descripcion = administrador.Descripcion,
    Nombre = administrador.Nombre,
    Apellido = administrador.Apellido,
    Email = administrador.Email,
    Password = administrador.Password,
    Telefono = administrador.Telefono,
    FechaNacimiento = administrador.FechaNacimiento,
    TipoCuenta = administrador.TipoCuenta,
    Activo = administrador.Activo,
    FechaRegistro = administrador.FechaRegistro
     });
    return administrador;
        }

        // DELETE: api/Administradores/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            connection.Execute(@"DELETE FROM ""Administradores"" WHERE ""Id"" = @Id", new { Id = id });
        }

       /* private bool AdministradorExists(int id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }*/
    }
}
