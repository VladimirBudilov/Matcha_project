using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BLL.Helpers;
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
    private readonly EmailHelper _emailHelper;

    public AuthController(UserService userService, IOptions<JwtConfig> jwtConfig, IMapper mapper, ILogger<AuthController> logger, EmailHelper emailHelper)
    {
        _userService = userService;
        _jwtConfig = jwtConfig.Value;
        _mapper = mapper;
        _logger = logger;
        _emailHelper = emailHelper;
    }
    
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery]string email, [FromQuery]string token)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        if (user.ResetToken != token)
        {
            return BadRequest("Invalid token");
        }

        var alreadyVerified = await _userService.ConfirmEmailAsync(user.UserId);
        if (alreadyVerified)
        {
            return BadRequest("Email already verified");
        }
        return Ok();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserAuthRequestDto loginDto)
    {
        var isValid = await _userService.AuthenticateUser(loginDto.UserName, loginDto.Password);
        if (!isValid)
        {
            _logger.LogWarning("Invalid login attempt");
            return BadRequest(new AuthResponseDto()
            {
                ErrorMessage = "Invalid Authentication",
                Result = false
            });
        }

        var token = GenerateJwtToken(await _userService.GetUserByUserNameAsync(loginDto.UserName));
        return Ok(new AuthResponseDto()
        {
            Result = true,
            Token = token
        });
    }
    
    [HttpPost("registration")]
    public async Task<IActionResult> Register([FromBody] UserDto value)
    {
        try
        {
            var userModel = _mapper.Map<UserModel>(value);
            var token = await _userService.RegisterUserAsync(userModel);
            
            var emailUrl = Request.Scheme + "://" + Request.Host + "/api/auth/verify-email?email=" + userModel.Email + "&token=" + token;
            var emailBody = "Please click on the link to verify your email: <a href=\"" + System.Text.Encodings.Web.HtmlEncoder.Default.Encode(emailUrl) + "\">link</a>";
            
            _emailHelper.SendEmail(userModel.Email, emailBody);
            
        }
        catch   (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
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
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
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