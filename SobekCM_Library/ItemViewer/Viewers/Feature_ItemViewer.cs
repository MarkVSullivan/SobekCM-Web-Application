#region Using directives

using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.Database;
using SobekCM.Library.Items.Authority;
using SobekCM.Library.Navigation;

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
        /// <value> This is a single page viewer, so this property always returns FALSE</value>
        public override bool Show_Page_Selector
        {
            get
            {
                return false;
            }
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Feature_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

			// Build the value
			StringBuilder builder = new StringBuilder(5000);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Start the citation table
            builder.AppendLine("\t\t<!-- FEATURE VIEWER OUTPUT -->" );
            builder.AppendLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>Index of Features</b></span></td></tr>" );
            builder.AppendLine("\t\t<tr><td class=\"SobekDocumentDisplay\">");
            builder.AppendLine("\t\t\t<div class=\"SobekCitation\">");

			// Get the list of streets from the database
			Map_Features_DataSet features = SobekCM_Database.Get_All_Features_By_Item( CurrentItem.SobekCM_Web.ItemID, Tracer );
			Create_Feature_Index( builder, features );

			// Finish the citation table
			builder.AppendLine( "\t\t\t</div>"  );
            builder.AppendLine("\t\t</td>" );
            builder.AppendLine("\t\t<!-- END FEATURE VIEWER OUTPUT -->" );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;

			// Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add( mainLiteral );
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
                Tracer.Add_Trace("Feature_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
		}

		/// <summary> Build the HTML for the feature index for this item </summary>
        /// <param name="html"> Stringbuilder to feed the HTML into </param>
        /// <param name="features"> Dataset containig all of the features linked to this item </param>
        protected internal void Create_Feature_Index(StringBuilder html, Map_Features_DataSet features)
		{
            if (features == null)
            {
                html.AppendLine("<br />");
                html.AppendLine("<center><b>UNABLE TO LOAD FEATURES FROM DATABASE</b></center>");
                html.AppendLine("<br />");
                CurrentMode.Mode = Display_Mode_Enum.Contact;
                html.AppendLine("<center>Click <a href=\"" + CurrentMode.Redirect_URL() + "\">here</a> to report this issue.</center>");
                html.AppendLine("<br />");
                CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                return;
            }

				string currentView = CurrentMode.ViewerCode;

				// This will be presented in a table, so start the table
				html.AppendLine( "<table width=\"100%\">");

				// Determine (roughly) how many rows for each side
				// of the column
				int rows_per_column = features.Features.Count / 2;
				if (( features.Features.Count % 2 ) > 0 )
					rows_per_column++;

				// Start the large table for each section
                html.AppendLine("\t<tr>");
                html.AppendLine("\t\t<td width=\"46%\" valign=\"top\">");

				// Create the first column of feature information
				Insert_One_Feature_Column( html, features, 0, rows_per_column );			

				// Move to second column of large table, after making a small margin column
                html.AppendLine("\t\t</td>");
                html.AppendLine("\t\t<td width=\"8%\"></td> <!-- Spacer Column -->");
                html.AppendLine("\t\t<td width=\"46%\" valign=\"top\">");

				// Create the second column of feature information
				Insert_One_Feature_Column( html, features, rows_per_column, features.Features.Count );			

				// Finish off the large table
                html.AppendLine("\t\t</td>\n\t</tr>\n</table>");
                html.AppendLine("<!-- End output from GEMS_Map_Object.Feature_Index -->");

				CurrentMode.ViewerCode = currentView;
		}

		/// <summary> Insert the HTML for a single feature column into the Stringbuilder </summary>
		/// <param name="html"> Stringbuilder to feed the HTML into </param>
        /// <param name="features"> Dataset containig all of the features linked to this item </param>
		/// <param name="startRow"> Row of the set of features to begin on </param>
		/// <param name="endRow"> Last row in the set of features </param>
		protected internal void Insert_One_Feature_Column( StringBuilder html, Map_Features_DataSet features, int startRow, int endRow )
		{
			// Declare some variables for looping
			string lastFeatureName = "%";

		    // Start the table for this row
            html.AppendLine("\t\t\t<table width=\"100%\"> <!-- Table to display a single column of feature information -->");

			// Now, loop through all the results
		    int i = startRow;
			while ( i < endRow )
			{
			    // Get this feature
			    Map_Features_DataSet.FeaturesRow thisFeature = features.Features[ i++ ];

			    // Only display this if it is not null
			    if ((thisFeature.IssorterNull()) || (thisFeature.sorter.Trim().Length <= 0)) continue;

			    // If this feature name starts with a new letter, add the letter now
			    if ( thisFeature.sorter.Trim()[0] != lastFeatureName[0] )
			    {
			        // Add a row for the letter
                    html.AppendLine("\t\t\t\t<tr> <td colspan=\"2\" class=\"bigletter\" align=\"center\">" + (thisFeature.sorter.Trim())[0] + "</td></tr>");
			        lastFeatureName = thisFeature.sorter.Trim();
			    }

			    // Start this row
                html.AppendLine("\t\t\t\t<tr class=\"index\">");

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
                html.AppendLine("\t\t\t\t\t<td>" + display + "</td>");

			    // Add the link to the sheet
			    CurrentMode.ViewerCode = thisFeature["PageSequence"].ToString();
                html.AppendLine("\t\t\t\t\t<td><a href=\"" + CurrentMode.Redirect_URL() + "\">" + thisFeature.PageName.Replace("Sheet", "").Trim() + "</a></td>");			

			    // End this row
                html.AppendLine("\t\t\t\t</tr>");
			}

		    // End this table
            html.AppendLine("\t\t\t</table>");
		}
	}
}
