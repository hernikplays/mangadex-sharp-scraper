
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace mangadex_sharp_scraper.Classes
{
    public class MangaDex
    {
        public static async Task<List<Chapter>> GetChapterList(string uuid,string lang)
        {
            RestClient client = new("https://api.mangadex.org");
            var request = new RestRequest($"chapter?manga={uuid}&limit=100&translatedLanguage={lang}");
            var response = await client.ExecuteAsync(request);
            if (response.ErrorException != null || response.StatusCode != HttpStatusCode.OK)
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
                box.Content = Utility.GenerateMessageBox("Error", message, box);
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    box.ShowDialog(desktop.MainWindow);
                }
                return new List<Chapter>();
            }
            var root = JObject.Parse(response.Content);

            List<ChapterResponse>? chaptersResponses = root["results"]?.ToObject<List<ChapterResponse>>();
            List<Chapter> chapters = new List<Chapter>();
            foreach (var chapter in chaptersResponses)
            {
                chapter.Chapter.Attributes.PageUrls = await chapter.Chapter.Attributes.FormImageUrls(chapter.Chapter.Id);
                chapters.Add(chapter.Chapter);
                await Task.Delay(1000);
            }
            return chapters;
        }

        public static async Task<string> GetMangaTitle(string uuid)
        {
            RestClient client = new("https://api.mangadex.org");
            var request = new RestRequest($"manga/{uuid}");
            var response = await client.ExecuteAsync(request);
            if (response.ErrorException != null || response.StatusCode != HttpStatusCode.OK)
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
                box.Content = Utility.GenerateMessageBox("Error", message, box);
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    box.ShowDialog(desktop.MainWindow);
                }
                return "";
            }
            var root = JObject.Parse(response.Content);

            return root["data"]["attributes"]["title"]["en"].ToString();
        }
    }
}