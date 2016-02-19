using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class Related_Images_ItemViewer : iItemViewer
    {
        public string ViewerBox_CssId { get; private set; }
        public string ViewerBox_InlineStyle { get; private set; }
        public List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors { get; private set; }
        public void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }

        public void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            throw new NotImplementedException();
        }

        public void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }

        public void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }

        public void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }

        public void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }

        public ItemViewer_PageSelector_Type_Enum Page_Selector { get; private set; }
        public int PageCount { get; private set; }
        public int Current_Page { get; private set; }
        public string First_Page_URL { get; private set; }
        public string Previous_Page_URL { get; private set; }
        public string Next_Page_URL { get; private set; }
        public string Last_Page_URL { get; private set; }
        public string[] Go_To_Names { get; private set; }
    }
}
