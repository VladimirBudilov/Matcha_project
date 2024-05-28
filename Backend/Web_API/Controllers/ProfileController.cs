using AutoMapper;
using BLL.Sevices;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
using Web_API.Helpers;
using Web_API.Hubs.Helpers;
using Web_API.Hubs.Services;
using Profile = DAL.Entities.Profile;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ProfileController(
    ProfileService profileService,
    ActionService actionService,
    IMapper mapper,
    DtoValidator validator,
    ClaimsService claimsService,
    NotificationService notificationService,
    UserService userService
) : ControllerBase
{
    // GET: api/profile
    [HttpGet]
    public async Task<ProfilesData> GetAllProfilesInfo(
        [FromQuery] SearchParameters search,
        [FromQuery] SortParameters sort,
        [FromQuery] PaginationParameters pagination)
    {
        var id = claimsService.GetId(User.Claims);
        search.UserId = id;
        validator.CheckSearchParameters(search);
        validator.CheckSortParameters(sort);
        validator.CheckPaginationParameters(pagination);

        var (amountOfPages, output) = await profileService.GetFullProfilesAsync(search, sort, pagination);
        output = await profileService.CheckUsersLikes(output, id);
        var profiles = mapper.Map<List<ProfileResponse>>(output);
        return new ProfilesData()
        {
            Profiles = profiles,
            AmountOfPages = amountOfPages,
            PageSize = pagination.PageSize,
            PageNumber = pagination.PageNumber
        };
    }

    // GET api/<UsersController>/5
    [HttpGet("{id:int}")]
    public async Task<FullProfileResponseDto> GetProfileFullDataById([FromRoute] int id)
    {
        validator.CheckPositiveNumber(id);

        var viewerId = claimsService.GetId(User.Claims);
        if (await actionService.TryViewUser(viewerId, id))
        {
            notificationService.AddNotification(id, new Notification()
            {
                Actor = (await userService.GetUserByIdAsync(viewerId))!.UserName,
                Message = "You have a new view",
                Type = NotificationType.View,
            });
        }

        var model = await profileService.GetFullProfileByIdAsync(id);
        model = await profileService.CheckUserLike(model, viewerId);
        var output = mapper.Map<FullProfileResponseDto>(model);
        return output;
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id:int}")]
    public async Task UpdateProfile([FromRoute] int id, [FromBody] ProfileDto profileCreation)
    {
        validator.CheckUserAuth(id, User.Claims);
        validator.CheckPositiveNumber(id);
        validator.ProfileRequestDto(profileCreation);
        var model = mapper.Map<Profile>(profileCreation);
        await profileService.UpdateProfileAsync(id, model);
    }

    [HttpGet("interests")]
    public async Task<IActionResult> GetInterests()
    {
        var output = await profileService.GetInterestsAsync();

        return Ok(output);
    }
}