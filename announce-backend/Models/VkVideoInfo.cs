using System.Text.Json.Serialization;

namespace announce_backend.Models;

public class VkVideoInfo
{
    [JsonPropertyName("title")] 
    public string VideoName { get; set; } = string.Empty;
    [JsonPropertyName("player")] 
    public string VideoUrl { get; set; } = string.Empty;
    [JsonPropertyName("upload_date")] 
    public string UploadDate { get; set; } = string.Empty;
}