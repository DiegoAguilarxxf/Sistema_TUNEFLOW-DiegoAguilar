using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Usuario.Produccion;

namespace Modelos.Tuneflow.Usuario.Administracion
{
    public class ArtistStatistics
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Artist))] public int ArtistId { get; set; }
        public int TotalPlays { get; set; }
        public int TotalFollowers { get; set; }
        public int PublishedSongs { get; set; }
        public int PublishedAlbums { get; set; }

        public Artist? Artist { get; set; }
    }

}
