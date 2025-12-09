using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class Provincias
    {
        [Key]
        public int IdProvincia { get; set; }
        public string Nombre { get; set; } = "";
        public string Borrado { get; set; } = "No";
        public ICollection<Usuario_Provincia> UsuarioProvincias { get; set; } = new List<Usuario_Provincia>();

    }
}
