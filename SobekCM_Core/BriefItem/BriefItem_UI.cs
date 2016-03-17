using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Data about a brief digital object item that is 
    /// computed once by the user interface and stored in the user interface
    /// cache for subsequent needs </summary>
    public class BriefItem_UI
    {
        private Dictionary<string, string> viewerCodesDictionary;  

        public List<string> Viewers_By_Priority { get; set; } 

        public List<string> Viewers_Menu_Order { get; set; }



        public bool ContainsViewerCode(string ViewerCode)
        {
            return viewerCodesDictionary.ContainsKey(ViewerCode);
        }

    }
}
