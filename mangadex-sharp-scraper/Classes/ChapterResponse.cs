using Newtonsoft.Json;

namespace mangadex_sharp_scraper.Classes
{
    public class ChapterResponse
    {
        public string Result;
        [JsonProperty("data")]
        public Chapter Chapter;
    }
}