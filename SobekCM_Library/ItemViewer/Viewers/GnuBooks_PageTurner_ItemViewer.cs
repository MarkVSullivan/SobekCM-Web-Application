#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Bib_Package.Divisions;

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
        /// <value> This is a single page viewer, so this property always returns FALSE</value>
        public override bool Show_Page_Selector
        {
            get
            {
                return false;
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

        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Returns FALSE since nothing was added to the left navigational bar </returns>
        /// <remarks> For this item viewer, this method does nothing except return FALSE </remarks>
        public override bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("GnuBooks_PageTurner_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("GnuBooks_PageTurner_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }


            // Build the value
            StringBuilder builder = new StringBuilder(15000);

            // Add the division
            builder.AppendLine("          <div id=\"GnuBook\"><p style=\"font-size: 14px;\">Book Turner presentations require a Javascript-enabled browser.</p></div>" + Environment.NewLine );

            // Add the javascript
            builder.AppendLine("<script type=\"text/javascript\"> ");
            builder.AppendLine("  //<![CDATA[");

            // Get the list of jpegs, along with widths and heights
            List<int> width = new List<int>();
            List<int> height = new List<int>();
            List<string> files = new List<string>();
            List<abstract_TreeNode> allpages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;
            foreach (Page_TreeNode thisPage in allpages)
            {
                // Step through each page looking for the jpeg
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    if ((thisFile.MIME_Type(thisFile.File_Extension) == "image/jpeg") && (thisFile.System_Name.ToUpper().IndexOf("THM.JPG") < 0 ))
                    {
                        if (!files.Contains(thisFile.System_Name))
                        {
                            files.Add(thisFile.System_Name);
                            width.Add(thisFile.Width);
                            height.Add(thisFile.Height);
                        }

                        break;
                    }
                }
            }

            // Add the script for this resource
            builder.AppendLine("    // Create the GnuBook object");
            builder.AppendLine("    gb = new GnuBook();");
            builder.AppendLine();
            builder.AppendLine("    // Return the width of a given page");
            builder.AppendLine("    gb.getPageWidth = function(index) {");
            if (width[0] > 0)
            {
                builder.AppendLine("        if (index <= 2) return " + width[0] + ";");
            }
            for (int i = 1; i < width.Count; i++)
            {
                if (width[i] > 0)
                {
                    builder.AppendLine("        if (index == " + (i + 2) + ") return " + width[i] + ";");
                }
            }
            builder.AppendLine("        return 638;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    // Return the height of a given page");
            builder.AppendLine("    gb.getPageHeight = function(index) {");
            if (height[0] > 0)
            {
                builder.AppendLine("        if (index <= 2) return " + height[0] + ";");
            }
            for (int i = 1; i < height.Count; i++)
            {
                if (height[i] > 0)
                {
                    builder.AppendLine("        if (index == " + (i + 2) + ") return " + height[i] + ";");
                }
            }
            builder.AppendLine("        return 825;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    // Return the URI for a page, by index");
            builder.AppendLine("    gb.getPageURI = function(index) {");
            builder.AppendLine("        var imgStr = (index).toString();");
            builder.AppendLine("        if (index < 2) return '" + CurrentMode.Base_URL + "default/images/bookturner/emptypage.jpg';");
            for (int i = 0; i < files.Count; i++  )
            {
                builder.AppendLine("        if (index == " + (i + 2) + ") imgStr = '" + files[i] + "';");
            }
            builder.AppendLine("        if (index > " + (files.Count + 1) + ") return '" + CurrentMode.Base_URL + "default/images/bookturner/emptypage.jpg';");
            string source_url = CurrentItem.SobekCM_Web.Source_URL.Replace("\\", "/");
            if (source_url[source_url.Length - 1] != '/')
                source_url = source_url + "/";
            builder.AppendLine("        return '" + source_url + "' + imgStr;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    // Return which side, left or right, that a given page should be displayed on");
            builder.AppendLine("    gb.getPageSide = function(index) {");
            builder.AppendLine("        if (0 == (index & 0x1)) {");
            builder.AppendLine("            return 'R';");
            builder.AppendLine("        } else {");
            builder.AppendLine("            return 'L';");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    // This function returns the left and right indices for the user-visible");
            builder.AppendLine("    // spread that contains the given index.  The return values may be");
            builder.AppendLine("    // null if there is no facing page or the index is invalid.");
            builder.AppendLine("    gb.getSpreadIndices = function(pindex) {   ");
            builder.AppendLine("        var spreadIndices = [null, null]; ");
            builder.AppendLine("        if ('rl' == this.pageProgression) {");
            builder.AppendLine("            // Right to Left");
            builder.AppendLine("            if (this.getPageSide(pindex) == 'R') {");
            builder.AppendLine("                spreadIndices[1] = pindex;");
            builder.AppendLine("                spreadIndices[0] = pindex + 1;");
            builder.AppendLine("            } else {");
            builder.AppendLine("            // Given index was LHS");
            builder.AppendLine("                spreadIndices[0] = pindex;");
            builder.AppendLine("                spreadIndices[1] = pindex - 1;");
            builder.AppendLine("            }");
            builder.AppendLine("        } else {");
            builder.AppendLine("            // Left to right");
            builder.AppendLine("            if (this.getPageSide(pindex) == 'L') {");
            builder.AppendLine("                spreadIndices[0] = pindex;");
            builder.AppendLine("                spreadIndices[1] = pindex + 1;");
            builder.AppendLine("            } else {");
            builder.AppendLine("                // Given index was RHS");
            builder.AppendLine("                spreadIndices[1] = pindex;");
            builder.AppendLine("                spreadIndices[0] = pindex - 1;");
            builder.AppendLine("            }");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        return spreadIndices;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    // For a given \"accessible page index\" return the page number in the book.");
            builder.AppendLine("    // For example, index 5 might correspond to \"Page 1\" if there is front matter such");
            builder.AppendLine("    // as a title page and table of contents.");
            builder.AppendLine("    gb.getPageNum = function(index) {");
            builder.AppendLine("        return index;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    // Total number of leafs");

            // TRUE FOR EVEN PAGE BOOKS at least
            builder.AppendLine("    gb.numLeafs = " + (files.Count + 4) + ";");
            builder.AppendLine();
            builder.AppendLine("    // Book title and the URL used for the book title link");
            builder.AppendLine("    gb.bookTitle= '" + CurrentItem.Bib_Info.Main_Title.ToString().Replace("'","") + "';");
            builder.AppendLine("    gb.bookUrl = '" + CurrentMode.Base_URL + CurrentMode.BibID + "/" + CurrentMode.VID + "';");
            builder.AppendLine();
            builder.AppendLine("    // Let's go!");
            builder.AppendLine("    gb.init();");


            builder.Append("  //]]>");
            builder.Append("</script>");


            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add(mainLiteral);
        }
    }
}



