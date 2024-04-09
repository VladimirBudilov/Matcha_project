using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Matcha_Carter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    internal class UserController : ControllerBase
    {
        public UserController() { }


        [HttpGet]
        [Route("/{id}")]
        public IActionResult GetUsers(Guid id)
        {
            return Ok("get user with id");
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok("get all users");
        }

        [HttpPost]
        public IActionResult CreateUser()
        {
            return Ok("create user");
        }

        [HttpPut]
        [Route("/{id}")]
        public IActionResult UpdateUser(Guid id)
        {
            return Ok("update user with id");
        }

        [HttpDelete]
        [Route("/{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            return Ok("delete user with id");
        }
    }
}
