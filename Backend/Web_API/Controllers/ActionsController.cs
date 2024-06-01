using AutoMapper;
using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    ClaimsService claimsService,
    ChatService chatService,
    IMapper mapper,
    ProfileService profileService
    ) : ControllerBase
{
    [HttpPost("like")]
    public async Task<IActionResult> LikeUser([FromBody]UserActionRequestDto userAction)
    {
        validator.CheckId(userAction.producerId);
        validator.CheckId(userAction.consumerId);
        validator.CheckUserAuth(userAction.producerId, User.Claims);
        
        var (notificationType, output) = await actionService.LikeUser(userAction.producerId, userAction.consumerId);
        notificationService.AddNotification(userAction.consumerId, new Notification()
        {
            Type = notificationType,
            Message = "You have a new userAction",
            Actor = (await userService.GetUserByIdAsync(userAction.producerId))!.UserName
        });
        return Ok(output);
    }
    
    [HttpPost("chat")]
    public async Task<ActionResult<ResponseDto<List<MessageResponseDto>>>> GetChat([FromBody]UserActionRequestDto userAction)
    {
        //check that user is authorized
        validator.CheckId(userAction.producerId);
        validator.CheckId(userAction.consumerId);
        validator.CheckUserAuth(userAction.producerId, User.Claims);
        //check that users are matched
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
}