namespace Candidate.Dtos
{
    public class MiembroDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public int IdProvincia { get; set; }
    }
}
