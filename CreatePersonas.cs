using System.ComponentModel.DataAnnotations;

namespace Candidate
{
    public class CreatePersonas
    {
        public int IdPersona {  get; set; }  
        public string Nombre { get; set; } = default!;

       
        public string Telefono { get; set; } = default!;

       
        public string Direccion { get; set; } = default!;
        public string Cedula { get; set; } = default!;
        public string Provincia { get; set; } = default!;
    }
}
