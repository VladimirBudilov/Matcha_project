﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BLL.Sevices;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Web_API.DTOs;
using Web_API.DTOs.Request;
using Web_API.Helpers;

namespace Web_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(UserService userService, IMapper mapper, DtoValidator validator, ClaimsService claimsService, ActionService actionService) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById([FromRoute]int id)
        {
            validator.CheckId(id);
            validator.CheckUserAuth(id,User.Claims);
            var userId = claimsService.GetId(User.Claims);
            var isBlocked = await actionService.CheckIfUserIsBlocked(userId, id);
            if (isBlocked) return Forbid();
            var user = await userService.GetUserByIdAsync(id);
            var output = mapper.Map<UserDto>(user);
            return Ok(output);
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser([FromRoute]int id, [FromBody] UserDto value)
        {
            validator.CheckId(id);
            validator.CheckUserAuth(id, User.Claims);
            validator.UserDto(value);
            var userModel = mapper.Map<User>(value);
            await userService.UpdateUserAsync(id, userModel);
            var output = mapper.Map<UserDto>(userModel);
            return Ok(output);
        }
        [HttpPut("{id:int}/update-password")]
        public async Task<IActionResult> UpdateUserPassword([FromRoute]int id, [FromBody] PasswordUpdatingDto value)
        {
            validator.CheckId(id);
            validator.CheckUserAuth(id, User.Claims);
            validator.ValidatePassword(value.NewPassword);
            validator.ValidatePassword(value.OldPassword);
            await userService.UpdatePasswordAsync(id, value.OldPassword, value.NewPassword);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser([FromRoute]int id)
        {
            validator.CheckId(id);
            validator.CheckUserAuth(id, User.Claims);
            var output = await userService.DeleteUserAsync(id);
            return Ok(output);
        }
    }
}