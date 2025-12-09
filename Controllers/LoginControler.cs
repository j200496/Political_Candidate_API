using Candidate.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace Candidate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginControler
    {
        private readonly AppDbContext _context;
        public LoginControler(AppDbContext context)
        {
            _context = context;
        }
      
        /* [HttpPost]
         public async Task<ActionResult<Usuarios>> Login( Login login)
         {
             var User = await _context.Usuarios
          .FirstOrDefaultAsync(u => u.Usuario == login.UserName && u.Contraseña == login.Password);
             if (User == null)
             {
                 return UnauthorizedAccessException (new { message = "Usuario o contraseña incorrectos" });
             }

             return Ok(User);


         }*/

    }
}
