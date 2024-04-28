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
        
        public async Task<IActionResult> UpdateUserPassword([FromRoute]int id, [FromBody] PasswordUpdatingDto value)
        {
            validator.CheckId(id);
            //TODO turn on when site will be ready
            //CheckUserAuth(id);
            validator.ValidatePassword(value.NewPassword);
            validator.ValidatePassword(value.OldPassword);
            await userService.UpdatePasswordAsync(id, value.OldPassword, value.NewPassword);
            return Ok();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser([FromRoute]int id)
        {
            validator.CheckId(id);
            //TODO turn on when site will be ready
            //CheckUserAuth(id);
            var output = await userService.DeleteUserAsync(id);
            return Ok(output);
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
    }
}