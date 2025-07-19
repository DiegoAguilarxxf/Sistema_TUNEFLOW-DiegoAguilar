using Modelos.Tuneflow.User.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Production;

namespace Modelos.Tuneflow.User.Profiles
{
    public class Profile
    {
        [Key] public int Id { get; set; } // Identificador único del perfil

        [ForeignKey(nameof(Client))] public int? ClientId { get; set; } // Identificador del cliente al que pertenece el perfil

        [ForeignKey(nameof(Artist))] public int? ArtistId { get; set; } // Identificador del artista al que pertenece el perfil
        public Client? Client { get; set; } // Relación con el cliente (opcional)
        public Artist? Artist { get; set; } // Relación con el artista (opcional)
        public string ProfileImage { get; set; } // URL o ruta de la imagen del perfil
        public string Biography { get; set; } // Breve biografía o descripción del perfil
        public DateTime CreationDate { get; set; } // Fecha de creación del perfil
    }

}
