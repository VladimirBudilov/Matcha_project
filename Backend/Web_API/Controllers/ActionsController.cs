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
    [HttpPost("like/{likerId:int}")]
    public async Task<IActionResult> LikeUser([FromQuery]int likerId, [FromQuery]int likedId)
    {
        //TODO add validation
        validator.CheckId(likedId);
        validator.CheckId(likerId);
        validator.CheckUserAuth(likerId, User.Claims);
        
        var output = await actionService.LikeUser(likerId, likedId);
        return Ok(output);
    }
}