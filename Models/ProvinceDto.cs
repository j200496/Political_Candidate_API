using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class ProvinceDto
    {
        [Key]
        public int IdProvincia { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}
