using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Google_Sitemap_Generator.Service
{
    public class Crawler
    {
        public delegate void UrlAnalysedDelegade(object Sender, EventArgs oEventArgs);

        public event UrlAnalysedDelegade UrlAnalysedEvent;

        public int QueleLength
        {
            get { return Queue.Count; }
            private set { } 
        }

        public int CollectedCount
        {
            get { return Collected.Count; }
            private set { }
        }

        public int Interval = 500;

        public List<Uri> Collected;

        protected Uri Uri;

        protected Queue<Uri> Queue;

        public Crawler(string uri)
        {
            Uri = new Uri(uri);

            Queue = new Queue<Uri>();
            Queue.Enqueue(Uri);

            Collected = new List<Uri>();
            Collected.Add(Uri);
        }

        public async Task<List<Uri>> CrawlUris()
        {
            while (0 < Queue.Count)
            {
                await ProcessNext();
                await Task.Delay(Interval);
            }

            return Collected;
        }

        public void RaiseUrlAnalysedEvent()
        {
            if (null != UrlAnalysedEvent)
            {
                UrlAnalysedEvent(this, new EventArgs());
            }
        }

        protected async Task ProcessNext()
        {
            Uri uri = Queue.Dequeue();
            List<string> urlList = await GetLinks(uri);
            foreach (string url in urlList)
            {
                try
                {
                    Uri uriCandidate = BuildUri(url);
                    if (IsUriUnique(uriCandidate) && IsPartOfSite(uriCandidate))
                    {
                        Queue.Enqueue(uriCandidate);
                    }
                }
                catch (Exception ex)
                {
                    //it's ok for now... just some shitty url given
                }
            }

            RaiseUrlAnalysedEvent();
        }

        private bool IsUriUnique(Uri uri)
        {
            return !(Collected.Contains(uri) || Queue.Contains(uri));
        }

        private Uri BuildUri(string url)
        {
            if (!IsAbsoluteUrl(url))
            {
                return new Uri(Uri, url);
            }

            return new Uri(url);
        }

        private bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        private bool IsPartOfSite(Uri uri)
        {
            return Uri.IsBaseOf(uri);
        }

        private async Task<List<String>> GetLinks(Uri uri)
        {
            List<String> linkList = new List<string>();

            HtmlWeb htmlWeb = new HtmlWeb();

            try
            {
                HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync(uri.AbsoluteUri);
                IEnumerable<HtmlNode> links = htmlDocument.DocumentNode.Descendants("a").Where(x => x.Attributes.Contains("href"));

                Collected.Add(uri); //collect only existing url's

                foreach (var link in links)
                {
                    linkList.Add(link.Attributes["href"].Value);
                }
            } catch (Exception ex)
            {
                //Just ignore for now
                //TODO some kind of logging here
            }
            
            return linkList;
        }
    }
}
