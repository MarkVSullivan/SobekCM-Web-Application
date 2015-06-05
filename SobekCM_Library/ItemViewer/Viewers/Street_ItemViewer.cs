#region Using directives

using System.IO;
using System.Text;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Items.Authority;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the list of streets (from the authority database or metadata) associated with a digital resource. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Street_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Streets"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Streets; }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 800 </value>
        public override int Viewer_Width
        {
            get
            {
                return 800;
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

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Street_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Build the value
            //Map_Streets_DataSet streets = SobekCM_Database.Get_All_Streets_By_Item(CurrentItem.Web.ItemID, Tracer);

            //if (streets == null)
            //{
            //    Output.WriteLine("<br />");
            //    Output.WriteLine("<center><b>UNABLE TO LOAD STREETS FROM DATABASE</b></center>");
            //    Output.WriteLine("<br />");
            //    CurrentMode.Mode = Display_Mode_Enum.Contact;
            //    Output.WriteLine("<center>Click <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">here</a> to report this issue.</center>");
            //    Output.WriteLine("<br />");
            //    CurrentMode.Mode = Display_Mode_Enum.Item_Display;
            //}
            //else
            //{
            //    // Save the current viewer code
            //    string current_view_code = CurrentMode.ViewerCode;

            //    // Start the citation table
            //    Output.WriteLine("\t\t<!-- STREET VIEWER OUTPUT -->" );
            //    Output.WriteLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>Index of Streets</b></span></td></tr>" );
            //    Output.WriteLine("\t\t<tr><td class=\"SobekDocumentDisplay\">");
            //    Output.WriteLine("\t\t\t<div class=\"SobekCitation\">" );

            //    // Get the list of streets from the database
            //    Create_Street_Index(Output, streets);

            //    // Finish the citation table
            //    Output.WriteLine("\t\t\t</div>" );
            //    Output.WriteLine("\t\t</td>" );
            //    Output.WriteLine("\t\t<!-- END STREET VIEWER OUTPUT -->" );

            //    // Restore the mode
            //    CurrentMode.ViewerCode = current_view_code;
            //}
        }

        /// <summary> Build the HTML for the street index for this item </summary>
        /// <param name="Output"> Stream to write into </param>
        /// <param name="streets"> Dataset containig all of the streets linked to this item </param>
        protected internal void Create_Street_Index(TextWriter Output, Map_Streets_DataSet streets)
        {
            string currentView = CurrentMode.ViewerCode;

            // This will be presented in a table, so start the table
            Output.WriteLine("\n\n<!-- Start output from GEMS_Map_Object.Street_Index -->");
            Output.WriteLine("<table width=\"100%\">");

            // Determine (roughly) how many rows for each side
            // of the column
            int rows_per_column = streets.Streets.Count  / 3;
            if (( streets.Streets.Count % 3 ) > 0 )
                rows_per_column++;

            // Start the large table for each section
            Output.WriteLine("\t<tr>");
            Output.WriteLine("\t\t<td width=\"32%\" valign=\"top\">");

            // Create the first column of street information
            Insert_One_Street_Column( Output, streets, 0, rows_per_column );			

            // Move to second column of large table, after making a small margin column
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<td width=\"2%\"></td> <!-- Spacer Column -->");
            Output.WriteLine("\t\t<td width=\"32%\" valign=\"top\">");

            // Create the second column of street information
            Insert_One_Street_Column( Output, streets, rows_per_column, (2* rows_per_column) - 1 );		

            // Move to third column of large table, after making a small margin column
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<td width=\"2%\"></td> <!-- Spacer Column -->");
            Output.WriteLine("\t\t<td width=\"32%\" valign=\"top\">");

            // Create the third column of street information
            Insert_One_Street_Column( Output, streets, (2 * rows_per_column), streets.Streets.Count );	

            // Finish off the large table
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t<tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<!-- End output from GEMS_Map_Object.Street_Index -->");

            CurrentMode.ViewerCode = currentView;
        }

        /// <summary> Insert the HTML for a single street column into the Stringbuilder </summary>
        /// <param name="streets"> Dataset containig all of the streets linked to this item </param>
        /// <param name="Output"> Response stream to write directly to </param>
        /// <param name="startRow"> Row of the set of streets to begin on </param>
        /// <param name="endRow"> Last row in the set of streets </param>
        protected internal void Insert_One_Street_Column( TextWriter Output, Map_Streets_DataSet streets, int startRow, int endRow )
        {
            // Declare some variables for looping
            string lastStreetName = "%";

            // Start the table for this row
            Output.WriteLine("\t\t\t<table width=\"100%\"> <!-- Table to display a single column of street information -->");

            // Now, loop through all the results
            int i = startRow;
            while ( i < endRow )
            {
                // Get this street
                Map_Streets_DataSet.StreetsRow thisStreet = streets.Streets[ i++ ];

                // If this street name starts with a new letter, add the letter now
                if ( thisStreet.StreetName.Trim()[0] != lastStreetName[0] )
                {
                    // Add a row for the letter
                    Output.WriteLine("\t\t\t\t<tr> <td colspan=\"3\" class=\"bigletter\" align=\"center\">" + (thisStreet.StreetName.Trim())[0] + "</td> </tr>");
                    lastStreetName = thisStreet.StreetName.Trim();
                }

                // Start this row
                Output.WriteLine("\t\t\t\t<tr class=\"index\">");

                // Add the street name and direction
                if (( !thisStreet.IsStreetDirectionNull() ) && ( thisStreet.StreetDirection.Length > 0 ))
                    Output.WriteLine("\t\t\t\t\t<td>" + thisStreet.StreetName + ", " + thisStreet.StreetDirection + "<td>");
                else
                    Output.WriteLine("\t\t\t\t\t<td>" + thisStreet.StreetName + "<td>");

                // Determine the second column of data and add it
                Output.WriteLine("\t\t\t\t\t<td align=\"right\">" + street_display(thisStreet) + "</td>");

                // Add the link to the sheet
                CurrentMode.ViewerCode = thisStreet.PageSequence.ToString();
                Output.WriteLine("\t\t\t\t\t<td><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">" + thisStreet.PageName.Replace("Sheet", "").Trim() + "</a></td>");				

                // End this row
                Output.WriteLine("\t\t\t\t</tr>");
            }

            // End this table
            Output.WriteLine("\t\t\t</table>");
        }

        /// <summary> Build the information to display about the street, depending on what
        /// values are present in the Strongly typed street datarow </summary>
        /// <param name="thisStreet"> Strongly typed street datarow </param>
        /// <returns> text string (non-HTML) </returns>
        private string street_display( Map_Streets_DataSet.StreetsRow thisStreet )
        {
            // Start the return 
            StringBuilder html = new StringBuilder(5000);

            // If there is a start and end streets, start with that
            if (( !thisStreet.IsStartAddressNull() ) && ( thisStreet.StartAddress > 0 ) && ( !thisStreet.IsEndAddressNull() ) && ( thisStreet.EndAddress > 0 ))
            {
                // Add this to the string
                html.Append( thisStreet.StartAddress + " - " + thisStreet.EndAddress );
            }

            // Add segment info if there is some
            if (( !thisStreet.IsSegmentDescriptionNull() ) && ( thisStreet.SegmentDescription.Length > 0 ))
            {
                // If there is already start and end info, add a comma
                if ( html.Length > 0 )
                    html.Append( ", " );

                // Add this segment info
                html.Append( thisStreet.SegmentDescription );
            }

            // Return this built string
            return html.ToString();
        }
    }
}
