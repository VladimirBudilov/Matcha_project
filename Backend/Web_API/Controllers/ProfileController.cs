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
public class ProfileController(ProfileService profileService, IMapper mapper,
    DtoValidator validator
    ) : ControllerBase
{
    // GET: api/<UsersController>
    [HttpGet]
    public async Task<ProfilesData> GetAllProfilesInfo([FromQuery]SearchParameters search,
        [FromQuery] SortParameters sort,[FromQuery] PaginationParameters pagination)
    {
        /*validator.CheckSearchParameters(search);
        validator.CheckSortParameters(sort);
        validator.CheckPaginationParameters(pagination);*/
        
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
    [HttpGet("{id:long}")]
    public async Task<ProfileFullDataForOtherUsersDto> GetProfileFullDataById([FromRoute]long id)
    {
        validator.CheckId(id);
        //TODO change fem rating if check somebody else's profile
        
        var model =  await profileService.GetFullProfileByIdAsync(id);
        var output = mapper.Map<ProfileFullDataForOtherUsersDto>(model);
        return output;
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id:long}")]
    public async Task UpdateProfile([FromRoute]long id, [FromBody] ProfileDto profileCreation)
    {
        validator.CheckUserAuth(id, User.Claims);
        validator.CheckId(id);
        validator.ProfileRequestDto(profileCreation);
        var model = mapper.Map<Profile>(profileCreation);
        await profileService.UpdateProfileAsync(id, model);
    }
}
