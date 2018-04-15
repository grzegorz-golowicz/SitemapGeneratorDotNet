using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google_Sitemap_Generator.Common
{
    public class StringWriterUtf8 : StringWriter
    {

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
