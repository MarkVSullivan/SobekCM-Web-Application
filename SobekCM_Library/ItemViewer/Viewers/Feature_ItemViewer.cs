#region Using directives

using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Items.Authority;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the list of features (from the authority database or metadata) associated with a digital resource. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
	public class Feature_ItemViewer : abstractItemViewer
	{
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Features"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Features; }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 700 </value>
		public override int Viewer_Width
		{
			get
			{
				return 700;
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
                Tracer.Add_Trace("Feature_ItemViewer.Write_Main_Viewer_Section", "");
            }

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Start the citation table
            Output.WriteLine("\t\t<!-- FEATURE VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>Index of Features</b></span></td></tr>" );
            Output.WriteLine("\t\t<tr><td class=\"SobekDocumentDisplay\">");
            Output.WriteLine("\t\t\t<div class=\"SobekCitation\">");

			// Get the list of streets from the database
            //Map_Features_DataSet features = SobekCM_Database.Get_All_Features_By_Item( CurrentItem.Web.ItemID, Tracer );
            //Create_Feature_Index( Output, features );

			// Finish the citation table
			Output.WriteLine( "\t\t\t</div>"  );
            Output.WriteLine("\t\t</td>" );
            Output.WriteLine("\t\t<!-- END FEATURE VIEWER OUTPUT -->" );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
		}

        /// <summary> Build the HTML for the feature index for this item </summary>
        /// <param name="Output"> HTML output stream </param>
        /// <param name="Features"> Dataset containig all of the features linked to this item </param>
        protected internal void Create_Feature_Index( TextWriter Output, Map_Features_DataSet Features)
		{
            if (Features == null)
            {
                Output.WriteLine("<br />");
                Output.WriteLine("<center><b>UNABLE TO LOAD FEATURES FROM DATABASE</b></center>");
                Output.WriteLine("<br />");
                CurrentMode.Mode = Display_Mode_Enum.Contact;
                Output.WriteLine("<center>Click <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">here</a> to report this issue.</center>");
                Output.WriteLine("<br />");
                CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                return;
            }

				string currentView = CurrentMode.ViewerCode;

				// This will be presented in a table, so start the table
				Output.WriteLine( "<table width=\"100%\">");

				// Determine (roughly) how many rows for each side
				// of the column
				int rows_per_column = Features.Features.Count / 2;
				if (( Features.Features.Count % 2 ) > 0 )
					rows_per_column++;

				// Start the large table for each section
                Output.WriteLine("\t<tr>");
                Output.WriteLine("\t\t<td width=\"46%\" valign=\"top\">");

				// Create the first column of feature information
				Insert_One_Feature_Column( Output, Features, 0, rows_per_column );			

				// Move to second column of large table, after making a small margin column
                Output.WriteLine("\t\t</td>");
                Output.WriteLine("\t\t<td width=\"8%\"></td> <!-- Spacer Column -->");
                Output.WriteLine("\t\t<td width=\"46%\" valign=\"top\">");

				// Create the second column of feature information
                Insert_One_Feature_Column(Output, Features, rows_per_column, Features.Features.Count);			

				// Finish off the large table
                Output.WriteLine("\t\t</td>\n\t</tr>\n</table>");
                Output.WriteLine("<!-- End output from GEMS_Map_Object.Feature_Index -->");

				CurrentMode.ViewerCode = currentView;
		}

        /// <summary> Insert the HTML for a single feature column into the Stringbuilder </summary>
        /// <param name="Output"> Output stream to write the HTML to </param>
        /// <param name="Features"> Dataset containig all of the features linked to this item </param>
        /// <param name="StartRow"> Row of the set of features to begin on </param>
        /// <param name="EndRow"> Last row in the set of features </param>
        protected internal void Insert_One_Feature_Column( TextWriter Output, Map_Features_DataSet Features, int StartRow, int EndRow )
		{
			// Declare some variables for looping
			string lastFeatureName = "%";

		    // Start the table for this row
            Output.WriteLine("\t\t\t<table width=\"100%\"> <!-- Table to display a single column of feature information -->");

			// Now, loop through all the results
		    int i = StartRow;
			while ( i < EndRow )
			{
			    // Get this feature
			    Map_Features_DataSet.FeaturesRow thisFeature = Features.Features[ i++ ];

			    // Only display this if it is not null
			    if ((thisFeature.IssorterNull()) || (thisFeature.sorter.Trim().Length <= 0)) continue;

			    // If this feature name starts with a new letter, add the letter now
			    if ( thisFeature.sorter.Trim()[0] != lastFeatureName[0] )
			    {
			        // Add a row for the letter
                    Output.WriteLine("\t\t\t\t<tr> <td colspan=\"2\" class=\"bigletter\" align=\"center\">" + (thisFeature.sorter.Trim())[0] + "</td></tr>");
			        lastFeatureName = thisFeature.sorter.Trim();
			    }

			    // Start this row
                Output.WriteLine("\t\t\t\t<tr class=\"index\">");

			    // Create the string to display
			    string display;
			    if (( !thisFeature.IsCorporateNameNull() ) && ( thisFeature.CorporateName.Length > 0 ))
			    {
			        // Is there a building use for this as well
			        if (( !thisFeature.IsFeatureNameNull() ) && ( thisFeature.FeatureName.Trim().Length > 0 ))
			            display = thisFeature.CorporateName + ", " + thisFeature.FeatureName;
			        else
			            display = thisFeature.CorporateName;
			    }
			    else
			    {
			        // Add location desc, if there is one
			        if (( !thisFeature.IsLocationDescNull() ) && ( thisFeature.LocationDesc.Length > 0 ))
			            display = thisFeature.FeatureName + " ( " + thisFeature.LocationDesc + " )";
			        else
			            display = thisFeature.FeatureName;
			    }

			    // Add the name
                Output.WriteLine("\t\t\t\t\t<td>" + display + "</td>");

			    // Add the link to the sheet
			    CurrentMode.ViewerCode = thisFeature["PageSequence"].ToString();
                Output.WriteLine("\t\t\t\t\t<td><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">" + thisFeature.PageName.Replace("Sheet", "").Trim() + "</a></td>");			

			    // End this row
                Output.WriteLine("\t\t\t\t</tr>");
			}

		    // End this table
            Output.WriteLine("\t\t\t</table>");
		}
	}
}
