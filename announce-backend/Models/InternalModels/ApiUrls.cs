namespace announce_backend.Models.InternalModels;

public static class ApiUrls
{
    public static readonly Dictionary<string, string> ApiDictionary = new()
    {
        {"РР", "https://rusroman.ru/admin/api/"},
        {"РД", "https://rudetective.tv/admin/api/"},
        {"РБ", "https://bestrussia.tv/admin/api/"},
        {"Ком","https://komedia-tv.ru/admin/api/"},
        {"Син","https://cinetv.ru/admin/api/"},
        {"Фан","https://fantv.ru/admin/api/"},
        {"МЗК","https://mosfilmgold.ru/admin/api/"},
        {"НСТ","https://strashnoe.tv/admin/api/"},
    };
}