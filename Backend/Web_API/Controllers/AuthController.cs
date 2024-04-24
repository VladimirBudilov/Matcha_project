using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BLL.Models;
using BLL.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web_API.Configurations;
using Web_API.DTOs;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtConfig _jwtConfig;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserService userService, IOptions<JwtConfig> jwtConfig, IMapper mapper, ILogger<AuthController> logger)
    {
        _userService = userService;
        _jwtConfig = jwtConfig.Value;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserAuthRequestDTO loginDto)
    {
        var isValid = await _userService.AuthenticateUser(loginDto.UserName, loginDto.Password);
        if (!isValid)
        {
            _logger.LogWarning("Invalid login attempt");
            return Unauthorized();
        }

        var token = GenerateJwtToken(await _userService.GetUserByUserNameAsync(loginDto.UserName));
        return Ok(token);
    }

    private string GenerateJwtToken(UserModel user)
    {
        var jwtTokenHendler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(6),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = jwtTokenHendler.CreateToken(tokenDescriptor);
        
        return jwtTokenHendler.WriteToken(token);
    }
}