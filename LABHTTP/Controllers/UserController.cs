using LABHTTP.Data;
using LABHTTP.Model.DTO;
using LABHTTP.Repository;
using LABHTTP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LABHTTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly AppDbContext _db;
        private readonly PasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository,UserService service, AppDbContext db, PasswordHasher passwordHasher)
        {
            _service = service;
            _db = db;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(userId);
        }
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return NoContent();
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Invalid Credentials");
            if (_passwordHasher.Verify(request.Password, user.Password))
            {
                var token = _service.GenerateJwtToken(user);
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                return Ok();
            }
            else
            {
                return Unauthorized("Invalid Credentials");
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] CreateUserRequest request)
        {
            var hash = _passwordHasher.Generate(request.Password);
            var user = new User
            {
                Email = request.Email,
                Password = hash,
                Role = "User"
            };
            _userRepository.AddAsync(user);
            var token = _service.GenerateJwtToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            return Ok();

        }
    }
}
