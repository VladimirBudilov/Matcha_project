using AutoMapper;
using BLL.Sevices;
using DAL.Entities;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
using Web_API.DTOs.Request;
using Web_API.DTOs.Response;
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
    ProfileService profileService,
    ClaimsService claimsService
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
        return Ok(new Like()
        {
            LikerId = actor.Id,
            LikedId = userAction.consumerId,
            IsLiked = wasAdded
        });
    }
    
    [HttpGet("blacklist")]
    public async Task<IActionResult> GetBlackList()
    {
        var id = claimsService.GetId(User.Claims);
        
        var blockedUsersId = await actionService.GetBlockedUsersIdASync(id);
        var output = new List<FullProfileResponseDto>();
        foreach (var userId in blockedUsersId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null) continue;
            var model = await profileService.GetFullProfileByIdAsync(userId);
            model = await profileService.CheckUserLike(model, id);
            var profile = mapper.Map<FullProfileResponseDto>(model);
            if(notificationService.IsUserOnline(userId)) profile.isOnlineUser = true;
            output.Add(profile);
        }
        var response = new ResponseDto<List<FullProfileResponseDto>>()
        {
            Data = output,
            Success = true,
            Error = null,
        };
        return Ok(response);
    }
    
    [HttpGet("viewed")]
    public async Task<IActionResult> GetViewedProfiles()
    {
        var id = claimsService.GetId(User.Claims);
        var viewedProfiles = await profileService.GetViewedProfilesAsync(id);
        var output = new List<FullProfileResponseDto>();
        foreach (var profile in viewedProfiles)
        {
            var model = await profileService.CheckUserLike(profile, id);
            var profileDto = mapper.Map<FullProfileResponseDto>(model);
            if(notificationService.IsUserOnline(profile.Id)) profileDto.isOnlineUser = true;
            output.Add(profileDto);
        }
        
        var response = new ResponseDto<List<FullProfileResponseDto>>()
        {
            Data = output,
            Success = true,
            Error = null,
        };
        
        return Ok(response);
    }
    
    [HttpGet("likers")]
    public async Task<IActionResult> GetLikedProfiles()
    {
        var id = claimsService.GetId(User.Claims);
        var viewedProfiles = await profileService.GetLikersProfilesAsync(id);
        var output = new List<FullProfileResponseDto>();
        foreach (var profile in viewedProfiles)
        {
            var model = await profileService.CheckUserLike(profile, id);
            var profileDto = mapper.Map<FullProfileResponseDto>(model);
            if(notificationService.IsUserOnline(profile.Id)) profileDto.isOnlineUser = true;
            output.Add(profileDto);
        }
        
        var response = new ResponseDto<List<FullProfileResponseDto>>()
        {
            Data = output,
            Success = true,
            Error = null,
        };
        
        return Ok(response);
    }
        
}