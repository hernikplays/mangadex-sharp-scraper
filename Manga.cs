using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace mangadex_sharp_scraper
{
    public class Manga
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string[] Artist { get; set; }
        public string[] Author { get; set; }
        public List<ChapterLite> Chapters { get; set; }
        public string[] AltTitles { get; set; }
        public int[] Tags { get; set; }
        public bool IsHentai { get; set; }
        public long Follows { get; set; }
        public long Views { get; set; }
        public int Comments { get; set; }
        public long LastUploaded { get; set; }
        public string MainCover { get; set; }
        public int StatusCode { get; set; }


    }
}
