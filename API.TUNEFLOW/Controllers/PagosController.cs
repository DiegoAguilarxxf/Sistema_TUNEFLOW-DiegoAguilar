using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Pagos;

namespace API.TUNEFLOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
       /* private readonly TUNEFLOWContext _context;

        public PagosController(TUNEFLOWContext context)
        {
            _context = context;
        */

        private DbConnection connection;
        public PagosController(IConfiguration config)
        {
            var connString = config.GetConnectionString("TUNEFLOWContext");
            connection = new Npgsql.NpgsqlConnection(connString);
            connection.Open();
        }

        // GET: api/Pagos
        [HttpGet]
        public IEnumerable<Pago> GetPago()
        {   var pagos = connection.Query<Pago>("SELECT * FROM \"Pagos\"");
            return pagos;
        }

        // GET: api/Pagos/5
        [HttpGet("{id}")]
        public ActionResult<Pago> GetPago(int id)
        {
            var pago = connection.QuerySingle<Pago>(@"SELECT * FROM ""Pagos"" WHERE ""Id"" = @Id", new { Id = id });

            if (pago == null)
            {
                return NotFound();
            }

            return pago;
        }

        // PUT: api/Pagos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public void PutPago(int id, [FromBody] Pago pago)
        {
            connection.Execute(@"UPDATE ""Pagos"" SET 
                ""ClienteId"" = @ClienteId,
                ""FechaPago"" = @FechaPago,
                ""Monto"" = @Monto,
                ""MetodoPago"" = @MetodoPago
                WHERE ""Id"" = @Id", new
            {
                Id = id,
                ClienteId = pago.ClienteId,
                FechaPago = pago.FechaPago,
                Monto = pago.Monto,
                MetodoPago = pago.MetodoPago
            });
        }

        // POST: api/Pagos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public Pago PostPago([FromBody]Pago pago)
        { connection.Execute(@"INSERT INTO ""Pagos"" (""ClienteId"", ""FechaPago"", ""Monto"", ""MetodoPago"") 
                VALUES (@ClienteId, @FechaPago, @Monto, @MetodoPago) RETURNING ""Id""", new
        {
            ClienteId = pago.ClienteId,
            FechaPago = pago.FechaPago,
            Monto = pago.Monto,
            MetodoPago = pago.MetodoPago
        });
            return pago;
        }

        // DELETE: api/Pagos/5
        [HttpDelete("{id}")]
        public void DeletePago(int id)
        {
           connection.Execute(@"DELETE FROM ""Pagos"" WERE ""Id"" = @Id", new { Id = id });
           
        }
        /*
        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }*/
    }
}
