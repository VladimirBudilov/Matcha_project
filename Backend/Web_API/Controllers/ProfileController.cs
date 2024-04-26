using AutoMapper;
using BLL.Sevices;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController(ProfileService profileService, IMapper mapper) : ControllerBase
{
    // GET: api/<UsersController>
    [HttpGet]
    public async Task<IEnumerable<string>> GetAllProfilesInfo()
    {
        return await profileService.GetAllUsersAsync();
    }

    // GET api/<UsersController>/5
    [HttpGet("{id}")]
    public async Task<ProfileDto> GetAllProfileInfo(int id)
    {
        var model =  await profileService.GetFullDataAsync(id);
        var output = mapper.Map<ProfileDto>(model);
        return output;
    }

    // POST api/<UsersController>
    [HttpPost]
    public async Task Post([FromBody] string value)
    {
        await Task.CompletedTask;
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id}")]
    public async Task Put(int id, [FromBody] string value)
    {
        await Task.CompletedTask;
    }

    // DELETE api/<UsersController>/5
    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        await Task.CompletedTask;
    }
}
