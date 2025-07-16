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

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {/*
        private readonly TUNEFLOWContext _context;

        public ClientesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        private DbConnection connection;
        public ClientsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Clientes
        [HttpGet]
        public IEnumerable<Cliente> GetCliente()
        {
           var cliente= connection.Query<Cliente>("SELECT * FROM \"Clientes\"");
            return cliente;
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public ActionResult<Cliente> GetClienteById(int id)
        {
            var cliente = connection.QuerySingleOrDefault<Cliente>(@"SELECT * FROM ""Clientes"" WHERE ""Id"" = @Id", new { Id = id });

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("Usuario/{idUsuario}")]
        public ActionResult<Cliente> GetClienteByUsuarioId(string idUsuario)
        {
            var cliente = connection.QuerySingleOrDefault<Cliente>(@"SELECT * FROM ""Clientes"" WHERE ""UsuarioId"" = @UsuarioId", new { UsuarioId = idUsuario });
            if (cliente == null)
            {
                return NotFound();
            }
            return cliente;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutCliente(int id,[FromBody] Cliente cliente)
        {
            connection.Execute(@"UPDATE ""Clientes"" SET 
                ""Nombre"" = @Nombre,
                ""Apellido"" = @Apellido,
                ""Email"" = @Email,
                ""FechaNacimiento"" = @FechaNacimiento,
                ""PaisId"" = @PaisId,
                ""SuscripcionId"" = @SuscripcionId
                WHERE ""Id"" = @Id", new
            {
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email,
                FechaNacimiento = cliente.FechaNacimiento,
                PaisId = cliente.PaisId,
                SuscripcionId = cliente.SuscripcionId,
                Id = id
            });
              
        }

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Cliente> PostCliente([FromBody]Cliente cliente)
        {
            var sql = @"INSERT INTO ""Clientes"" (""PaisId"", ""SuscripcionId"", ""Nombre"", ""Apellido"", ""Email"", ""Password"",""Telefono"",""FechaNacimiento"",""TipoCuenta"",""Activo"",""FechaRegistro"",""UsuarioId"") 
                VALUES (@PaisId, @SuscripcionId, @Nombre, @Apellido, @Email, @Password, @Telefono, @FechaNacimiento, @TipoCuenta, @Activo, @FechaRegistro, @UsuarioId) RETURNING ""Id"";";
            
            var idDevuelto = connection.ExecuteScalar<int>(sql,new
           {
               PaisId = cliente.PaisId,
               SuscripcionId = cliente.SuscripcionId,
               Nombre = cliente.Nombre,
               Apellido = cliente.Apellido,
               Email = cliente.Email,
               Password = cliente.Password,
               Telefono = cliente.Telefono,
               FechaNacimiento = cliente.FechaNacimiento,
               TipoCuenta = cliente.TipoCuenta,
               Activo = cliente.Activo,
               FechaRegistro = cliente.FechaRegistro,
               UsuarioId = cliente.UsuarioId

            });

            cliente.Id = idDevuelto;

            return CreatedAtAction(nameof(GetClienteById), new { id = idDevuelto }, cliente);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public void DeleteCliente(int id)
        {
            connection.Execute(@"DELETE FROM ""Clientes"" WHERE ""Id"" = @Id", new { Id = id });
        }
/*
        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }*/
    }
}
