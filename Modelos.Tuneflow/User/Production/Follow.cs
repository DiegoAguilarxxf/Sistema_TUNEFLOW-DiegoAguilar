using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Usuario.Consumidor;

namespace Modelos.Tuneflow.Usuario.Produccion
{
    public class Follow
    {
        [Key] public int Id { get; set; }

        [ForeignKey(nameof(Client))] public int ClientId { get; set; }

        [ForeignKey(nameof(Artist))] public int ArtistId { get; set; }

        public Client? Client { get; set; }
        public Artist? Artist { get; set; }
    }

}
