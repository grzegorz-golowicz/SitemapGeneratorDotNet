using System;
using System.Xml.Serialization;

namespace Google_Sitemap_Generator.Model
{
    public class Location
    {
        public Location(string uri, string changeFreq, DateTimeOffset lastMod)
        {
            Url = uri;
            ChangeFreq = changeFreq;
            LastMod = lastMod.ToString("yyyy-MM-dd");
        }

        public Location()
        {

        }

        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("lastmod")]
        public string LastMod { get; set; }

        [XmlElement("changefreq")]
        public string ChangeFreq { get; set; }
    }
}
