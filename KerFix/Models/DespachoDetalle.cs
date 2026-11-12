
namespace KerFix.Models
{
    public class DespachoDetalle
    {
        public int Id { get; set; }

        public int DespachoId { get; set; }

        public int? InventarioRepuestoId { get; set; }

        public int? ServicioId { get; set; }

        public int Cantidad { get; set; }

        public decimal Valor { get; set; }
    }
}
