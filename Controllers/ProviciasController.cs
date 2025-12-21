using Candidate.Dtos;
using Candidate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;

namespace Candidate.Controllers
{
    // [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProviciasController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        public ProviciasController(AppDbContext appDbContext, IConfiguration configuration)
        {
            _dbcontext = appDbContext;
            _configuration = configuration;
        }
        //Metodo que devuelve la lista de todoss los territorios
       // [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provincias>>> GetProvincias()
            => await _dbcontext.Provincias.Where(p => p.Borrado == "No").ToListAsync();

        [Authorize]
        [HttpGet("Lista-prov/{idUsuario}")]
        public async Task<ActionResult> GetProvasig(int idUsuario)
        {
            var ids = await _dbcontext.Usuario_Provincia.Where(x => x.IdUsuario == idUsuario).Select(x => x.IdProvincia).ToListAsync();
            return Ok(ids);
        }
        [Authorize]
        [HttpGet("Getpropu")]
        public async Task<IActionResult> GetPropu(int id)
        {
            var provincias = await _dbcontext.Usuario_Provincia
                .Where(up => up.IdUsuario == id)
                .Join(
                    _dbcontext.Provincias,
                    up => up.IdProvincia,
                    p => p.IdProvincia,
                    (up, p) => new
                    {
                        p.IdProvincia,
                        p.Nombre,
                        p.Borrado
                    }
                )
                .Where(x => x.Borrado == "No")
                .ToListAsync();

            return Ok(provincias);
        }
        [HttpGet("miembros-por-provincia")]
        public async Task<IActionResult> GetMiembrosPorProvincia()
        {
            var datos =  await _dbcontext.Provincias.Where(p => p.Borrado == "No").Select(p => new
            {
                provincia = p.Nombre,
                cantidad = _dbcontext.Personas.Count(x => x.IdProvincia == p.IdProvincia && x.Borrado == "No"),
                meta = p.Meta
            }).ToListAsync();

            return Ok(datos);

        }

         [HttpGet("miembros-por-prov")]
         public async Task<IActionResult> GetMiembrosTotalPorProvincia()
         {
             var resultado = await _dbcontext.Personas.Where(p => p.Borrado == "No")
                 .Include(p => p.Provincia)
                 .GroupBy(p => p.Provincia.Nombre)
                 .Select(g => new
                 {
                     Provincia = g.Key,
                     Cantidad = g.Count()

                 })
                 .ToListAsync();

             return Ok(resultado);
         }
        

        // [Authorize(Roles = "Administrador")]

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult<ProvinciasDto>>EditProv(int id, [FromBody] ProvinciasDto dto)
        {
            if(id != dto.IdProvincia)
            {
                return BadRequest("El ID de la URL y el del cuerpo no coinciden.");

            }
            var provexist = await _dbcontext.Provincias.FindAsync(id);
            if(provexist == null)
            {
                return NotFound();
            }

            provexist.Nombre = dto.Nombre;
            provexist.Meta = dto.Meta;
            await _dbcontext.SaveChangesAsync();
            return Ok(provexist);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Provincias>> GetoneProv(int id)
        {
            var provincia = await _dbcontext.Provincias.FindAsync(id);
            if (provincia == null)
            {
                return NotFound();
            }
            return provincia;
        }
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Provincias>> PostProvincia([FromBody] ProvinciasDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var prov = new Provincias
            {
                Nombre = dto.Nombre,
                Meta = dto.Meta

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

        [Authorize(Roles = "Administrador")]
        [HttpPut]
        public async Task<ActionResult> BorrarProv(int id, [FromBody] Provincias prov)
        {
            var buscarprov = await _dbcontext.Provincias.FindAsync(id);
            if (buscarprov == null)
            {
                return BadRequest(ModelState);
            }
            buscarprov.Borrado = "Si";
            await _dbcontext.SaveChangesAsync();
            return Ok(new { message = "Prov Borrada!" });
        }
        //[Authorize(Roles = "Administrador")]
        [HttpPost("asignar-provincias")]
        public IActionResult AsignarProvincias([FromBody] AsignacionDTO dto)
        {
            try
            {
                // 1. Eliminar asignaciones actuales
                var actuales = _dbcontext.Usuario_Provincia
                    .Where(x => x.IdUsuario == dto.IdUsuario);

                _dbcontext.Usuario_Provincia.RemoveRange(actuales);

                // 2. Insertar nuevas asignaciones
                foreach (var idProv in dto.Provincias)
                {
                    _dbcontext.Usuario_Provincia.Add(new Usuario_Provincia
                    {
                        IdUsuario = dto.IdUsuario,
                        IdProvincia = idProv
                    });
                }

                _dbcontext.SaveChanges();

                return Ok(new { mensaje = "Asignación realizada con éxito" });

            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar provincias: {ex.Message}");
            }
        }
   
    }
}

