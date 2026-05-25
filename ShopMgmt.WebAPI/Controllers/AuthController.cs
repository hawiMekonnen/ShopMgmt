using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.DTOS;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { Message = "Invalid credentials." });

        // Verify password hash
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { Message = "Invalid credentials." });

        // Build JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        if (user.ShopId.HasValue)
        {
            claims.Add(new Claim("shopId", user.ShopId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"])) ,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var response = new LoginResponseDto
        {
            Token = tokenString,
            Email = user.Email,
            Role = user.Role.ToString(),
            UserId = user.UserId,
            ShopId = user.ShopId
        };

        return Ok(response);
    }
}
