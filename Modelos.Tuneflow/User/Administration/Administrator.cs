using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Usuario.Administracion
{
    public class Administrator : User
    {
        public string Description { get; set; } // Descripción del administrador, puede incluir su rol o responsabilidades.
        public string UserId { get; set; } // Identificador único del usuario administrador, puede ser un GUID o un ID de base de datos.
    }

}
