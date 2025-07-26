using Modelos.Tuneflow.User.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Models
{
    public class ArtistVerificationRequest
    {
        public int Id { get; set; }

        public int ArtistId { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public virtual Artist Artist { get; set; } = null!;
    }

}
