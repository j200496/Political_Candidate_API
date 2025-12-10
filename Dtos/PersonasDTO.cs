namespace Candidate.Dtos
{
    public class PersonasDTO
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public int IdProvincia { get; set; }
       public int IdUsuario { get; set;}
    }
}
