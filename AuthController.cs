using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.DTO;
using WebApiTemplate.Services;

namespace WebApiTemplate.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IValidator<UserRegisterDto> _registerValidator;
        private readonly IValidator<UserLoginDto> _loginValidator;

        public AuthController(AuthService authService,
            IValidator<UserRegisterDto> registerValidator,
            IValidator<UserLoginDto> loginValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto userDto)
        {
            var validationResult = _registerValidator.Validate(userDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var response = _authService.Register(userDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto userDto)
        {
            var validationResult = _loginValidator.Validate(userDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var token = _authService.Authenticate(userDto);
            if (token == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("change-role/{userId}")]
        [Authorize(Roles = "Admin")] // 🔹 Only Admin can change roles
        public IActionResult ChangeUserRole(int userId, [FromBody] string newRole)
        {
            bool updated = _authService.ChangeUserRole(userId, newRole);
            if (!updated) return NotFound(new { message = "User not found" });

            return Ok(new { message = $"User role updated to {newRole}" });
        }
    }
}
