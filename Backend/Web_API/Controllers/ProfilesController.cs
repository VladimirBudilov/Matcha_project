﻿using AutoMapper;
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
public class ProfilesController(
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
        [FromBody] Parameters parameters)
    {
        var id = claimsService.GetId(User.Claims);
        validator.CheckSearchParameters(parameters.Search);
        validator.CheckSortParameters(parameters.Sort);
        validator.CheckPaginationParameters(parameters.Pagination);

        var (amountOfPages, output) =
            await profileService.GetFullProfilesAsync(parameters.Search, parameters.Sort, parameters.Pagination, id);
        output = await profileService.CheckUsersLikes(output, id);
        var profiles = mapper.Map<List<ProfileResponse>>(output);
        return new ProfilesData()
        {
            Profiles = profiles,
            AmountOfPages = amountOfPages,
            PageSize = parameters.Pagination.PageSize,
            PageNumber = parameters.Pagination.PageNumber
        };
    }

    // GET api/<UsersController>/5
    [HttpGet("{id:int}")]
    public async Task<FullProfileResponseDto> GetProfileFullDataById([FromRoute] int id)
    {
        validator.CheckId(id);

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
        validator.CheckId(id);
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
    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        var id = claimsService.GetId(User.Claims);
        var output = await profileService.GetFiltersAsync(id);

        return Ok(output);
    }
}