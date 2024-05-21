using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.Helpers;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ActionsController(
    ActionService actionService,
    DtoValidator validator
    ) : ControllerBase
{
    [HttpPost("like")]
    public async Task<IActionResult> LikeUser([FromQuery]int likerId, [FromQuery]int likedId)
    {
        validator.CheckPositiveNumber(likedId);
        validator.CheckPositiveNumber(likerId);
        validator.CheckUserAuth(likerId, User.Claims);
        
        var output = await actionService.LikeUser(likerId, likedId);
        return Ok(output);
    }
    
    /*[HttpGet("chat")]
    public async Task<IActionResult> GetChat([FromQuery]int userId)
    {
        //create chat with signalR hub
        
    }*/
}