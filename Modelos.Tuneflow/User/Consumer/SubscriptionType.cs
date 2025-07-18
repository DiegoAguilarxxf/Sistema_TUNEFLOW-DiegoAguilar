using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Usuario.Consumidor
{
    public class SubscriptionType
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }  // Free, Premium, Familiar
        public double Price { get; set; }
        public int MemberLimit { get; set; }
    }

}
