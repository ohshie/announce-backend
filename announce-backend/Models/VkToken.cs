using System.ComponentModel.DataAnnotations;

namespace announce_backend.Models;

public class VkToken
{
    [Key] 
    public string ApiToken { get; set; } = string.Empty;
}