using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.User.Administration;
using Dapper;
using Npgsql;


namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {   
        private readonly IConfiguration _config;
        public AdministratorsController(IConfiguration config)
        {
            _config = config;
        }
       

        // GET: api/Administradores
        [HttpGet]
        public IEnumerable<Administrator> Get()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            var administrators = connection.Query<Administrator>("SELECT * FROM \"Administrators\"");
            return administrators;
        }

        // GET: api/Administradores/5
        [HttpGet("{id}")]
        public ActionResult <Administrator> Get(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            var administrator = connection.QuerySingle<Administrator>(@"SELECT * FROM ""Administrators"" WHERE ""Id"" = @Id", new { Id=id });
          

            if (administrator == null)
            {
                return NotFound();
            }

            return administrator;
        }

        // PUT: api/Administradores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void  Put(int id,[FromBody] Administrator administrator)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            connection.Execute(@"UPDATE ""Administrators"" SET 
             ""Description"" = @Description,
            ""FirstName""= @FirstName,
            ""LastName"" = @LastName,
            ""Email"" = @Email,
            ""Password"" = @Password,
            ""Phone"" = @Phone,
            ""BirthDate"" = @BirthDate,
            ""AccountType"" = @AccountType,
            ""IsActive"" = @IsActive,
            ""RegistrationDate"" = @RegistrationDate
        WHERE ""Id"" = @Id",
            new
                { Description=administrator.Description,
                FirstName = administrator.FirstName,
                LastName = administrator.LastName,
                Email = administrator.Email,
                Password = administrator.Password,
                Phone = administrator.Phone,
                BirthDate = administrator.BirthDate,
                AccountType= administrator.AccountType,
                IsActive = administrator.IsActive,
                RegistrationDate = administrator.RegistrationDate,
                Id = id


            });
        }
        // POST: api/Administradores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Administrator Post([FromBody]Administrator administrator)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();

            connection.Execute(
    @"INSERT INTO ""Administrators""
     (""Description"", ""FirstName"", ""LastName"", ""Email"", ""Password"", ""Phone"", ""BirthDate"", ""AccountType"", ""IsActive"", ""RegistrationDate"")
    VALUES 
     (@Description, @FirstName, @LastName, @Email, @Password, @Phone, @BirthDate, @AccountType, @IsActve, @RegistrationDate)",
    new
    {
        Description = administrator.Description,
        FirstName = administrator.FirstName,
        LastName = administrator.LastName,
        Email = administrator.Email,
        Password = administrator.Password,
        Phone = administrator.Phone,
        BirthDate = administrator.BirthDate,
        AccountType = administrator.AccountType,
        IsActive = administrator.IsActive,
        RegistrationDate = administrator.RegistrationDate,
    });
    return administrator;
        }

        // DELETE: api/Administradores/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("TUNEFLOWContext"));
            connection.Open();
            connection.Execute(@"DELETE FROM ""Administrators"" WHERE ""Id"" = @Id", new { Id = id });
        }

       /* private bool AdministradorExists(int id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }*/
    }
}
