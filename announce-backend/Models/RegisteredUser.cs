using System.ComponentModel.DataAnnotations;
using announce_backend.Models.DTOs;

namespace announce_backend.Models;

public class RegisteredUser
{
    [Key]
    public string Token { get; set; }
    public UserDto User { get; set; }
}