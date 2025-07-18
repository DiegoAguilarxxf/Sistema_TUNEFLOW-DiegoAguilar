using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.Usuario.Perfiles;

namespace Modelos.Tuneflow.Usuario.Produccion
{
    public class Artist : User
    {
        public string StageName { get; set; }
        public string MusicGenre { get; set; }
        public string Biography { get; set; }
        [ForeignKey(nameof(Country))] public int CountryId { get; set; }
        public bool Verified { get; set; } // Indica si el artista ha sido verificado por la plataforma.
        public string UserId { get; set; } // Identificador único del usuario artista, puede ser un GUID o un ID de base de datos.
        public Country? Country { get; set; }
        public List<Song>? Songs { get; set; }
        public List<Album>? Albums { get; set; }
       // public Profile? Profile { get; set; } // Relación con el perfil del artista
        public string? ProfileImage { get; set; } // URL o ruta de la imagen del perfil del artista
    }

}
