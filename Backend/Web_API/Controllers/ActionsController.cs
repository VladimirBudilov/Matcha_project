using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
using Web_API.DTOs.Request;
using Web_API.Helpers;
using Web_API.Hubs.Helpers;
using Web_API.Hubs.Services;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ActionsController(
    ActionService actionService,
    NotificationService notificationService,
    DtoValidator validator,
    UserService userService
    ) : ControllerBase
{
    [HttpPost("userAction")]
    public async Task<IActionResult> LikeUser([FromBody]UserActionRequestDto userAction)
    {
        validator.CheckPositiveNumber(userAction.ProducerId);
        validator.CheckPositiveNumber(userAction.ConsumerId);
        validator.CheckUserAuth(userAction.ConsumerId, User.Claims);
        
        var (notificationType, output) = await actionService.LikeUser(userAction.ConsumerId, userAction.ProducerId);
        notificationService.AddNotification(userAction.ProducerId, new Notification()
        {
            Type = notificationType,
            Message = "You have a new userAction",
            Actor = (await userService.GetUserByIdAsync(userAction.ConsumerId))!.UserName
        });
        return Ok(output);
    }
    
    [HttpGet("chat")]
    public async Task<IActionResult> GetChat([FromBody]UserActionRequestDto userAction)
    {
        //create chat with signalR hub
        return Ok();
    }
}