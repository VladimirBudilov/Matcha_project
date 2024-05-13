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
    private readonly JwtConfig _jwtConfig = jwtConfig.Value;

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        validator.EmailAndToken(email, token);
        await emailService.CheckEmailAndToken(userService, authService, email, token);
        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthRequestDto loginDto)
    {
        validator.UserAuthRequestDto(loginDto);
        var isValid = await authService.AuthenticateUser(loginDto.UserName, loginDto.Password);
        if (!isValid)
        {
            logger.LogWarning("Invalid login attempt");
            return BadRequest(new AuthResponseDto()
            {
                Error = "Invalid Authentication",
                Result = false
            });
        }

        var token = GenerateJwtToken(await userService.GetUserByUserNameAsync(loginDto.UserName));
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
        //remove jwt refresh token
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register([FromBody] UserRegestrationDto value)
    {
        validator.UserRegistrationDto(value);
        try
        {
            var userModel = mapper.Map<User>(value);
            var token = await authService.RegisterUserAsync(userModel);
            if (token == null) return BadRequest("User already exists");
            var emailUrl = Request.Scheme + "://" + Request.Host + "/api/auth/verify-email?email=" + userModel.Email + "&token=" + token;
            var emailBody = "Please click on the link to verify your email: <a href=\"" + System.Text.Encodings.Web.HtmlEncoder.Default.Encode(emailUrl) + "\">link</a>";

            //TODO uncomment when will be ready smtp server
            //_emailService.SendEmail(userModel.Email, emailBody);
        }
        catch (Exception ex)
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
    
    [HttpGet("get-id")]
    public async Task<IActionResult> GetId()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        return Ok(id);
    }
}
