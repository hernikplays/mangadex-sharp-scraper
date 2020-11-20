using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace mangadex_sharp_scraper
{
    public class Chapters
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public int MangaId { get; set; }
        public string Chapter { get; set; }
        public string Volume { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public Group[] Groups { get; set; }
        public long Timestamp { get; set; }
        public int Comments { get; set; }
        public string[] Pages { get; set; }
        public string Server { get; set; }
        public string ServerFallback { get; set; }
        public List<string> PageURLs { get; set; }

        public Chapters GetPageURLs()
        {
            if(Pages.Length <1) throw new Exception("No pages found for chapter");
            foreach (string page in this.Pages)
            {
                List<string> e = new List<string>();
                
                e.Add(this.Server + this.Hash + "/" + page);
                PageURLs = e;
            }

            return this;
        }
    }
}
