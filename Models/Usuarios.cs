using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public string Borrado { get; set; } = "No";

        public ICollection<Usuario_Provincia> UsuarioProvincias { get; set; } = new List<Usuario_Provincia>();

    }
}
