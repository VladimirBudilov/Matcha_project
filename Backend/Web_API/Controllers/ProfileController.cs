using BLL.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersProfileController(UserProfileService userProfileService) : ControllerBase
{
    // GET: api/<UsersController>
    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        return await userProfileService.GetAllUsersAsync();
    }

    // GET api/<UsersController>/5
    [HttpGet("{id}")]
    public async Task<string> Get(int id)
    {
        return await Task.FromResult("value");
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
