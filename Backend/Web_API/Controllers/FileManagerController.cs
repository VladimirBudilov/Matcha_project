using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.Helpers;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class FileManagerController(DtoValidator validator) : ControllerBase
{
    [HttpPost("uploadPhoto/{userId:long}")]
    public async Task<IActionResult> UploadPhoto(long id, [FromBody] IFormFile file, [FromQuery] bool isMain = false)
    {
        validator.CheckUserAuth(id, User.Claims);
        if (file == null)
        {
            return BadRequest("File is null");
        }

        if (file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        //TODO implement Addint photo to database

        return Ok();
    }
    
    [HttpPost("deletePhoto/{userId:long}")]
    public async Task<IActionResult> DeletePhoto([FromRoute]long userId, long photoId, [FromQuery] bool isMain = false)
    {
        validator.CheckUserAuth(userId, User.Claims);
        //TODO implement deleting photo from database

        return Ok();
    }
    
}