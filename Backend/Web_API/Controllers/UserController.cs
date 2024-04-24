using AutoMapper;
using BLL.Models;
using Microsoft.AspNetCore.Mvc;
using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Web_API.DTOs;

namespace Web_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService, IMapper mapper) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<UserInfoRequestDto>> GetUsers()
        {
            var users = await userService.GetAllUsersAsync();
            var output = mapper.Map<IEnumerable<UserInfoRequestDto>>(users);
            return output;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<UserInfoRequestDto> GetUserById(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            var output = mapper.Map<UserInfoRequestDto>(user);
            return output;
        }

        [HttpGet("username/{userName}")]
        public async Task<UserInfoRequestDto> GetUserByUserName(string userName)
        {
            var user = await userService.GetUserByUserNameAsync(userName);
            var output = mapper.Map<UserInfoRequestDto>(user);
            return output;
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task UpdateUser(int id, [FromBody] UserReguestDto value)
        {
            var userModel = mapper.Map<UserModel>(value);
            await userService.UpdateUserAsync(id, userModel);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}