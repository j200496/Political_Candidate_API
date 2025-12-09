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
        //Metodo que devuelve la lista de todas las provincias de RD
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provincias>>> GetProvincias()
            => await _dbcontext.Provincias.Where(p => p.Borrado == "No").ToListAsync();


        [HttpGet("Lista-prov/{idUsuario}")]
        public async Task<ActionResult> GetProvasig(int idUsuario)
        {
            var ids = await _dbcontext.Usuario_Provincia.Where(x => x.IdUsuario == idUsuario).Select(x => x.IdProvincia).ToListAsync();
            return Ok(ids);
        }

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

        [HttpPost]
        public async Task<ActionResult<Provincias>> PostProvincia([FromBody] ProvinceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var prov = new Provincias
            {
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


        /* [HttpGet("usuarios/{idUsuario}/provincias")]
         public async Task<IActionResult> GetProvinciasPorUsuario(int idUsuario)
         {
             var provincias = await _dbcontext.Provincias
                 .FromSqlRaw("EXEC sp_GetProvinciasPorUsuario @IdUsuario={0}", idUsuario)
                 .ToListAsync();

             return Ok(provincias);
         }
         [HttpPost("asignaciones")]
         public async Task<IActionResult> AsignarProvincia(AsignacionDTO dto)
         {
             await _dbcontext.Database.ExecuteSqlRawAsync(
                 "EXEC sp_AsignarProvincia @IdUsuario={0}, @IdProvincia={1}",
                 dto.IdUsuario, dto.IdProvincia);

             return Ok("Asignación creada");
         }

           [HttpGet("miembros-p-provincia")]
           public async Task<ActionResult<IEnumerable<MiembroProvincias>>> GetMiembrosPprov()
           {
               var result = await _dbcontext.Personas.Where(p => p.Borrado == "No").GroupBy(m => m.Provincia).Select(g => new MiembroProvincias
               {
                   Provincia = g.Key,
                   CantidadMiembros = g.Count()
               }).ToListAsync();

         [HttpGet("filtrar-por-nombre")]
         public async Task<ActionResult<IEnumerable<Provincias>>> FilterProv([FromQuery] string name)
         {
             var filter = await _dbcontext.Provincias.Where(p => p.Nombre.Contains(name)).ToListAsync();
             if (filter == null)
             {
                 return Ok(_dbcontext.Provincias.ToListAsync());
             }
             return Ok(filter);
         }
       /*  [HttpGet("listar")]
         public IActionResult ListarProvincias()
         {
             try
             {
                 var provincias = _dbcontext
                     .Set<ProvinciasDto>()
                     .FromSqlRaw("EXEC selprov")
                     .ToList();

                 return Ok(provincias);
             }
             catch (Exception ex)
             { return BadRequest($"Error al obtener provincias: {ex.Message}");

             }
         }
          [HttpGet("listar")]
        public IActionResult ListarProvincias()
        {
            try
            {
                var provincias = _dbcontext
                    .Set<ProvinciasDto>()
                    .FromSqlRaw("EXEC selprov")
                    .ToList();

                return Ok(provincias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener provincias: {ex.Message}");

            }
        }

       /*  
       [HttpGet("select")]
         public IActionResult SelectUnaProv(int IdProvincia)
         {
             try
             {
                 var param = new SqlParameter("@IdProvincia", IdProvincia);

                 var provincia = _dbcontext.Provincia
                     .FromSqlRaw("EXEC selunaprov @IdProvincia", param)
                     .AsEnumerable()
                     .FirstOrDefault();

                 return Ok(provincia);
             }
             catch (Exception ex)
             {
                 return BadRequest($"Error al obtener provincias: {ex.Message}");
             }


         }

         [HttpPost("{idUsuario}/provincias")]
         public async Task<IActionResult> AsignarProvincias(
             int idUsuario, AsignarProvinciasDto dto)
         {
             var asignacionesActuales = _dbcontext.Usuarios
                 .Where(x => x.IdUsuario == idUsuario);

             _dbcontext.Usuario_Provincia.RemoveRange((Usuario_Provincia)asignacionesActuales);

             var nuevasAsignaciones = dto.IdsProvincias.Select(p =>
                 new Usuario_Provincia
                 {
                     IdUsuario = idUsuario,
                     IdProvincia = p
                 });

             _dbcontext.Usuario_Provincia.AddRange(nuevasAsignaciones);
             await _dbcontext.SaveChangesAsync();

             return Ok("Asignaciones actualizadas");
         }


         //Metodo para añadir provincias


       [HttpPut]
       public async Task<ActionResult>BorrarProv(int id, [FromBody] Provincias prov)
         {
             var buscarprov = await _dbcontext.Provincias.FindAsync(id);
             if(buscarprov == null)
             {
                 return BadRequest(ModelState);
             }
             buscarprov.Borrado = "Si";
             await _dbcontext.SaveChangesAsync();
             return Ok(new { message = "Prov Borrada!" });
         } */
    }
}

