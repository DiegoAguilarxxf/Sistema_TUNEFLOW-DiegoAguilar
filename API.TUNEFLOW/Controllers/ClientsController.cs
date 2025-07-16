using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.ExceptionServices;
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
        public IEnumerable<Client> GetCliente()
        {
           var cliente= connection.Query<Client>("SELECT * FROM \"Clients\"");
            return cliente;
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public ActionResult<Client> GetClienteById(int id)
        {
            var client = connection.QuerySingleOrDefault<Client>(@"SELECT * FROM ""Clients"" WHERE ""Id"" = @Id", new { Id = id });

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpGet("Usuario/{idUsuario}")]
        public ActionResult<Client> GetClienteByUsuarioId(string idUser)
        {
            var client = connection.QuerySingleOrDefault<Client>(@"SELECT * FROM ""Clients"" WHERE ""UserId"" = @UserId", new { UserId = idUser });
            if (client == null)
            {
                return NotFound();
            }
            return client;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutCliente(int id,[FromBody] Client client)
        {
            connection.Execute(@"UPDATE ""Clients"" SET 
                ""FirstName"" = @FirstName,
                ""LastName"" = @LastName,
                ""Email"" = @Email,
                ""BirthDate"" = @BirthDate,
                ""CountryId"" = @CountryId,
                ""SubscriptionId"" = @SubscriptionId
                WHERE ""Id"" = @Id", new
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                BirthDate = client.BirthDate,
                CountryId = client.CountryId,
                SubscriptionId = client.SubscriptionId,
                Id = id
            });
              
        }

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Client> PostCliente([FromBody]Client client)
        {
            var sql = @"INSERT INTO ""Clients"" (""CountryId"", ""SubscriptionId"", ""FirstName"", ""LastName"", ""Email"", ""Password"",""Phone"",""BirthDate"",""AccountType"",""IsActive"",""RegistrationDate"",""UserId"") 
                VALUES (@CountryId, @SubscriptionId, @FirstName, @LastName, @Email, @Password, @Phone, @BirthDate, @AccountType, @IsActive, @RegistrationDate, @UserId) RETURNING ""Id"";";
            
            var idReturned = connection.ExecuteScalar<int>(sql,new
           {
               CountryId = client.CountryId,
               SubscriptionId = client.SubscriptionId,
               FirstName = client.FirstName,
               LastName = client.LastName,
               Email = client.Email,
               Password = client.Password,
               Phone = client.Phone,
               BirthDate = client.BirthDate,
               AccountType = client.AccountType,
               IsActive = client.IsActive,
               RegistrationDate = client.RegistrationDate,
               UserId = client.UserId

            });

            client.Id = idReturned;

            return CreatedAtAction(nameof(GetClienteById), new { id = idReturned }, client);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public void DeleteCliente(int id)
        {
            connection.Execute(@"DELETE FROM ""Clients"" WHERE ""Id"" = @Id", new { Id = id });
        }
/*
        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }*/
    }
}
