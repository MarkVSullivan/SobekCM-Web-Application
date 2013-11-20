using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Library.HTML
{
    /// <summary> Enumeration is used to allow individual HtmlSubwriters 
    /// (and viewers under those subwriters) to bubble up specific behaviors
    /// to the main HTMl writer and specific subwriter </summary>
    public enum HtmlSubwriter_Behaviors_Enum : byte
    {
        Suppress_Internal_Header,

        Suppress_Header,

        Suppress_Footer,

        Suppress_Banner,

        Item_Subwriter_NonWindowed_Mode,

        Item_Subwriter_Suppress_Bottom_Pagination,
        
        Item_Subwriter_Suppress_Titlebar,

        Item_Subwriter_Suppress_Item_Menu,

        Item_Subwriter_Suppress_Left_Navigation_Bar,

        Item_Subwriter_Requires_Left_Navigation_Bar,

		Item_Subwriter_Full_JQuery_UI,

        MySobek_Subwriter_Mimic_Item_Subwriter
    }
}
