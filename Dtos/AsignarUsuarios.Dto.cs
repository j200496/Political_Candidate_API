namespace Candidate.Dtos
{
    public class AsignarUsuarios
    {
        public int IdProvincia { get; set; }
        public List<int> Usuarios { get; set; } = new List<int>();
    }
}
