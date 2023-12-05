using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Models;

namespace DAL.Clases
{
    public class APISecurity
    {
        private static APISegurityEntities6 db = new APISegurityEntities6();

        public static Nullable<bool> Usuariovalidar(string Usuario, string Clave)
        {
            var result = db.Usuariovalidar(Usuario, Clave).FirstOrDefault();
            return result;
        }
    }
}