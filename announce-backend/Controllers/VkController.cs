using announce_backend.Business.VkVideo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace announce_backend.Controllers;

[Authorize]
public class VkController(VkTokenManager tokenManager) : ControllerBase
{
    [HttpGet("CheckSavedVkToken", Name = "CheckSavedVkToken")]
    public async Task<ActionResult> CheckSavedVkToken()
    {
        var success = await tokenManager.CheckTokenOnRequest();
        if (!success)
        {
            return StatusCode(500, "Please update VK Token");
        }

        return Ok();
    }

    [HttpPost("UpdateVkToken", Name = "UpdateVkToken")]
    public async Task<ActionResult> UpdateVkToken([FromBody]string newVkToken)
    {
        if (string.IsNullOrEmpty(newVkToken))
        {
            return BadRequest("Token cannot be empty string");
        }

        if (newVkToken.Contains(' '))
        {
            return BadRequest("VkToken cannot contain white spaces");
        }

        var updatedVkToken = await tokenManager.UpdateCurrentToken(newVkToken);
        if (updatedVkToken is null)
        {
            return BadRequest("Provided token did not work. Please doublecheck your token.");
        }
        
        return Ok("Token updated");
    }
}