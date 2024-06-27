using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTOs;
using Web_API.Helpers;

namespace Web_API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class FileManagerController(
    DtoValidator validator,
    ProfileService profileService,
    ClaimsService claimsService
    ) : ControllerBase
{
  [HttpPost("uploadPhoto/{userId:int}")]
    public async Task<IActionResult> UploadPhoto([FromRoute]int userId, IFormFile file, [FromQuery] bool isMain = false)
    {
        //TODO add validation
        
    if (file == null || file.Length == 0)
    {
        return BadRequest("File is null or empty");
    }
    
    validator.CheckUserAuth(userId, User.Claims);
    
    using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    
    
    await profileService.UploadPhotoAsync(userId, memoryStream.ToArray(), isMain);
    
    return Ok("File uploaded successfully");
    }
    
    [HttpDelete("deletePhoto/{userId:int}")]
    public async Task<IActionResult> DeletePhoto([FromRoute]int userId, [FromQuery]int photoId, [FromQuery] bool isMain = false)
    {
        //TODO add validation
        
        profileService.DeletePhotoASync(userId, photoId, isMain);
        return Ok("Photo deleted successfully");
    }
    
}