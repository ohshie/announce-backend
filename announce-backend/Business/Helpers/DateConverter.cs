using System.Globalization;
using announce_backend.Models;
using NodaTime;

namespace announce_backend.Business.Helpers;

public class DateConverter(ILogger<DateConverter> logger)
{
    private enum RuDates
    {
        Пн = 1,
        Вт = 2,
        Ср = 3,
        Чт = 4,
        Пт = 5,
        Сб = 6,
        Вс = 7,
        Понедельник  = 1,
        Вторник = 2,
        Среда = 3,
        Четверг = 4,
        Пятница = 5,
        Суббота = 6,
        Воскресенье = 7
    };

    public void Execute(List<Announce> announces)
    {
        foreach (var announce in announces)
        {
            logger.LogInformation("Parsing {AnnounceName} written date {AnnounceDate}", announce.Title, announce.EndDate);
            announce.EndDate = ProcessDate(announce.EndDate);
        }
    }
    
    private string ProcessDate(string unformattedDate)
    {
        DateTime announceDate;
        
        if (unformattedDate.Contains(','))
        {
            var splitOnComma = unformattedDate.Split(',');

            announceDate = splitOnComma[0].Contains('-') ? WhenContainsHyphen(splitOnComma[0]) : DayOfWeekToDate(splitOnComma[0]);
        }
        else
        {
            announceDate = unformattedDate.Contains('-') ? WhenContainsHyphen(unformattedDate) : DayOfWeekToDate(unformattedDate);
        }
        
        return announceDate.ToString("dd.MM.yyyy");
    }

    private DateTime WhenContainsHyphen(string splitOnComma)
    {
        var splitOnHyphen = splitOnComma.Split('-');

        var announceDate = DayOfWeekToDate(splitOnHyphen[1]);
        
        return announceDate;
    }

    private DateTime DayOfWeekToDate(string dayOfTheWeek)
    {
        dayOfTheWeek = dayOfTheWeek.Trim();
        
        if (TryParseExactInvariant(dayOfTheWeek, "dd.MM.yyyy", out var announceDate)) return announceDate;
        
        if (!Enum.TryParse(dayOfTheWeek, out RuDates parsedRuDate)) return GetDefaultAnnounceDate();

        logger.LogWarning("Unable to parse date: {Date}. Using default date", dayOfTheWeek);
        announceDate = GetMonday().AddDays(Convert.ToInt32(parsedRuDate) - 1);
        
        return announceDate;
    }

    /// <summary>
    /// Gets default announce date.
    /// Defaults to next sunday from the date of parsing.
    /// </summary>
    /// <returns>DateTime in form of dd.MM.yyyy containing next sunday date</returns>
    private DateTime GetDefaultAnnounceDate()
    {
        var nextMonday = GetMonday();

        return nextMonday.AddDays(6).Date;
    }
    
    private bool TryParseExactInvariant(string input, string format, out DateTime result)
    {
        return DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    private DateTime GetMonday()
    {
        DateTime.TryParse(LocalDate.FromDateTime(DateTime.Today)
                .Next(IsoDayOfWeek.Monday)
                .ToDateOnly()
                .ToString(),
            new CultureInfo("ru-RU"), 
            DateTimeStyles.AdjustToUniversal, 
            out var nextMonday);

        return Convert.ToDateTime(nextMonday.ToString("dd.MM.yyyy"));
    }
}