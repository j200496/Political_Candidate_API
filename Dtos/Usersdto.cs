using System.ComponentModel.DataAnnotations;

namespace Candidate.Dtos
{
    public class Usersdto
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
