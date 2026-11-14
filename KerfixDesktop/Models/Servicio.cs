
namespace KerfixDesktop.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public decimal Valor { get; set; }
    }
}
