using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Consumidor.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public Nullable<int> IdUsuario { get; set; }
        public Nullable<int> IdProducto { get; set; }
        public Nullable<int> Cantidad { get; set; }
    }
}