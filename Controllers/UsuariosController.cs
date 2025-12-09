using Candidate.Dtos;
using Candidate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Candidate.Controllers
{
    // [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.Where(u => u.Borrado == "No").ToListAsync();
        }
        [HttpGet("provincias/{idProvincia}/usuarios")]
        public async Task<IActionResult> GetUsuariosPorProvincia(int idProvincia)
        {
            var usuarios = await _context.Usuarios
                .FromSqlRaw("EXEC sp_GetUsuariosPorProvincia @IdProvincia={0}", idProvincia)
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUSer(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        [HttpPost("asignaciones")]
        public async Task<IActionResult> AsignarProvincia(Usuario_Provincia dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AsignarProvincia @IdUsuario={0}, @IdProvincia={1}",
                dto.IdUsuario, dto.IdProvincia);

            return Ok("Asignación creada");
        }

        [HttpPost]
        public async Task<ActionResult<Usuarios>> Postusuarios([FromBody] Usersdto usersdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool contrasenaExiste = await _context.Usuarios
         .AnyAsync(x => x.Contraseña == usersdto.Contraseña);

            if (contrasenaExiste)
                return BadRequest("La contraseña ya existe.");

            var usuario = new Usuarios
            {
                Usuario = usersdto.Usuario,
                Contraseña = usersdto.Contraseña,
                Rol = usersdto.Rol

            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.IdUsuario }, usuario);
        }
        [HttpPost("asignar-usuarios")]
        public IActionResult AsignarUsuarios([FromBody] AsignarUsuarios dto)
        {
            try
            {
                // Eliminar asignaciones existentes
                var actuales = _context.Usuario_Provincia
                    .Where(x => x.IdProvincia == dto.IdProvincia);

                _context.Usuario_Provincia.RemoveRange(actuales);

                // Crear nuevas asignaciones
                foreach (var idUsuario in dto.Usuarios)
                {
                    _context.Usuario_Provincia.Add(new Usuario_Provincia
                    {
                        IdUsuario = idUsuario,
                        IdProvincia = dto.IdProvincia
                    });
                }

                _context.SaveChanges();

                return Ok("Asignaciones actualizadas correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar usuarios: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] Usersdto dto)
        {
            var userexist = await _context.Usuarios.FindAsync(id);
            if (userexist == null)
            {
                return NotFound();
            }

            userexist.Usuario = dto.Usuario;
            userexist.Contraseña = dto.Contraseña;
            userexist.Rol = dto.Rol;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Datos actualizados" });
        }

        [HttpPut("Borrar/{id}")]
        public async Task<ActionResult> Borrar(int id, [FromBody] Usuarios u)
        {
            var buscar = await _context.Usuarios.FindAsync(id);
            if (buscar == null)
            {
                return BadRequest(ModelState);
            }
            buscar.Borrado = "Si";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Usuario Borrado" });

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var userdeleted = await _context.Usuarios.Where(u => u.IdUsuario == id).ExecuteDeleteAsync();
            if (userdeleted > 0)
            {
                await _context.Usuarios.ToListAsync();
            }
            return NoContent();
        }
    }
}
