using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace mangadex_sharp_scraper.Classes
{
    public class Attributes
    {
        [JsonProperty("title")] public string ChapterTitle;
        [JsonProperty("volume")] public int? ChapterVolume;
        [JsonProperty("chapter")] public string? ChapterNumber;
        [JsonProperty("translatedLanguage")] public string ChapterLanguage;
        [NonSerialized] public List<string> PageUrls;
        public string Hash;
        public List<string> Data;

        public async Task<List<string>> FormImageUrls(string id)
        {
            var data = this.Data;
            var hash = this.Hash;
            RestClient _client = new("https://api.mangadex.org");
            _client.UseNewtonsoftJson();
            var request = new RestRequest("at-home/server/" + id);
            var response = _client.Execute<AtHome>(request);
            if (response.ErrorException != null || !response.IsSuccessful)
            {
                string message;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        message = "Manga not found. Make sure you are entering the new UUID and not the old number ID.";
                        break;
                    case HttpStatusCode.BadGateway:
                        message =
                            "There is a problem with the MangaDex servers. Check if you are able to access them or if you have internet access.";
                        break;
                    case HttpStatusCode.GatewayTimeout:
                        message = "The MangaDex servers have timed out.";
                        break;
                    case HttpStatusCode.ServiceUnavailable:
                        message =
                            "Service is unavailable.";
                        break;
                    default:
                        message = response.ErrorMessage;
                        break;
                }
                MessageBox box = new();
                box.Content = Utility.GenerateMessageBox("Error", message,box);
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    box.ShowDialog(desktop.MainWindow);
                }

                return new List<string>();
            }
            
            AtHome home = response.Data;
            List<string> pageUrls = new();
            foreach (var page in data)
            {
                pageUrls.Add($"{home.Url}/data/{hash}/{page}");
            }

            return pageUrls;
        }
    }
}