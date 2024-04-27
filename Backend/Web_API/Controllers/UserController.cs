using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BLL.Sevices;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Web_API.DTOs;
using Web_API.Helpers;

namespace Web_API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService, IMapper mapper, DtoValidator validator) : ControllerBase
    {
        // GET: api/<UsersController>
        //TODO Will be removed in the future
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await userService.GetAllUsersAsync();
            var output = mapper.Map<IEnumerable<UserDto>>(users);
            return output;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id:int}")]
        public async Task<UserDto> GetUserById([FromRoute]int id)
        {
            //TODO turn on when site will be ready
            //CheckUserAuth(id);
            validator.CheckId(id);
            var user = await userService.GetUserByIdAsync(id);
            var output = mapper.Map<UserDto>(user);
            return output;
        }

        [HttpGet("username/{userName}")]
        //TODO Will be removed in the future
        public async Task<UserDto> GetUserByUserName(string userName)
        {
            validator.ValidateUsername(userName);
            var user = await userService.GetUserByUserNameAsync(userName);
            var output = mapper.Map<UserDto>(user);
            return output;
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser([FromRoute]int id, [FromBody] UserDto value)
        {
            validator.CheckId(id);
            //TODO turn on when site will be ready
            //CheckUserAuth(id);
            validator.UserDto(value);
            var userModel = mapper.Map<User>(value);
            await userService.UpdateUserAsync(id, userModel);
            return Ok(userModel);
        }

        private void CheckUserAuth(int id)
        {
            if (User?.Claims == null)
            {
                throw new NotAuthorizedRequestException();
            }

            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            if (claim == null)
            {
                throw new NotAuthorizedRequestException();
            }

            var authorised = int.TryParse(claim.Value, out var userId);
            if (!authorised || id != userId)
            {
                throw new ForbiddenRequestException();
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser([FromRoute]int id)
        {
            validator.CheckId(id);
            //TODO turn on when site will be ready
            //CheckUserAuth(id);
            await userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}