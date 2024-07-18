using System.ComponentModel.DataAnnotations;

namespace announce_backend.Models;

public class RegisterModel
{
    [Required] 
    public string Username { get; set; }
    [Required] 
    public string Password { get; set; }
}