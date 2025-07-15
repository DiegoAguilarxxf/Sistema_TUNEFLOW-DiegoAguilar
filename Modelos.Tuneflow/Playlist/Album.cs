using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Media;

namespace Modelos.Tuneflow.Playlist
{
    public class Album
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; } // Pop, Rock, Hip-Hop, etc.
        public DateOnly CreationDate { get; set; } // Album creation date
        public string Description { get; set; } // Album description
        public string CoverPath { get; set; } // Path to the album cover image
        public List<Song> Songs { get; set; } // List of songs that belong to this album
    }

}
