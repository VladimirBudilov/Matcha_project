﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BLL.Sevices;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Web_API.DTOs;

namespace Web_API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService, IMapper mapper) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await userService.GetAllUsersAsync();
            var output = mapper.Map<IEnumerable<UserDto>>(users);
            return output;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<UserDto> GetUserById(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            var output = mapper.Map<UserDto>(user);
            return output;
        }

        [HttpGet("username/{userName}")]
        public async Task<UserDto> GetUserByUserName(string userName)
        {
            var user = await userService.GetUserByUserNameAsync(userName);
            var output = mapper.Map<UserDto>(user);
            return output;
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto value)
        {
            if (User?.Claims == null)
            {
                return Unauthorized();
            }

            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            if (claim == null)
            {
                return Unauthorized();
            }

            var authorised = int.TryParse(claim.Value, out var userId);
            if (!authorised || id != userId)
            {
                return Forbid();
            }
            var userModel = mapper.Map<User>(value);
            await userService.UpdateUserAsync(id, userModel);
            return Ok(userModel);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (User?.Claims == null)
            {
                return Unauthorized();
            }

            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            if (claim == null)
            {
                return Unauthorized();
            }

            var authorised = int.TryParse(claim.Value, out var userId);
            if (!authorised || id != userId)
            {
                return Forbid();
            }
            
            await userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}