using System.Text.Json;
using announce_backend.DAL.AnnounceDbContext;
using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using announce_backend.Models;
using announce_backend.Models.InternalModels;

namespace announce_backend.Business.VkVideo;

public class VkTokenManager(IVkTokenRepository vkTokenRepository, IUnitOfWork<AnnounceDbContext> unitOfWork,
    ILogger<VkTokenManager> logger)
{
    private async Task<VkToken?> GetCurrentToken()
    {
        var currentToken = await vkTokenRepository.GetDefault();

        if (!string.IsNullOrEmpty(currentToken.ApiToken)) return currentToken;
        
        logger.LogError("No token is currently saved");
        return null;
    }

    public async Task<VkToken?> UpdateCurrentToken(string token)
    {
        var vkToken = new VkToken
        {
            ApiToken = token
        };
            
        var tokenIsValid = await CheckTokenValidity(vkToken);
        if (!tokenIsValid) return null;

        var currentToken = await GetCurrentToken();
        if (currentToken is null)
        {
            await vkTokenRepository.Add(vkToken);
        }
        else
        {
            await vkTokenRepository.Update(vkToken);
        }
        
        await unitOfWork.Save();
        return vkToken;
    }

    public async Task<bool> CheckTokenOnRequest()
    {
        var token = await GetCurrentToken();
        if (token is null)
        {
            logger.LogError("Currently there are no saved Token, please add one");
            return false;
        }

        var success = await CheckTokenValidity(token);
        return success;
    }

    private async Task<bool> CheckTokenValidity(VkToken token)
    {
        var requestUrl = $"https://api.vk.com/method/video.get?" +
                         $"owner_id=-{VkGroupIdDictionary.ChannelGroupIdMap.First().Value}&" +
                         $"access_token={token.ApiToken}&" +
                         $"v=5.131";

        try
        {
            using (HttpClient httpClient = new())
            {
                var response = await httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                var success = JsonDocument.Parse(content).RootElement.TryGetProperty("error", out var error);
                if (!success) return true;
                
                logger.LogError("Provided key: \n {ApiKey}\nis not the correct one, please check your key", token.ApiToken);
                
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError("Something went horribly wrong when attempting to check validity of {VkToken} token\n" +
                            "{Exception}", token.ApiToken, e.Message);
            return false;
        }
    }
}