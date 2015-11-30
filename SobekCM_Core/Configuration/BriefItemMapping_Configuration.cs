using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Core.Configuration
{
    public class BriefItemMapping_Configuration
    {
        private static string defaultSetId;
        private static readonly Dictionary<string, List<BriefItemMapping_Mapper>> mappingSetsDictionary;

    }


    public class BriefItemMapping_Mapper
    {
        public string Assembly { get; set; }

        public string Class { get; set; }

        public bool Enabled { get; set; }


    }


}
