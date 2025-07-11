﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Usuario.Perfiles;

namespace Modelos.Tuneflow.Usuario.Consumidor
{
    public class Cliente : Usuario
    {
        [ForeignKey(nameof(Pais))]public int PaisId  { get; set; }
        [ForeignKey(nameof(Suscripcion))] public int SuscripcionId { get; set; }

        public string UsuarioId { get; set; } // Identificador único del usuario cliente, puede ser un GUID o un ID de base de datos.
        public Suscripcion? Suscripcion { get; set; }

        public Pais? Pais { get; set; }

    }
}
