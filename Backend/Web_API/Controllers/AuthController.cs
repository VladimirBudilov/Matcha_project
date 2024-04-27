using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BLL.Sevices;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web_API.Configurations;
using Web_API.DTOs;
using Web_API.Helpers;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserService userService, AuthService authService,
    IOptions<JwtConfig> jwtConfig, IMapper mapper, ILogger<AuthController> logger,
    EmailService emailService, DtoValidator validator)
    : ControllerBase
{
    private readonly AuthService _authService = authService;
    private readonly UserService _userService = userService;
    private readonly JwtConfig _jwtConfig = jwtConfig.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly EmailService _emailService = emailService;
    private readonly DtoValidator _validator = validator;

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery]string email, [FromQuery]string token)
    {
        await CheckEmailAndToken(email, token);
        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserAuthRequestDto loginDto)
    {
        var isValid = await _authService.AuthenticateUser(loginDto.UserName, loginDto.Password);
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
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        return Ok();
        //TODO implement logout
        //remove jwt token from user
    }
    
    [HttpPost("registration")]
    public async Task<IActionResult> Register([FromBody] UserDto value)
    {
        _validator.UserDto(value);
        try
        {
            var userModel = _mapper.Map<User>(value);
            var token = await _authService.RegisterUserAsync(userModel);
            if(token == null) return BadRequest("User already exists");
            var emailUrl = Request.Scheme + "://" + Request.Host + "/api/auth/verify-email?email=" + userModel.Email + "&token=" + token;
            var emailBody = "Please click on the link to verify your email: <a href=\"" + System.Text.Encodings.Web.HtmlEncoder.Default.Encode(emailUrl) + "\">link</a>";
            
            //TODO uncomment when will be ready smtp server
            //_emailService.SendEmail(userModel.Email, emailBody);
        }
        catch   (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    private string GenerateJwtToken(User user)
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
    
    private async Task CheckEmailAndToken(string email, string token)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new DataValidationException("Invalid email");
        }

        if (user.ResetToken != token)
        {
            throw new DataValidationException("Invalid token");
        }

        var alreadyVerified = await _authService.ConfirmEmailAsync(user.UserId);
        if (alreadyVerified)
        {
            throw new DataValidationException("Email already verified");
        }
    }

}