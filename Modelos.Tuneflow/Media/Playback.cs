using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.User.Consumer;

namespace Modelos.Tuneflow.Media
{
    public class Playback
    {
        [Key] public int Id { get; set; }

        [ForeignKey(nameof(Client))] public int ClientId { get; set; }

        [ForeignKey(nameof(Song))] public int SongId { get; set; }

        public DateTime PlaybackDate { get; set; }

        public Client? Client { get; set; }
        public Song? Song { get; set; }
    }

}
