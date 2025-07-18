using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Usuario.Consumidor
{
    public class Subscription
    {
        [Key] public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? JoinCode { get; set; } // Código de unión para planes familiares, si es aplicable.
        [ForeignKey(nameof(SubscriptionType))] public int SubscriptionTypeId { get; set; }
        public SubscriptionType? SubscriptionType { get; set; }

        public List<Client>? Members { get; set; }
    }

}
