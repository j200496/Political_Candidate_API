namespace Candidate.Models
{
    public class MiembroDto
    {
        public int IdMiembro { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string CaptadoPor { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
