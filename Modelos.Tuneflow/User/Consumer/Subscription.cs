using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.User.Consumer;

namespace Modelos.Tuneflow.User.Consumer
{
    public class Subscription
    {
        [Key] public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? JoinCode { get; set; } // Código de unión para planes familiares, si es aplicable.
        [ForeignKey(nameof(SubscriptionType))] public int SubscriptionTypeId { get; set; }
        public int NumberMembers { get; set; } // Número de miembros en la suscripción, especialmente para planes familiares.
        public SubscriptionType? SubscriptionType { get; set; }

        public List<Client>? Members { get; set; }
    }

}
