using Newtonsoft.Json;

namespace mangadex_sharp_scraper.Classes
{
    public class AtHome
    {
        [JsonProperty("baseUrl")]
        public string? Url;
    }
}