using announce_backend.Models;
using announce_backend.Models.DTOs;

namespace announce_backend.Business.Mapper;

public class Mapper(ILogger<Mapper> logger)
{
    public List<Announce> MapAnnounces(List<AnnounceDTO> tableAnnounces)
    {
        List<Announce> announces = [];
        
        foreach (var announce in tableAnnounces)
        {
            logger.LogInformation("Mapping {AnnounceTitle}", announce.Name);
            var properAnnounce = new Announce
            {
                Title = announce.Name.TrimEnd(),
                Channel = announce.Channel,
                StartDate = DateTime.Today.ToString("dd.MM.yyyy"),
                EndDate = announce.TextDate,
                ScheduleDate = announce.TextDate,
            };

            ChannelGroupSorter(properAnnounce);
            
            announces.Add(properAnnounce);
        }

        return announces;
    }
    
    private void ChannelGroupSorter(Announce announce)
    {
        announce.ChannelGroup = announce.Channel switch
        {
            "Фан" => "Фан",
            "МЗК" => "МЗК",
            "НСТ" => "НСТ",
            _ => "Кино"
        };
    }
}