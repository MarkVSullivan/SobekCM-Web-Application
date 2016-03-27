using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Menu
{
    public interface iItemMenuProvider
    {
        void Add_Main_Menu(TextWriter Output, string CurrentCode, bool ItemRestrictedFromUserByIP, bool ItemCheckedOutByOtherUser, BriefItemInfo CurrentItem, Navigation_Object CurrentMode, User_Object Currentuser, Custom_Tracer Tracer);

    }
}
