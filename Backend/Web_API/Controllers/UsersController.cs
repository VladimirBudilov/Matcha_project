using AutoMapper;
using BLL.Models;
using Microsoft.AspNetCore.Mvc;
using BLL.Sevices;
using Web_API.DTOs;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(UserService userService, IMapper mapper) : ControllerBase
    {
 

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<UserInfoDto>> Get()
        {
            return new List<UserInfoDto>();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<UserInfoDto> Get(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            var output = mapper.Map<UserInfoDto>(user);
            return output;
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task CreateNewUser([FromBody] UserReguestDTO value)
        {
            var UserModel = mapper.Map<UserModel>(value);
            await userService.CreateUserAsync(UserModel);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] UserReguestDTO value)
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
}