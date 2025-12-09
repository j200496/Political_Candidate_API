namespace Candidate.Dtos
{
    public class AsignacionDTO
    {
        public int IdUsuario { get; set; }
        public List<int> Provincias { get; set; } = new List<int>();
    }
}
