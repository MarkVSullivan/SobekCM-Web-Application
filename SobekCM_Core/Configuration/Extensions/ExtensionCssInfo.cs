using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Core.Configuration.Extensions
{
    public enum ExtensionCssInfoConditionEnum : byte
    {
        // always|admin|item|metadata|mysobek

        always,

        admin,

        item,

        metadata,

        mysobek,

        aggregation,

        results
    }
    
    public class ExtensionCssInfo
    {
        public ExtensionCssInfoConditionEnum Condition { get; set; }

        public string URL { get; set; }
    }
}
