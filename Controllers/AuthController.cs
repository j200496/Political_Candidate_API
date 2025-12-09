using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Candidate.Models;
using Microsoft.EntityFrameworkCore;

namespace Candidate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(x => x.Usuario == dto.Usuario && x.Contraseña == dto.Contraseña);

            if (usuario == null)
                return Unauthorized("Usuario o clave incorrectos");

            if(usuario.Borrado == "Si")
            {
                return Unauthorized("El usuario no existe");
            }

            var token = GenerarToken(usuario);

            return Ok(new
            {
                token,
                usuario = usuario.Usuario,
                rol = usuario.Rol,
                idusuario = usuario.IdUsuario
            });
        }

        private string GenerarToken(Usuarios usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Usuario),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)  
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
