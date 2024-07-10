using BLL.Sevices;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers.AutoMappers;


[Route("api/[controller]")]
[ApiController]
public class SeedController(
    UserService userService,
    SeedData seedData
) : ControllerBase

{
    [HttpGet("seed/{secretKey}")]
    public async Task<IActionResult> Seed([FromRoute] int secretKey)
    {
        if (secretKey != 42) return BadRequest();

        var users = await userService.GetAllUserAsync();
        if (users.Count() != 0) return BadRequest();
        
        await seedData.Seed();
        return Ok();
    }
}