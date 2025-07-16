using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Media;

namespace Modelos.Tuneflow.Playlists
{
    public class Playlist
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; } // Playlist title
        public string Description { get; set; } // Playlist description
        public DateTime CreationDate { get; set; } // Playlist creation date
        [ForeignKey(nameof(Client))] public int ClientId { get; set; }
        public string PlaylistCover { get; set; }
        public Client? Client { get; set; }
        public List<Song>? Songs { get; set; } // List of songs that belong to this playlist
    }

}
