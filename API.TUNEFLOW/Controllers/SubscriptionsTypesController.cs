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
    public class SubscriptionsTypesController : ControllerBase
    {
        /* private readonly TUNEFLOWContext _context;

         public TiposSuscripcionesController(TUNEFLOWContext context)
         {
             _context = context;
         }
        */
        private DbConnection connection;
        public SubscriptionsTypesController(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }
        // GET: api/TiposSuscripciones
        [HttpGet]
        public IEnumerable<SubscriptionType> GetTipoSuscripcion()
        {
            var subscriptionTypes = connection.Query<SubscriptionType>("SELECT * FROM \"SubscriptionsTypes\"");
            return subscriptionTypes;
        }

        // GET: api/TiposSuscripciones/5
        [HttpGet("{id}")]
        public ActionResult<SubscriptionType> GetTipoSuscripcion(int id)
        {
            var subscriptionType = connection.QuerySingle<SubscriptionType>(@"SELECT * FROM ""SubscriptionsTypes"" WHERE ""Id"" = @Id", new { Id = id });

            if (subscriptionType == null)
            {
                return NotFound();
            }

            return subscriptionType;
        }

        // PUT: api/TiposSuscripciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutTipoSuscripcion(int id,[FromBody] SubscriptionType subscriptionType)
        {
            connection.Execute(@"UPDATE ""SubscriptionsTypes"" SET 
                ""Name"" = @Name,
                ""Price"" = @Price,
                ""MemberLimit"" = @MemberLimit,
                WHERE ""Id"" = @Id", new
            {
                Id = id,
                Name = subscriptionType.Name,
                Price = subscriptionType.Price,
                MemberLimit= subscriptionType.MemberLimit
            });
        }

        // POST: api/TiposSuscripciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public SubscriptionType PostTipoSuscripcion([FromBody]SubscriptionType subscriptionType)
        {
           connection.Execute(@"INSERT INTO ""SubscriptionsTypes"" (""Name"", ""Price"", ""MembersLimit"") 
                VALUES (@Name, @Price, @MembersLimit)", new
           {
               Name = subscriptionType.Name,
               Price = subscriptionType.Price,
               MemberLimit = subscriptionType.MemberLimit
           });
            return subscriptionType;
        }

        // DELETE: api/TiposSuscripciones/5
        [HttpDelete("{id}")]
        public void DeleteTipoSuscripcion(int id)
        {
           connection.Execute(@"DELETE FROM ""SubscriptionsTypes"" WHERE ""Id"" = @Id", new { Id = id });
         
        }
        /*
        private bool TipoSuscripcionExists(int id)
        {
            return _context.TiposSuscripciones.Any(e => e.Id == id);
        }*/
    }
}
