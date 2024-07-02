using AutoMapper;
using BLL.Sevices;
using DAL.Models;
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
    UserService userService,
    ChatService chatService,
    IMapper mapper,
    ProfileService profileService
) : ControllerBase
{
    [HttpPost("like")]
    public async Task<IActionResult> LikeUser([FromBody] UserActionRequestDto userAction)
    {
        validator.CheckId(userAction.producerId);
        validator.CheckId(userAction.consumerId);
        validator.CheckUserAuth(userAction.producerId, User.Claims);

        var (notificationType, output) = await actionService.LikeUser(userAction.producerId, userAction.consumerId);
        var actor = await userService.GetUserByIdAsync(userAction.producerId);
        
        notificationService.AddNotification(userAction.consumerId, new Notification()
        {
            Type = notificationType,
            Message = "You have been liked",
            Actor = actor!.UserName + " " + actor!.LastName
        });
        await notificationService.SendNotificationToUser(userAction.consumerId);
        return Ok(output);
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ResponseDto<List<MessageResponseDto>>>> GetChat(
        [FromBody] UserActionRequestDto userAction)
    {
        validator.CheckId(userAction.producerId);
        validator.CheckId(userAction.consumerId);
        validator.CheckUserAuth(userAction.producerId, User.Claims);
        if (!await profileService.IsMatch(userAction.producerId, userAction.consumerId)) return BadRequest();

        var messages = await chatService.GetMessages(userAction.producerId, userAction.consumerId);

        var output = mapper.Map<IEnumerable<MessageResponseDto>>(messages);
        var response = new ResponseDto<List<MessageResponseDto>>()
        {
            Data = output.ToList(),
            Success = true,
            Error = null,
        };
        return Ok(response);
    }
    
    [HttpPost("block")]
    public async Task<IActionResult> GetBlackList([FromBody] UserActionRequestDto userAction)
    {
        validator.CheckId(userAction.producerId);
        validator.CheckId(userAction.consumerId);
        validator.CheckUserAuth(userAction.producerId, User.Claims);
            
        var wasAdded = await actionService.TryUpdateBlackListAsync(userAction.producerId, userAction.consumerId);
        var actor = await userService.GetUserByIdAsync(userAction.producerId);
        notificationService.AddNotification(userAction.consumerId, new Notification()
        {
            Type = NotificationType.BlackListed,
            Message = wasAdded ? "You have been blacklisted" : "You have been unblocked",
            Actor = actor!.UserName + " " + actor!.LastName
        });
        await notificationService.SendNotificationToUser(actor.Id);
        return Ok();
    }
        
}