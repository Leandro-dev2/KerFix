
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

            // TABLA CLIENTES
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("cliente");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
            });

            // TABLA SERVICIOS
            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("servicio");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Tipo).HasColumnName("tipo");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Valor).HasColumnName("valor");
            });

            // TABLA INVENTARIO
            modelBuilder.Entity<InventarioRepuesto>(entity =>
            {
                entity.ToTable("inventario_repuesto");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TipoRepuesto).HasColumnName("tipo_repuesto");
                entity.Property(e => e.Especificacion).HasColumnName("especificacion");
                entity.Property(e => e.Capacidad).HasColumnName("capacidad");
                entity.Property(e => e.Precio).HasColumnName("precio");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            });

            // TABLA REGISTROS
            modelBuilder.Entity<Registro>(entity =>
            {
                entity.ToTable("registro");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
                entity.Property(e => e.FechaRecepcion).HasColumnName("fecha_recepcion");
                entity.Property(e => e.Tipo).HasColumnName("tipo");
                entity.Property(e => e.Marca).HasColumnName("marca");
                entity.Property(e => e.Modelo).HasColumnName("modelo");
                entity.Property(e => e.Serie).HasColumnName("serie");
                entity.Property(e => e.DiagnosticoCliente).HasColumnName("diagnostico_cliente");
                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.HasOne<Cliente>()
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente);
            });

            // TABLA DESPACHOS
            modelBuilder.Entity<Despacho>(entity =>
            {
                entity.ToTable("despacho");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RegistroId).HasColumnName("registro_id");

                entity.Property(e => e.FechaTermino)
                    .HasColumnName("fecha_termino")
                    .HasColumnType("timestamp without time zone");

                entity.Property(e => e.ObsRecom).HasColumnName("obs_recom");
                entity.Property(e => e.ValorTotal).HasColumnName("valor_total");

                entity.HasOne<Registro>()
                    .WithMany()
                    .HasForeignKey(e => e.RegistroId);
            });

            // TABLA DETALLES DE DESPACHO
            modelBuilder.Entity<DespachoDetalle>(entity =>
            {
                entity.ToTable("despacho_detalle");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DespachoId).HasColumnName("despacho_id");
                entity.Property(e => e.InventarioRepuestoId).HasColumnName("inventario_repuesto_id");
                entity.Property(e => e.ServicioId).HasColumnName("servicio_id");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.Valor).HasColumnName("valor");

                entity.HasOne<Despacho>()
                    .WithMany()
                    .HasForeignKey(e => e.DespachoId);

                entity.HasOne<InventarioRepuesto>()
                    .WithMany()
                    .HasForeignKey(e => e.InventarioRepuestoId);

                entity.HasOne<Servicio>()
                    .WithMany()
                    .HasForeignKey(e => e.ServicioId);
            });
        }
    }
}
