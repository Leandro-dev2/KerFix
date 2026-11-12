
namespace KerFix.Models
{
    public class InventarioRepuesto
    {
        public int Id { get; set; }

        public string TipoRepuesto { get; set; } = string.Empty;

        public string Especificacion { get; set; } = string.Empty;

        public string Capacidad { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public int Cantidad { get; set; }
    }
}
