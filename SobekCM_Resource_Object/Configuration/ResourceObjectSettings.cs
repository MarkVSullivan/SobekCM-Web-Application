using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.Configuration
{
    public static class ResourceObjectSettings
    {
        public static Metadata_Configuration MetadataConfig { get; set; }

        static ResourceObjectSettings()
        {
            MetadataConfig = new Metadata_Configuration();
        }
    }
}
