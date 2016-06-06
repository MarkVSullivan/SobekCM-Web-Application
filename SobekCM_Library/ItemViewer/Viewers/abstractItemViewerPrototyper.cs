using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    public abstract class abstractItemViewerPrototyper : iItemViewerPrototyper
    {
        /// <summary> Name of this viewer, which matches the viewer name from the database and 
        /// in the configuration files as well.  This is actually populate by the configuration information </summary>
        public string ViewerType { get; set; }

        /// <summary> Code for this viewer, which can also be set from the configuration information </summary>
        public string ViewerCode { get; set; }

        /// <summary> If this viewer is tied to certain files existing in the digital resource, this lists all the 
        /// possible file extensions this supports (from the configuration file usually) </summary>
        public string[] FileExtensions { get; set; }

        public abstract bool Include_Viewer(BriefItemInfo CurrentItem);

        public abstract bool Override_On_Checkout(BriefItemInfo CurrentItem);

        public abstract bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted);

        public abstract void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems);

        public abstract iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer);
    }
}
