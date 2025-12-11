using Candidate.Dtos;
using Candidate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Xml;

namespace Candidate.Controllers
{
    // [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        public PersonasController(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        // Metodo para devolver la lista de personas donde borrado = No
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personas>>> GetPersonas()
        {
            var resultado = await _dbcontext.Personas.Where(p => p.Borrado == "No").Select(p => new
            {
                p.IdPersona,
                p.Nombre,
                p.Telefono,
                p.Direccion,
                p.Cedula,
                Provincia = p.Provincia.Nombre,
                p.Usuario.Usuario
            }).ToListAsync();
            return Ok(resultado);
        }

    
        [HttpGet("Selectppu")]
        public async Task<ActionResult<IEnumerable<Personas>>> GetPersonasPorUs(int id)
        {
            var personas = await _dbcontext.Personas
         .Include(p => p.Provincia)
         .Where(p => p.Borrado == "No" && p.IdUsuario == id)
         .Select(p => new
         {
             p.IdPersona,
             p.Nombre,
             p.Telefono,
             p.Direccion,
             p.Cedula,
             p.Borrado,
             p.IdUsuario,
             Provincia = p.Provincia.Nombre
         })
         .ToListAsync();

            if (personas == null)
            {
                return NoContent();
            }
            return Ok(personas);
        }

        [HttpGet("Prov")]
        public async Task<ActionResult<IEnumerable<Provincias>>> ListaProvincias()

            => await _dbcontext.Provincias.Where(p => p.Borrado == "No").ToListAsync();


      
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Personas>> PostPersonas([FromBody] PersonasDTO dto)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid == null)
                return Unauthorized("Token inválido o faltante.");

            int IdUsuario = int.Parse(userid);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var persona = new Personas
            {
                IdPersona = dto.IdPersona,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Cedula = dto.Cedula,
                IdProvincia = dto.IdProvincia,
                IdUsuario = IdUsuario,
            };
            if (persona.IdPersona > 0)
            {
                _dbcontext.Entry(persona).State = EntityState.Modified;
            }
            else
            {
                _dbcontext.Personas.Add(persona);
            }

            await _dbcontext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPersonas), new { id = persona.IdPersona }, persona);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Personas>> GetPersona(int id)
        {
            var person = await _dbcontext.Personas.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }
            return person;
        }
        //Metodo para recolectar la el total de personas 
        [Authorize(Roles = "Administrador")]
        [HttpGet("total")]
        public async Task<ActionResult<int>> TotalMiembros()
        {
            try
            {
                var total = await _dbcontext.Personas.Where(p => p.Borrado == "No").CountAsync();
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        [Authorize(Roles = "Equipo")]
        [HttpGet("Totalppu")]
        public async Task<ActionResult<int>> ToTalMiembrosPorUsuario(int id)
        {
            try
            {
                var total = await _dbcontext.Personas.Where(p => p.Borrado == "No" && p.IdUsuario == id).CountAsync();

                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }

        }

        //Metodo para filtrar personas por su nombre
        [Authorize]
        [HttpGet("filter")]
        public async Task<ActionResult> Filterperson([FromQuery] string name)
        {
            var filter = await _dbcontext.Personas.Where(p => p.Nombre.Contains(name)).ToListAsync();
            if (filter == null || filter.Count == 0)
            {
                return Ok(_dbcontext.Personas.ToListAsync());
            }
            if (string.IsNullOrEmpty(name))
            {
                return Ok(_dbcontext.Personas.ToListAsync());
            }
            return Ok(filter);
        }
        [Authorize]
        [HttpPost("miembros")]
        public async Task<IActionResult> InsertMiembro(MiembroDTO dto)
        {
            try
            {
                await _dbcontext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_InsertMiembro @Nombre={0}, @Cedula={1}, @Telefono={2}, @IdUsuario={3}, @IdProvincia={4}",
                    dto.Nombre, dto.Cedula, dto.Telefono, dto.IdUsuario, dto.IdProvincia
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message); // Captura RAISERROR del SP
            }

            return Ok("Miembro registrado");
        }
     
        //Metodo para actualizar los datos de la persona

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutPersona(int id, [FromBody] PersonasDTO person)
        {
            if (id != person.IdPersona) return BadRequest(ModelState);
            var personaexiste = await _dbcontext.Personas.FindAsync(id);
            if (personaexiste == null)
            {
                return NotFound();
            }
            personaexiste.Nombre = person.Nombre;
            personaexiste.Telefono = person.Telefono;
            personaexiste.Direccion = person.Direccion;
            personaexiste.Cedula = person.Cedula;
            personaexiste.IdProvincia = person.IdProvincia;
            await _dbcontext.SaveChangesAsync();
            return Ok();
        }
        [Authorize]
        [HttpPut("borrado/{id}")]
        public async Task<ActionResult> SetBorrado(int id)
        {
            var setborrado = await _dbcontext.Personas.FindAsync(id);

            if (setborrado == null)
            {
                return BadRequest(ModelState);
            }

            setborrado.Borrado = "Si";

            await _dbcontext.SaveChangesAsync();
            return Ok(new { message = "Exitosa" });

        }


        //Metodo para borrar los datos por su id
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePersona(int id)
        {
            var personasborradas = await _dbcontext.Personas.Where(p => p.IdPersona == id).ExecuteDeleteAsync();

            if (personasborradas > 0)
            {
                return Ok(await _dbcontext.Personas.ToListAsync());

            }
            return NoContent();
        }
    }
}
