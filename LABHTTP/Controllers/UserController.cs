using LABHTTP.Data;
using LABHTTP.Model.DTO;
using LABHTTP.Repository;
using LABHTTP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("User not found");
            if (_passwordHasher.Verify(request.Password, user.Password))
            {
                var token = _service.GenerateJwtToken(user);
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Ok();
            }
            else
            {
                return BadRequest("False Login Info");
            }
        }
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
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Ok();

        }
    }
}
