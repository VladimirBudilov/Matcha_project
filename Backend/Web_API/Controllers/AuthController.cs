using AutoMapper;
using BLL.Sevices;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserService userService, IMapper mapper, ILogger<AuthController> logger) : ControllerBase
{
    public async Task<IActionResult> Login(UserAuthoristaionDTO loginDto)
    {
        var isValid = await userService.AuthenticateUser(loginDto.UserName, loginDto.Password);
        if (!isValid)
        {
            logger.LogWarning("Invalid login attempt");
            return Unauthorized();
        }

        return Ok();
    }
}