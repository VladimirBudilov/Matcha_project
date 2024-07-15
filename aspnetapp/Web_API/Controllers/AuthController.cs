using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BLL.Helpers;
using BLL.Sevices;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web_API.Configurations;
using Web_API.DTOs;
using Web_API.DTOs.Request;
using Web_API.DTOs.Response;
using Web_API.Helpers;
using Web_API.Hubs.Helpers;
using Web_API.Hubs.Services;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserService userService,
    AuthService authService,
    IOptions<JwtConfig> jwtConfig,
    IMapper mapper,
    ILogger<AuthController> logger,
    EmailService emailService,
    DtoValidator validator,
    NotificationService notificationService
)
    : ControllerBase
{
    private readonly JwtConfig _jwtConfig = jwtConfig.Value;

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        validator.EmailAndToken(email, token);
        await emailService.CheckEmailAndToken(userService, authService, email, token);
        var user = await userService.GetUserByEmailAsync(email);
        user.IsVerified = true;
        await userService.UpdateUserAsync(user.Id, user);
        return Ok("Email verified successfully");
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
                Result = false,
                Error = "Invalid Authentication",
            });
        }

        var token = GenerateJwtToken(await userService.GetUserByUserNameAsync(loginDto.UserName));

        var user = await userService.GetUserByUserNameAsync(loginDto.UserName);
        notificationService.AddNotification(user.Id,
            new Notification() { Actor = user.UserName, Message = "You have logged in" });

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
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto value)
    {
        validator.UserRegistrationDto(value);

        var userModel = mapper.Map<User>(value);
        var token = await authService.RegisterUserAsync(userModel);
        if (token == null) return BadRequest("Actor already exists");
        var emailUrl = Request.Scheme + "://" + Request.Host + "/api/auth/verify-email?email=" + userModel.Email +
                       "&token=" + token;
        var emailBody = "Please click on the link to verify your email: <a href=\"" +
                        System.Text.Encodings.Web.HtmlEncoder.Default.Encode(emailUrl) + "\">link</a>";

        emailService.SendEmail(userModel.Email, emailBody);

        return Ok();
    }

    [HttpPost("restore-password")]
    public async Task<IActionResult> RestorePassword([FromBody] EmailDto email)
    {
        var user = await userService.GetUserByEmailAsync(email.email);
        if (user == null) return BadRequest("User not found");
        var newPassword = PasswordManager.GeneratePassword();
        await userService.UpdatePasswordAsync(user.Id, user.Password, newPassword);
        var emailBody = "Your new password is: " + newPassword;
        emailService.SendEmail(user.Email, emailBody);
        return Ok();
    }

    private string GenerateJwtToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Hash, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(6),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);

        return jwtTokenHandler.WriteToken(token);
    }

    [HttpGet("get-id")]
    public async Task<IActionResult> GetId()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (id == null) return Unauthorized();
        return Ok(id);
    }
}

public record EmailDto(string email);