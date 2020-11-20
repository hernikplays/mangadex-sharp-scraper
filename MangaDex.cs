
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace mangadex_sharp_scraper
{
    public class MangaDex
    {
        public static Manga GetMangaInfo(int id)
        {

            var client = new RestClient("https://mangadex.org/api/v2/");
            var req = "manga/" + id+"?include=chapters";
            var request = new RestRequest(req, DataFormat.Json);
            var response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Response resp = JObject.Parse(response.Content).ToObject<Response>();
                var manga = JObject.Parse(response.Content)["data"]["manga"].ToObject<Manga>();
                manga.Chapters = JObject.Parse(response.Content)["data"]["chapters"].ToObject<List<ChapterLite>>();
                manga.StatusCode = resp.Code;
                return manga;
            }
            else
            {
                Manga re = new Manga
                {
                    StatusCode = (int)response.StatusCode
                };
                return re;
            }
        }

        public static Chapters GetChapterInfo(int id)
        {
            var client = new RestClient("https://mangadex.org/api/v2/");
            var req = "chapter/" + id;
            var request = new RestRequest(req, DataFormat.Json);
            var response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var chapter = JObject.Parse(response.Content)["data"].ToObject<Chapters>();
                chapter.GetPageURLs();
                return chapter;
            }
            else
            {
                throw new Exception("There was a problem with getting info from MangaDex API");
            }
        }
    }
}
