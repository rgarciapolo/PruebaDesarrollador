using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace PruebaTecnica.Models
{
    public class DBSystemDBContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<dbo_Usuario>().ToTable("dbo.Usuario");
            modelBuilder.Entity<dbo_Ciudad>().ToTable("dbo.Ciudad");
            modelBuilder.Entity<dbo_TipoDocumento>().ToTable("dbo.TipoDocumento");
        }

        public DbSet<dbo_Usuario> dbo_Usuario { get; set; }

        public DbSet<dbo_Ciudad> dbo_Ciudad { get; set; }

        public DbSet<dbo_TipoDocumento> dbo_TipoDocumento { get; set; }

    }
}
 
