#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Configuration;
using SobekCM.Library.HTML;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> The JPEG2000 viewer allows a zoomable page image to be served through the IIPImage server </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
    public class JPEG2000_ItemViewer : abstractItemViewer
	{
		private readonly bool suppressNavigator;

		/// <summary> Constructor for a new instance of the JPEG2000 viewer </summary>
		public JPEG2000_ItemViewer()
		{
			// Set default height and width
			suppressNavigator = false;

			// Look for a width in the settings
			if (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("JPEG2000 ItemViewer.Suppress Navigator"))
			{
				if ( UI_ApplicationCache_Gateway.Settings.Additional_Settings["JPEG2000 ItemViewer.Suppress Navigator"].ToLower().Trim() != "false")
					suppressNavigator = true;
			}
		}

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

		/// <summary> Stream to which to write the HTML for this subwriter  </summary>
		/// <param name="Output"> Response stream for the item viewer to write directly to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
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

			if (suppressNavigator)
			{
				
				Output.WriteLine("      showNavigator:  false");
			}
			else
			{
				Output.WriteLine("      showNavigator:  true,");
				Output.WriteLine("      navigatorId:  \"sbkJp2_Navigator\",");

				// Doesn't actually set the navigator size (the CSS does), but setting this means
				// OpenSeaDragon won't try to set the width/height as a ratio of the main image.
				Output.WriteLine("      navigatorWidth:  \"195px\",");
				Output.WriteLine("      navigatorHeight:  \"195px\"");
			}

            Output.WriteLine("   });");
            Output.WriteLine();
            Output.WriteLine("   viewer.open(\"" + CurrentMode.Base_URL + "iipimage/iipsrv.fcgi?DeepZoom=" + UI_ApplicationCache_Gateway.Settings.Image_Server_Network.Replace("\\","/") + CurrentItem.Web.AssocFilePath.Replace("\\","/") +  FileName + ".dzi\");");
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

	        if (suppressNavigator)
		        return;

            string thumnbnail_text = "THUMBNAIL";

	        if (CurrentMode.Language == Web_Language_Enum.French)
            {
                thumnbnail_text = "MINIATURE";
            }

            if (CurrentMode.Language == Web_Language_Enum.Spanish)
            {
                thumnbnail_text = "MINIATURA";
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
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<script src=\"" + CurrentMode.Base_URL + "iipimage/openseadragon/openseadragon.min.js\"></script>");
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
				// If the navigator will be  shown, we need a left nav bar, so return different behaviors
	            if (!suppressNavigator)
	            {
		            return new List<HtmlSubwriter_Behaviors_Enum> {HtmlSubwriter_Behaviors_Enum.Suppress_Footer, HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination, HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Requires_Left_Navigation_Bar};
	            }
				return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Footer, HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination };

            }
        }
    }
}
