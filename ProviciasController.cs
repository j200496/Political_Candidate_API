using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Candidate
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviciasController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        public ProviciasController(AppDbContext appDbContext)
        {
           _dbcontext = appDbContext;
        }
        //Metodo que devuelve la lista de todas las provincias de RD
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provincias>>> GetProvincias()
            => await _dbcontext.Provincias.ToListAsync();

        //Metodo para añadir provincias
        [HttpPost]
        public async Task<ActionResult<Provincias>> PostProvincia([FromBody] CreateProvincias dto)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);

            var prov = new Provincias
            {
                IdProvincia = dto.IdProvincia,
                Nombre = dto.Nombre,

            };
            if (prov.IdProvincia > 0)
            {
                _dbcontext.Entry(prov).State = EntityState.Modified;
            }
            else
            {
                _dbcontext.Provincias.Add(prov);
            }

            await _dbcontext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProvincias), new { id = prov.IdProvincia }, prov);
        }
    }
}
