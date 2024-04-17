using AutoMapper;
using BLL.Models;
using Microsoft.AspNetCore.Mvc;
using BLL.Sevices;
using Web_API.DTOs;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService, IMapper mapper) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<UserInfoDto>> GetUsers()
        {
            var users = await userService.GetAllUsersAsync();
            var output = mapper.Map<IEnumerable<UserInfoDto>>(users);
            return output;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<UserInfoDto> GetUserById(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            var output = mapper.Map<UserInfoDto>(user);
            return output;
        }
        
        [HttpGet("username/{userName}")]
        public async Task<UserInfoDto> GetUserByUserName(string userName)
        {
            var user = await userService.GetUserByUserNameAsync(userName);
            var output = mapper.Map<UserInfoDto>(user);
            return output;
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task CreateNewUser([FromBody] UserReguestDTO value)
        {
            var userModel = mapper.Map<UserModel>(value);
            await userService.RegisterUserAsync(userModel);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task UpdateUser(int id, [FromBody] UserReguestDTO value)
        {
            var userModel = mapper.Map<UserModel>(value);
            await userService.UpdateUserAsync(id, userModel);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task DeleteUser(int id)
        {
            await userService.DeleteUserAsync(id);
        }
    }
}