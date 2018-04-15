using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Google_Sitemap_Generator.Model;

namespace Google_Sitemap_Generator.Service
{
    class SiteMapGenerator
    {
        protected List<Uri> UriList;

        public string SiteChangeFreq = "daily";

        public DateTimeOffset SiteLastMod;

        public SiteMapGenerator(List<Uri> uriList)
        {
            UriList = uriList;
            SiteLastMod = DateTimeOffset.Now;
        }

        public string GetGoogleSitemapAsString()
        {
            string serializedSitemap;

            using (StringWriter sw = new Common.StringWriterUtf8())
            {
                XmlSerializer xs = new XmlSerializer(typeof(Sitemap));

                Sitemap sitemap = GetGoogleSitemap();

                xs.Serialize(sw, sitemap);
                serializedSitemap = sw.ToString();
            }

            return serializedSitemap;
        }

        protected Sitemap GetGoogleSitemap()
        {
            Sitemap sitemap = new Sitemap();
            foreach (Uri uri in UriList)
            {
                sitemap.Add(getLocationFromUri(uri));
            }

            return sitemap;
        }

        private Location getLocationFromUri(Uri uri)
        {
            return new Location(uri.AbsoluteUri, SiteChangeFreq, SiteLastMod);
        }
    }
}
