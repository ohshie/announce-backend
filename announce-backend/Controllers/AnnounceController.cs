using announce_backend.Business.AnnounceManager;
using announce_backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace announce_backend.Controllers;

[Authorize]
public class AnnounceController(AnnounceCreator announceCreator) : ControllerBase
{
    [HttpPost("SubmitAnnounces", Name = "SubmitAnnounces")]
    public async Task<ActionResult> SubmitAnnounces([FromBody]List<AnnounceDTO>? announceDtos)
    {
        if (announceDtos is null || announceDtos.Count == 0)
        {
            return BadRequest("Empty announce list");
        }

        var success = await announceCreator.Execute(announceDtos);

        return success ? Ok() : StatusCode(500);
    }
}