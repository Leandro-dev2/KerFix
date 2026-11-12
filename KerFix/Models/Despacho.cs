
namespace KerFix.Models
{
    public class Despacho
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        public DateTime FechaTermino { get; set; }

        public string? ObsRecom { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
