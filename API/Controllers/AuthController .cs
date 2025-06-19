using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public AuthController(IAccountService accountService, IConfiguration config)
        {
            _accountService = accountService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var (principal, account) = await _accountService.GetClaimsPrincipalForLoginAsync(model.Email, model.Password);
            if (principal == null)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(principal.Claims);

            return Ok(new
            {
                token,
                user = account != null ? new
                {
                    AccountId = account.AccountId.ToString(),
                    account.AccountName,
                    account.AccountEmail,
                    AccountRole = account.AccountRole.ToString()
                } : new
                {
                    AccountId = "admin",
                    AccountName = _config["AdminAccount:Name"],
                    AccountEmail = _config["AdminAccount:Email"],
                    AccountRole = "Admin"
                }
            });

        }

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var secretKey = _config["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("SecretKey is null. Check appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}

