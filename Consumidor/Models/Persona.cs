using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Consumidor.Models
{
    public class Persona
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public Nullable<int> Telefono { get; set; }

    }
}