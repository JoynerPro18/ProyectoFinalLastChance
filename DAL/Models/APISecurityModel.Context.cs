﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class APISegurityEntities6 : DbContext
    {
        public APISegurityEntities6()
            : base("name=APISegurityEntities6")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblUsuario> tblUsuarios { get; set; }
    
        public virtual ObjectResult<Nullable<bool>> Usuariovalidar(string usuario, string clave)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(string));
    
            var claveParameter = clave != null ?
                new ObjectParameter("Clave", clave) :
                new ObjectParameter("Clave", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<bool>>("Usuariovalidar", usuarioParameter, claveParameter);
        }
    }
}