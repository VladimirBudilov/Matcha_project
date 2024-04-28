using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileManagerController : ControllerBase
{
    [HttpPost("uploadPhoto/{userId:long}")]
    public async Task<IActionResult> UploadPhoto(long id, [FromForm] IFormFile file)
    {
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
    public async Task<IActionResult> DeletePhoto([FromRoute]long userId, long photoId)
    {
        //TODO implement deleting photo from database

        return Ok();
    }
    
}