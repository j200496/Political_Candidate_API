using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Candidate
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly AppDbContext _dbcontext; 
        public PersonasController(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        // Metodo para devolver la lista de personas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personas>>> GetPersonas()

          => await _dbcontext.Personas.ToListAsync();

        //Metodo para devolver una persona por su id
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
        [HttpGet("total")]
        public async Task<ActionResult<int>>TotalMiembros()
        {
            try
            {
                var total = await _dbcontext.Personas.CountAsync();
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }
        //Metodo para filtrar personas por su nombre
        [HttpGet("filter")]
        public async Task<ActionResult> Filterperson([FromQuery] string name)
        {
            var filter = await _dbcontext.Personas.Where(p => p.Nombre.Contains(name)).ToListAsync();
            if(filter == null || filter.Count == 0)
            {
                return Ok(_dbcontext.Personas.ToListAsync());
            }
            if (string.IsNullOrEmpty(name))
            {
                return Ok(_dbcontext.Personas.ToListAsync());
            }
            return Ok(filter);
        }
        //Metodo para capturar los datos de las personas
        [HttpPost]
         public async Task<ActionResult<Personas>> PostPersonas([FromBody] CreatePersonas dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var persona = new Personas
            {
              IdPersona = dto.IdPersona,
              Nombre = dto.Nombre,
              Telefono = dto.Telefono,
              Direccion = dto.Direccion,
              Cedula = dto.Cedula,
              Provincia = dto.Provincia,
            };
            if(persona.IdPersona > 0)
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
        //Metodo para actualizar los datos de la persona
        [HttpPut("{id}")]
        public async Task<ActionResult>PutPersona(int id, [FromBody] Personas person)
        {
            if(id != person.IdPersona) return BadRequest(ModelState);
            var personaexiste = await _dbcontext.Personas.FindAsync(id);
            if(personaexiste == null)
            {
                return NotFound();
            }
            personaexiste.Nombre = person.Nombre;
            personaexiste.Telefono = person.Telefono;
            personaexiste.Direccion = person.Direccion;
            personaexiste.Cedula = person.Cedula;
            personaexiste.Provincia = person.Provincia;
            await _dbcontext.SaveChangesAsync();
            return Ok();
        }
        //Metodo para borrar los datos por su id
        [HttpDelete("{id}")]
        public async Task<ActionResult>DeletePersona(int id)
        {
            var personasborradas = await _dbcontext.Personas.Where(p => p.IdPersona == id).ExecuteDeleteAsync();

            if(personasborradas > 0)
            { 
                return Ok(await _dbcontext.Personas.ToListAsync());

            }
            return NoContent();
        }
    }
}
