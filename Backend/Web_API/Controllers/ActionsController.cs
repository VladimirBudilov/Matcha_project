﻿using BLL.Sevices;
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
    ChatService chatService
    ) : ControllerBase
{
    [HttpPost("like")]
    public async Task<IActionResult> LikeUser([FromBody]UserActionRequestDto userAction)
    {
        validator.CheckPositiveNumber(userAction.producerId);
        validator.CheckPositiveNumber(userAction.consumerId);
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
    
    [HttpGet("chat")]
    public async Task<ActionResult<ResponseDto<List<MessageResponseDto>>>> GetChat([FromBody]UserActionRequestDto userAction)
    {
        var messages = await chatService.GetMessages(userAction.producerId, userAction.consumerId);
        var response = new ResponseDto<List<MessageResponseDto>>()
        {
            
        }
        return Ok(messages);
    }

    [HttpGet("clearNotification/{id:int}")]
    public async Task<IActionResult> ClearNotifications([FromRoute] int id)
    {
        validator.CheckUserAuth(id, User.Claims);
        notificationService.ClearNotifications(id);
        return Ok();
    }
}