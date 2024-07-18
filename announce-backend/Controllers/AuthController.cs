using announce_backend.Business.Auth.Authorization;
using announce_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace announce_backend.Controllers;

public class AuthController(AuthManager authManager) : ControllerBase
{
    [Authorize]
    [HttpPost("CreateNewUser", Name = "CreateNewUser")]
    public async Task<ActionResult<RegisteredUser>> CreateNewUser(RegisterModel user)
    {
        if (string.IsNullOrEmpty(user.Username))
        {
            return BadRequest("Username is empty");
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            return BadRequest("Password is empty");
        }

        var registeredUser = await authManager.CreateNewUser(user);
        if (registeredUser is null)
        {
            return BadRequest("User already exist");
        }
        
        return registeredUser;
    }

    [HttpPost("Login", Name = "Login")]
    public async Task<ActionResult<RegisteredUser>> Login([FromBody]LoginModel loginUser)
    {
        var user = await authManager.Login(loginUser);
        if (user is null)
        {
            return BadRequest("Invalid username or password");
        }

        return Ok(user);
    }
}