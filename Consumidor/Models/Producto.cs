using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Consumidor.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public Nullable<int> Precio { get; set; }
        public string Categoria { get; set; }
    }
}