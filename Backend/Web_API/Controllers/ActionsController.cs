using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
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
    public async Task<IActionResult> LikeUser([FromBody]LikeRequestDto like)
    {
        validator.CheckPositiveNumber(like.likedId);
        validator.CheckPositiveNumber(like.likerId);
        validator.CheckUserAuth(like.likerId, User.Claims);
        
        var output = await actionService.LikeUser(like.likerId, like.likedId);
        return Ok(output);
    }
    
    /*[HttpGet("chat")]
    public async Task<IActionResult> GetChat([FromQuery]int userId)
    {
        //create chat with signalR hub
        
    }*/
}