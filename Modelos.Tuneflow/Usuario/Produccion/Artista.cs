using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Usuario.Perfiles;

namespace Modelos.Tuneflow.Usuario.Produccion
{
    public class Artista : Usuario
    {
        public string NombreArtistico { get; set; }
        public string GeneroMusical { get; set; }
        public string Biografia { get; set; }
        [ForeignKey(nameof(Pais))] public int PaisId { get; set; }
        public bool verificado { get; set; } // Indica si el artista ha sido verificado por la plataforma.
        public string UsuarioId { get; set; } // Identificador único del usuario artista, puede ser un GUID o un ID de base de datos.
        public Pais? Pais { get; set; }

    }
}
