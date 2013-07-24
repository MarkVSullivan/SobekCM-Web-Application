using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.Configuration;
using SobekCM.Library.HTML;

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class JPEG2000_ItemViewer : abstractItemViewer
    {

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.JPEG2000"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.JPEG2000; }
        }

        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> If the user has not zoomed into the image, 650 is returned, otherwise -1 </value>
        public override int Viewer_Width
        {
            get
            {
                return -1;
            }
        }


        public override void Write_Main_Viewer_Section(System.IO.TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG2000_ItemViewer.Write_Main_Viewer_Section", "Adds the container for the zoomable image");
            }

            Output.WriteLine("<td>");
            Output.WriteLine("<div id=\"sbkJp2_Container\" ></div>");
            Output.WriteLine();
            Output.WriteLine("<script type=\"text/javascript\">");
            Output.WriteLine("   viewer = OpenSeadragon({");
            Output.WriteLine("      id: \"sbkJp2_Container\",");
            Output.WriteLine("      prefixUrl : \"/iipimage/openseadragon/images/\",");
            Output.WriteLine("      showNavigator:  true,");
            Output.WriteLine("      navigatorId:  \"sbkJp2_Navigator\"");
            Output.WriteLine("   });");
            Output.WriteLine();
            Output.WriteLine("   viewer.open(\"" + CurrentMode.Base_URL + "iipimage/iipsrv.fcgi?DeepZoom=//fcla-sobekfs/ufdc/resources/" + CurrentItem.Web.AssocFilePath.Replace("\\","/") +  FileName + ".dzi\");");
            Output.WriteLine("</script>");
            Output.WriteLine("</td>");
        }

        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG2000_ItemViewer.Write_Nav_Bar_Menu_Section", "Adds small thumbnail for image navigation");
            }

            string thumnbnail_text = "THUMBNAIL";
            string click_on_thumbnail_text = "Click on Thumbnail to Recenter Image";

            if (CurrentMode.Language == Web_Language_Enum.French)
            {
                thumnbnail_text = "MINIATURE";
                click_on_thumbnail_text = "Faites un clic sur la minature pour faire centrer l'image";
            }

            if (CurrentMode.Language == Web_Language_Enum.Spanish)
            {
                thumnbnail_text = "MINIATURA";
                click_on_thumbnail_text = "Haga Clic en la Miniatura para centralizar la Imagen";
            }

            Output.WriteLine("        <ul class=\"sbkIsw_NavBarMenu\">");
            Output.WriteLine("          <li class=\"sbkIsw_NavBarHeader\"> " + thumnbnail_text + " </li>");
            Output.WriteLine("          <li class=\"sbkIsw_NavBarMenuNonLink\">");
            Output.WriteLine("            <div id=\"sbkJp2_Navigator\"></div>");
            Output.WriteLine("            <br />");
            Output.WriteLine("          </li>");
            Output.WriteLine("        </ul>");
        }


        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "jp2_set_fullscreen();"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "jp2_set_fullscreen();"));
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(System.IO.TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<script src=\"" + CurrentMode.Base_URL + "iipimage/openseadragon/openseadragon.min.js\"></script>");
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HTML.HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Footer, HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination, HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Requires_Left_Navigation_Bar };
            }
        }
    }
}
