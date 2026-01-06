using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Candidate.Models
{
    public class Personas
    {
        [Key]
        public int IdPersona { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public char Genero { get; set;} 
        public string Borrado { get; set; } = "No";

       // [ForeignKey("Provincia")]
        public int IdProvincia { get; set; }

       // [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Provincias Provincia { get; set; }

        public Usuarios Usuario { get; set; }


    }
}
