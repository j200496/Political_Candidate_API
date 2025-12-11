using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class Provincias
    {
        [Key]
        public int IdProvincia { get; set; }
        public string Nombre { get; set; } = "";
        public string Borrado { get; set; } = "No";
        public int Meta { get; set; }
        public ICollection<Usuario_Provincia> UsuarioProvincias { get; set; } = new List<Usuario_Provincia>();

    }
}
