using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using announce_backend.Models;
using announce_backend.Models.InternalModels;

namespace announce_backend.Business.AnnounceManager;

public class AnnouncePoster(ILogger<AnnouncePoster> logger, IConfiguration configuration)
{
    public async Task<Boolean> Create(IGrouping<string, Announce> grouping)
    {
        using (var client = new HttpClient())
        {
            var apiUrl = ApiUrls.ApiDictionary[grouping.Key];
            
            var apiEndpoint = "promo";
            
            client.DefaultRequestHeaders.Add("Auth", $"Bearer {CreateHeader()}");

            var jsonData = JsonSerializer.Serialize(new { data = grouping.ToArray() });
            var payload = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{apiUrl}{apiEndpoint}", payload);
            
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Successfully created announces on {Site}", apiUrl);
                return true;
            }
            
            var errorString = await response.Content.ReadAsStringAsync();
            logger.LogError("Error! {StatusCode} {Error}", response.StatusCode, errorString);
            return false;
        }
    }
    
    string CreateHeader()
    {
        var auth = $"{DateTime.Now.ToString("dd")}{configuration.GetSection("AnnounceKey").GetValue<string>("Secret")}";

        using (var md5 = MD5.Create())
        {
            var input = Encoding.UTF8.GetBytes(auth);
            var hash = md5.ComputeHash(input);

            var encodedHeader = Convert.ToHexString(hash);

            return encodedHeader.ToLowerInvariant();
        }
    }
}