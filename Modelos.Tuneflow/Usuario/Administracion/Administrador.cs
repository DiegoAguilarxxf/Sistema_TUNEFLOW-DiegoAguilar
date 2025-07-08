using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Usuario.Administracion
{
    public class Administrador : Usuario
    {
        public string Descripcion { get; set; } // Descripción del administrador, puede incluir su rol o responsabilidades.
        public string UsuarioId { get; set; } // Identificador único del usuario administrador, puede ser un GUID o un ID de base de datos.


    }
}
