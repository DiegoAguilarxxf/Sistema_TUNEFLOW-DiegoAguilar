using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Media;

namespace Modelos.Tuneflow.Playlists
{
    public class SongPlaylist
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Song))] public int SongId { get; set; }
        [ForeignKey(nameof(Playlist))] public int PlaylistId { get; set; }
        public Song? Song { get; set; } // Relationship with the song that belongs to this playlist
        public Playlist? Playlist { get; set; } // Relationship with the playlist this song belongs to
    }

}
