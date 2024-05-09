using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ActionsController : ControllerBase
{
[HttpPost("like/{userId:long}")]
    public async Task<IActionResult> LikeUser(long userId)
    {
        //TODO implement liking user

        return Ok();
    }
    
    [HttpPost("block/{userId:long}")]
    public async Task<IActionResult> BlockUser(long userId)
    {
        //TODO implement blocking user

        return Ok();
    }
    
    [HttpPost("viewed/{userId:long}")]
    public async Task<IActionResult> ViewedUser(long userId)
    {
        //TODO implement viewing user

        return Ok();
    }
}