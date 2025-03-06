using Microsoft.AspNetCore.Mvc;
using task_management.Services;
using task_management.Dtos;

namespace task_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto request)
        {            
            // Aqui você deve implementar sua lógica real de autenticação
            // Este é apenas um exemplo simples
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _tokenService.GenerateToken(request.Username, new List<string> { "Admin" });
                var expiration = DateTime.Now.AddMinutes(120); // Ajuste conforme sua configuração

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Expiration = expiration,
                    Username = request.Username
                });
            }

            return Unauthorized();
        }
    }
}