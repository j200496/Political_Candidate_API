using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class Usuario_Provincia
    {
        
        public int IdUsuario { get; set; } 
        public int IdProvincia { get; set; }
       public Usuarios Usuario { get; set; }
        public Provincias Provincia { get; set; }

    }
}
