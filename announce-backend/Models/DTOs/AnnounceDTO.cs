using System.Text.Json.Serialization;

namespace announce_backend.Models.DTOs;

public class AnnounceDTO
{
    [JsonPropertyName("number")]
    public int Id { get; set; }
    [JsonPropertyName("channelName")]
    public string Channel { get; set; } = string.Empty;
    [JsonPropertyName("announcementName")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("announceDate")]
    public string TextDate { get; set; } = string.Empty;
    [JsonPropertyName("tableBg")] 
    public string TableBg { get; set; } = string.Empty;
}