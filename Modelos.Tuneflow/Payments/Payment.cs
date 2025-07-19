using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.User.Consumer;

namespace Modelos.Tuneflow.Payments
{
    public class Payment
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Client))] public int ClientId { get; set; } // Identifier of the user who made the payment
        public DateTime PaymentDate { get; set; } // Date the payment was made
        public double Amount { get; set; } // Payment amount
        public string PaymentMethod { get; set; } // Payment method used (credit card, PayPal, etc.)
    }

}
