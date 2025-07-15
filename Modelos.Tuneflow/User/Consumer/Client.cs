using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Usuario.Perfiles;

namespace Modelos.Tuneflow.Usuario.Consumidor
{
    public class Client : User
    {
        [ForeignKey(nameof(Country))] public int CountryId { get; set; }
        [ForeignKey(nameof(Subscription))] public int SubscriptionId { get; set; }

        public string UserId { get; set; } // Identificador único del usuario cliente, puede ser un GUID o un ID de base de datos.
        public Subscription? Subscription { get; set; }

        public Country? Country { get; set; }
    }

}
