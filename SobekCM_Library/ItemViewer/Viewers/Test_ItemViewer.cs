using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Library.HTML;

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class Test_ItemViewer : abstractItemViewer
    {
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Test; }
        }


        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("</td>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/sobekcm_qc.js\"></script>");

            Output.WriteLine(" <script>");
            Output.WriteLine();
            Output.WriteLine("jQuery(document).ready(function () {");
            Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
            Output.WriteLine("  });");
            Output.WriteLine();
            Output.WriteLine("</script>");
            Output.WriteLine();
            Output.WriteLine("<ul id=\"sample-menu-1\" class=\"sf-menu\">");
            Output.WriteLine("<li class=\"current\">");
            Output.WriteLine("  <a href=\"#a\">menu item</a>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#aa\">menu item</a>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li class=\"current\">");
            Output.WriteLine("      <a href=\"#ab\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li class=\"current\"><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#aba\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#abb\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#abc\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#abd\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</li>");
            Output.WriteLine("<li>");
            Output.WriteLine("  <a href=\"#\">menu item</a>");
            Output.WriteLine("</li>");
            Output.WriteLine("<li>");
            Output.WriteLine("  <a href=\"#\">menu item</a>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("    <li>");
            Output.WriteLine("      <a href=\"#\">menu item</a>");
            Output.WriteLine("      <ul>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("        <li><a href=\"#\">menu item</a></li>");
            Output.WriteLine("      </ul>");
            Output.WriteLine("    </li>");
            Output.WriteLine("  </ul>");
            Output.WriteLine("</li>");
            Output.WriteLine("<li>");
            Output.WriteLine("  <a href=\"#\">menu item</a>");
            Output.WriteLine("</li> ");
            Output.WriteLine("</ul>");

            Output.WriteLine("</td>");
        }

        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "qc_set_fullscreen();"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "qc_set_fullscreen();"));
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_QC.css\" /> ");
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
					{
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
						HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
						HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
					};
            }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }
    }
}
