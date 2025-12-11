using System.ComponentModel.DataAnnotations;

namespace Candidate.Dtos
{
    public class ProvinciasDto
    {
        [Key]
        public int IdProvincia { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Meta {  get; set; }
    }
}
