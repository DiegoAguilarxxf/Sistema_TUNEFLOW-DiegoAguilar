using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Modelos
{
    public class Country
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Continent { get; set; }
        public string Currency { get; set; }
    }

}
