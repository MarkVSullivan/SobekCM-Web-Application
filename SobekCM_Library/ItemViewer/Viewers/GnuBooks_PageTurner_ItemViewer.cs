#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Library.HTML;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

#endregion


namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the item in a full-screen implementation of GnuBooks page turner. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class GnuBooks_PageTurner_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.GnuBooks_PageTurner"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.GnuBooks_PageTurner; }
        }


        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This functions essentially like a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }


        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }


        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1, since this is not valid for this viewer </value>
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
                Tracer.Add_Trace("GnuBooks_PageTurner_ItemViewer.Write_Main_Viewer_Section", "");
            }


            // Add the division
            Output.WriteLine("          <div id=\"GnuBook\"><p style=\"font-size: 14px;\">Book Turner presentations require a Javascript-enabled browser.</p></div>" + Environment.NewLine);


            // Add the javascript
            Output.WriteLine("<script type=\"text/javascript\"> ");
            Output.WriteLine("  //<![CDATA[");


            // Get the list of jpegs, along with widths and heights
            List<int> width = new List<int>();
            List<int> height = new List<int>();
            List<string> files = new List<string>();
            List<string> pagename = new List<string>();
            List<abstract_TreeNode> allpages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;
            foreach (Page_TreeNode thisPage in allpages)
            {
                // Step through each page looking for the jpeg
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    if ((thisFile.MIME_Type(thisFile.File_Extension) == "image/jpeg") && (thisFile.System_Name.ToUpper().IndexOf("THM.JPG") < 0))
                    {
                        if (!files.Contains(thisFile.System_Name))
                        {
                            pagename.Add(thisPage.Label);
                            files.Add(thisFile.System_Name);
                            width.Add(thisFile.Width);
                            height.Add(thisFile.Height);
                        }


                        break;
                    }
                }
            }


            // Add the script for this resource
            Output.WriteLine("    // Create the GnuBook object");
            Output.WriteLine("    gb = new GnuBook();");
            Output.WriteLine();
            Output.WriteLine("    // Return the width of a given page");
            Output.WriteLine("    gb.getPageWidth = function(index) {");
            if (width[0] > 0)
            {
                Output.WriteLine("        if (index <= 2) return " + width[0] + ";");
            }
            for (int i = 1; i < width.Count; i++)
            {
                if (width[i] > 0)
                {
                    Output.WriteLine("        if (index == " + (i + 2) + ") return " + width[i] + ";");
                }
            }
            Output.WriteLine("        return 638;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // Return the height of a given page");
            Output.WriteLine("    gb.getPageHeight = function(index) {");
            if (height[0] > 0)
            {
                Output.WriteLine("        if (index <= 2) return " + height[0] + ";");
            }
            for (int i = 1; i < height.Count; i++)
            {
                if (height[i] > 0)
                {
                    Output.WriteLine("        if (index == " + (i + 2) + ") return " + height[i] + ";");
                }
            }
            Output.WriteLine("        return 825;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // Return the URI for a page, by index");
            Output.WriteLine("    gb.getPageURI = function(index) {");
            Output.WriteLine("        var imgStr = (index).toString();");
            Output.WriteLine("        if (index < 2) return '" + CurrentMode.Base_URL + "default/images/bookturner/emptypage.jpg';");
            for (int i = 0; i < files.Count; i++)
            {
                Output.WriteLine("        if (index == " + (i + 2) + ") imgStr = '" + files[i] + "';");
            }
            Output.WriteLine("        if (index > " + (files.Count + 1) + ") return '" + CurrentMode.Base_URL + "default/images/bookturner/emptypage.jpg';");
            string source_url = CurrentItem.Web.Source_URL.Replace("\\", "/");
            if (source_url[source_url.Length - 1] != '/')
                source_url = source_url + "/";
            Output.WriteLine("        return '" + source_url + "' + imgStr;");
            Output.WriteLine("    }");
            Output.WriteLine();


            Output.WriteLine("    // Return the page label for a page, by index");
            Output.WriteLine("    gb.getPageName = function(index) {");
            Output.WriteLine("        var imgStr = '" + translator.Get_Translation("Page", CurrentMode.Language) + "' + this.getPageNum(index);");
            for (int i = 0; i < files.Count; i++)
            {
                Output.WriteLine("        if (index == " + (i + 2) + ") imgStr = '" + pagename[i] + "';");
            }


            Output.WriteLine("        return imgStr;");
            Output.WriteLine("    }");
            Output.WriteLine();


            Output.WriteLine("    // Return which side, left or right, that a given page should be displayed on");
            Output.WriteLine("    gb.getPageSide = function(index) {");
            Output.WriteLine("        if (0 == (index & 0x1)) {");
            Output.WriteLine("            return 'R';");
            Output.WriteLine("        } else {");
            Output.WriteLine("            return 'L';");
            Output.WriteLine("        }");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // This function returns the left and right indices for the user-visible");
            Output.WriteLine("    // spread that contains the given index.  The return values may be");
            Output.WriteLine("    // null if there is no facing page or the index is invalid.");
            Output.WriteLine("    gb.getSpreadIndices = function(pindex) {   ");
            Output.WriteLine("        var spreadIndices = [null, null]; ");
            Output.WriteLine("        if ('rl' == this.pageProgression) {");
            Output.WriteLine("            // Right to Left");
            Output.WriteLine("            if (this.getPageSide(pindex) == 'R') {");
            Output.WriteLine("                spreadIndices[1] = pindex;");
            Output.WriteLine("                spreadIndices[0] = pindex + 1;");
            Output.WriteLine("            } else {");
            Output.WriteLine("            // Given index was LHS");
            Output.WriteLine("                spreadIndices[0] = pindex;");
            Output.WriteLine("                spreadIndices[1] = pindex - 1;");
            Output.WriteLine("            }");
            Output.WriteLine("        } else {");
            Output.WriteLine("            // Left to right");
            Output.WriteLine("            if (this.getPageSide(pindex) == 'L') {");
            Output.WriteLine("                spreadIndices[0] = pindex;");
            Output.WriteLine("                spreadIndices[1] = pindex + 1;");
            Output.WriteLine("            } else {");
            Output.WriteLine("                // Given index was RHS");
            Output.WriteLine("                spreadIndices[1] = pindex;");
            Output.WriteLine("                spreadIndices[0] = pindex - 1;");
            Output.WriteLine("            }");
            Output.WriteLine("        }");
            Output.WriteLine();
            Output.WriteLine("        return spreadIndices;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // For a given \"accessible page index\" return the page number in the book.");
            Output.WriteLine("    // For example, index 5 might correspond to \"Page 1\" if there is front matter such");
            Output.WriteLine("    // as a title page and table of contents.");
            Output.WriteLine("    gb.getPageNum = function(index) {");
            Output.WriteLine("        return index;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // Total number of leafs");


            // TRUE FOR EVEN PAGE BOOKS at least
            Output.WriteLine("    gb.numLeafs = " + (files.Count + 4) + ";");
            Output.WriteLine();
            Output.WriteLine("    // Book title and the URL used for the book title link");
            Output.WriteLine("    gb.bookTitle= '" + CurrentItem.Bib_Info.Main_Title.ToString().Replace("'", "") + "';");
            Output.WriteLine("    gb.bookUrl = '" + CurrentMode.Base_URL + CurrentMode.BibID + "/" + CurrentMode.VID + "';");
            Output.WriteLine();
            Output.WriteLine("    // Let's go!");
            Output.WriteLine("    gb.init();");




            Output.Write("  //]]>");
            Output.Write("</script>");
        }


        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_BookTurner.css\" /> ");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/bookturner/jquery-1.2.6.min.js\"></script> ");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/bookturner/jquery.easing.1.3.js\"></script> ");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/bookturner/bookturner.js\"></script>    ");


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
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar,
						HtmlSubwriter_Behaviors_Enum.Suppress_Header
                    };
            }
        }


        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Clear();
        }


    }
}
