using AutoMapper;
using BLL.Sevices;
using DAL.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
using Web_API.Helpers;
using Profile = DAL.Entities.Profile;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ProfileController(
    ProfileService profileService,
    ActionService actionService,
    IMapper mapper,
    DtoValidator validator
    ) : ControllerBase
{
    // GET: api/<UsersController>
    [HttpGet]
    public async Task<ProfilesData> GetAllProfilesInfo(
        [FromQuery]SearchParameters search,
        [FromQuery] SortParameters sort,
        [FromQuery] PaginationParameters pagination)
    {
        //TODO implement DTO validation
        /*validator.CheckSearchParameters(search);
        validator.CheckSortParameters(sort);
        validator.CheckPaginationParameters(pagination);*/
        
        int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value, out var id );
        search.UserId = id;
        var output = await profileService.GetFullProfilesAsync(search, sort, pagination);
        var profiles = mapper.Map<List<ProfileForOtherUsers>>(output);
        return new ProfilesData()
        {
            Profiles = profiles,
            ProfilesCount = profiles.Count,
            PageSize = pagination.PageSize,
            PageNumber = pagination.PageNumber
        };
    }
    
    // GET api/<UsersController>/5
    [HttpGet("{id:int}")]
    public async Task<ProfileFullDataForOtherUsersDto> GetProfileFullDataById([FromRoute]int id)
    {
        validator.CheckId(id);

        int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value, out var viewerId);
        actionService.ViewedUser(viewerId, id);
        
        var model =  await profileService.GetFullProfileByIdAsync(id);
        var output = mapper.Map<ProfileFullDataForOtherUsersDto>(model);
        return output;
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id:int}")]
    public async Task UpdateProfile([FromRoute]int id, [FromBody] ProfileDto profileCreation)
    {
        validator.CheckUserAuth(id, User.Claims);
        validator.CheckId(id);
        validator.ProfileRequestDto(profileCreation);
        var model = mapper.Map<Profile>(profileCreation);
        await profileService.UpdateProfileAsync(id, model);
    }
    
    [HttpPost("interest")]
    public async Task<IActionResult> AddInterest([FromBody] string interest)
    {
        var output = await profileService.AddInterest(interest);
        return Ok(output);
    }
    
    [HttpGet("interests")]
    public async Task<IActionResult> GetInterests()
    {
        var output = await profileService.GetInterestsAsync();

        return Ok(output);
    }
    
    [HttpDelete("interest")]
    public async Task<IActionResult> RemoveInterest([FromQuery]string interest)
    {
        await profileService.RemoveInterest(interest);
        return Ok();
    }
}
