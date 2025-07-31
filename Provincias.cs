using System.ComponentModel.DataAnnotations;

namespace Candidate
{
    public class Provincias
    {
        [Key]
        public int IdProvincia {  get; set; }
        public string Nombre { get; set; } = "";
    }
}
