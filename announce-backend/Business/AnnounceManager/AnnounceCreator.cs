using announce_backend.Business.Helpers;
using announce_backend.Business.VkVideo;
using announce_backend.Models;
using announce_backend.Models.DTOs;

namespace announce_backend.Business.AnnounceManager;

public class AnnounceCreator(ILogger<AnnounceCreator> logger,
    VkVideoUrlFetcher vkVideoUrlFetcher,
    AnnouncePoster announcePoster,
    DateConverter dateConverter,
    Mapper.Mapper mapper)
{
    public async Task<bool> Execute(List<AnnounceDTO> tableAnnounces)
    {
        var announces = mapper.MapAnnounces(tableAnnounces).ToList();
        
        dateConverter.Execute(announces);
        
        await MapVideoUrlsToAnnounces(announces);
        
        if (announces.Any(a => string.IsNullOrEmpty(a.Url)))
        {
            return false;
        }

        var success = await PostAnnounces(announces);
        
        return success;
    }
    
    private async Task MapVideoUrlsToAnnounces(List<Announce> announces)
    {
        foreach (var group in announces.GroupBy(a => a.ChannelGroup))
        {
            var namesAndUrls = await vkVideoUrlFetcher.Execute(groupName: group.Key, 
                announcesAmount: group.Count());
            
            if (namesAndUrls is null || namesAndUrls.Count == 0)
            {
                logger.LogError("Failed to get any video URL's from vk for {GroupName}", group.Key);
                return;
            }

            foreach (var announce in group)
            {
                var nameAndUrl = namesAndUrls.Find(a => a.VideoName == announce.Title);
                if (nameAndUrl is null)
                {
                    logger.LogError("Tried to find {AnnounceTitle} in {GroupName} but none was found", announce.Title, group.Key);
                    return;
                }
                
                announce.Url = nameAndUrl.VideoUrl.Split("&hash")[0];
            }
        }
    }

    private async Task<bool> PostAnnounces(IEnumerable<Announce> announces)
    {
        var groupedAnnounces = announces.GroupBy(a => a.Channel);

        var errorInGroups = string.Empty;
        
        foreach (var announce in groupedAnnounces)
        {
            logger.LogInformation("Attempting to create {AmountOfAnnounces} announces on {SiteName}", 
                announce.Count(),announce.Key);

            var success = await announcePoster.Create(announce);

            if (!success)
            {
                errorInGroups += announce.Key;
            }
        }

        return (string.IsNullOrEmpty(errorInGroups));
    }
    
}