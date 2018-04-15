using System.Xml.Serialization;
using System.Collections.Generic;

namespace Google_Sitemap_Generator.Model
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        private List<Location> map { get; set; }

        public Sitemap()
        {
            map = new List<Location>();
        }

        [XmlElement("url")]
        public Location[] Locations
        {
            get
            {
                Location[] items = new Location[map.Count];
                map.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null)
                    return;
                Location[] items = (Location[])value;
                map.Clear();
                foreach (Location item in items)
                    map.Add(item);
            }
        }

        public void Add(Location item)
        {
            map.Add(item);
        }

    }
}
