using AutoMapper;
using BLL.Sevices;
using DAL.Helpers;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
using Web_API.Helpers;
using Profile = DAL.Entities.Profile;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController(ProfileService profileService, IMapper mapper,
    DtoValidator validator
    ) : ControllerBase
{
    // GET: api/<UsersController>
    [HttpGet]
    public async Task<IEnumerable<ProfileResponseDto>> GetAllProfilesInfo([FromQuery]FilterParameters filter)
    {
        var output = await profileService.GetFullProfilesAsync(filter);
        var profiles = mapper.Map<IEnumerable<ProfileResponseDto>>(output);
        return profiles;
    }
    
    // GET api/<UsersController>/5
    [HttpGet("{id:int}")]
    public async Task<ProfileResponseDto> GetAllProfileInfoById([FromRoute]int id)
    {
        validator.CheckId(id);
        var model =  await profileService.GetFullProfileByIdAsync(id);
        var output = mapper.Map<ProfileResponseDto>(model);
        return output;
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id:int}")]
    public async Task UpdateProfile([FromRoute]int id, [FromBody] ProfileRequestDto profile)
    {
        validator.CheckId(id);
        validator.ProfileRequestDto(profile);
        var model = mapper.Map<Profile>(profile);
        await profileService.UpdateProfileAsync(id, model);
    }
}
