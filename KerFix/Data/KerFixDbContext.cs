
using Microsoft.EntityFrameworkCore;
using KerFix.Models;

namespace KerFix.Data
{
    public class KerFixDbContext : DbContext
    {
        public KerFixDbContext(
            DbContextOptions<KerFixDbContext> options
        ) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Registro> Registros { get; set; }
        public DbSet<Despacho> Despachos { get; set; }
        public DbSet<DespachoDetalle> DespachoDetalles { get; set; }
        public DbSet<InventarioRepuesto> InventarioRepuestos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("cliente");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre");

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono");
            });

            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("servicio");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Valor)
                    .HasColumnName("valor");
            });
        }
    }
}
