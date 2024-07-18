using System.Text.Json;
using announce_backend.Models;

namespace announce_backend.Business.VkVideo;

public class VkVideoUrlFetcher(ILogger<VkVideoUrlFetcher> logger)
{
    public async Task<IEnumerable<VkVideoInfo>?> Execute(string vkTokenUrl, string? groupName, int announcesAmount = 0)
    {
        var token = TokenExtractor(vkTokenUrl);
        logger.LogInformation("Attempting to parse video Url's from VK with {VkToken}", 
            vkTokenUrl);

        var requestUrl = PrepareRequestUrl(token, groupName, announcesAmount);

        var responseBody = await MakeRequestFromVk(requestUrl);
        if (responseBody is null) return null;

        var videoInfos = ParseResponseFromVk(responseBody);

        return videoInfos;
    }

    private string PrepareRequestUrl(string token, string? group, int amount)
    {
        if (amount == 0)
        {
            amount = 1;
        }
        
        var ownerId = group is null ? VkGroupIdDictionary.ChannelGroupIdMap.Values.First() 
            : VkGroupIdDictionary.ChannelGroupIdMap[group];
        
        return $"https://api.vk.com/method/video.get?" +
                            $"owner_id=-{ownerId}&" +
                            $"count={amount*2}&" +
                            $"access_token={token}&" +
                            $"v=5.131";
    }

    private async Task<string?> MakeRequestFromVk(string requestUrl)
    {
        using (HttpClient client = new())
        {
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            return await response.Content.ReadAsStringAsync();
        }
    }

    private IEnumerable<VkVideoInfo>? ParseResponseFromVk(string responseBody)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                var root = doc.RootElement;
                var videoInfosJson = root.GetProperty("response").GetProperty("items");

                var videoInfos = JsonSerializer.Deserialize<List<VkVideoInfo>>(videoInfosJson.GetRawText());

                return videoInfos;
            }
        }
        catch (Exception e)
        {
            logger.LogError("Something went wrong processing request to VK. Please check token or group info. Error: {Exception} {ExceptionWhere}", e.Message, e.StackTrace);
            return null;
        }
    }
    
    private string TokenExtractor(string url)
    {
        var extractedToken = url.Split("token=")
            .Last()
            .Split("&expires")
            .First();

        return extractedToken;
    }
}