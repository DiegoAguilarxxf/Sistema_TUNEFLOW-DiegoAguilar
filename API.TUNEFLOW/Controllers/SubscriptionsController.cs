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

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private DbConnection connection;
        /*private readonly TUNEFLOWContext _context;

        public SuscripcionesController(TUNEFLOWContext context)
        {
            _context = context;
        }*/
        public SubscriptionsController(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Suscripciones
        [HttpGet]
        public IEnumerable<Subscription> GetSuscripcion()
        { var suscripciones = connection.Query<Subscription>("SELECT * FROM \"Subscriptions\"");
            return suscripciones;
        }

        // GET: api/Suscripciones/5
        [HttpGet("{id}")]
        public ActionResult<Subscription> GetSuscripcionById(int id)
        {
            var subscription = connection.QuerySingleOrDefault<Subscription>(@"SELECT * FROM ""Subscriptions"" WHERE ""Id"" = @Id", new { Id = id });

            if (subscription == null)
            {
                return NotFound();
            }

            return subscription;
        }

        // PUT: api/Suscripciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutSuscripcion(int id, [FromBody] Subscription subscription)
        {
            var affectedRows = connection.Execute(@"UPDATE ""Subscriptions"" SET 
                ""StartDate"" = @StartDate,
                ""JoinCode"" = @JoinCode,
                ""SubscriptionTypeId"" = @SubscriptionTypeId,
                ""NumberMembers"" = @NumberMembers
                 WHERE ""Id"" = @Id",
             new
             {
                 Id = id,
                 StartDate = subscription.StartDate,
                 JoinCode= subscription.JoinCode,
                 SubscriptionTypeId = subscription.SubscriptionTypeId,
                 NumberMembers = subscription.NumberMembers
             }

            );

            if (affectedRows == 0)
                return NotFound(); // No se encontró la suscripción con ese ID

            return NoContent(); // Actualización exitosa, sin contenido que devolver

        }

        // POST: api/Suscripciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Subscription> PostSuscripcion([FromBody] Subscription subscription)
        {
            var sql = @"INSERT INTO ""Subscriptions"" (""StartDate"", ""SubscriptionTypeId"",""NumberMembers"") 
                VALUES (@StartDate, @SubscriptionTypeId, @NumberMembers) 
                RETURNING ""Id"";";

            int idDevuelto = connection.ExecuteScalar<int>(sql, new
            {
                StartDate = subscription.StartDate,
                SubscriptionTypeId = subscription.SubscriptionTypeId,
                NumberMembers = subscription.NumberMembers
            });

            subscription.Id = idDevuelto;

            return CreatedAtAction(nameof(GetSuscripcionById), new { id = idDevuelto }, subscription);
        }
        // DELETE: api/Suscripciones/5
        [HttpDelete("{id}")]
        public void DeleteSuscripcion(int id)
        {
          connection.Execute(@"DELETE FROM ""Subscriptions"" WHERE ""Id"" = @Id", new { Id = id });
        }
/*
        private bool SuscripcionExists(int id)
        {
            return _context.Suscripciones.Any(e => e.Id == id);
        }*/
    }
}
