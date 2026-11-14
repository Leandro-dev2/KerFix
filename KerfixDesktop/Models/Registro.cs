
namespace KerfixDesktop.Models
{
    public class Registro
    {
        public int Id { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaRecepcion { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string? Marca { get; set; }

        public string? Modelo { get; set; }

        public string Serie { get; set; } = string.Empty;

        public string DiagnosticoCliente { get; set; } = string.Empty;

        public string Estado { get; set; } = "Pendiente";
    }
}
