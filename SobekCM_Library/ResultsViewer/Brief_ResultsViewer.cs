#region Using directives

using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.Search;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer shows the thumbnail and a small citation block for each item in the result set.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Brief_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Brief_ResultsViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Brief_ResultsViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Do nothing
        }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Brief_ResultsWriter.Add_HTML", "Rendering results in brief view");
            }

            // If results are null, or no results, return empty string
            if ((RequestSpecificValues.Paged_Results == null) || (RequestSpecificValues.Results_Statistics == null) || (RequestSpecificValues.Results_Statistics.Total_Items <= 0))
                return;

            const string VARIES_STRING = "<span style=\"color:Gray\">( varies )</span>";

            // Get the text search redirect stem and (writer-adjusted) base url 
            string textRedirectStem = Text_Redirect_Stem;
            string base_url = RequestSpecificValues.Current_Mode.Base_URL;
            if (RequestSpecificValues.Current_Mode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = RequestSpecificValues.Current_Mode.Base_URL + "l/";

            // Start the results
            StringBuilder resultsBldr = new StringBuilder(2000);
            resultsBldr.AppendLine("<section class=\"sbkBrv_Results\">");

            // Set the counter for these results from the page 
            int current_page = RequestSpecificValues.Current_Mode.Page.HasValue ? RequestSpecificValues.Current_Mode.Page.Value : 1;
            int result_counter = ((current_page - 1) * Results_Per_Page) + 1;

            // Step through all the results
            int current_row = 0;
            foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
            {
	            bool multiple_title = titleResult.Item_Count > 1;

	            // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);

                // Determine the internal link to the first (possibly only) item
                string internal_link = base_url + titleResult.BibID + "/" + firstItemResult.VID + textRedirectStem;

                // For browses, just point to the title
				if (RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Aggregation) // browse info only
                    internal_link = base_url + titleResult.BibID + textRedirectStem;

                // Start this row
                string title = firstItemResult.Title.Replace("<", "&lt;").Replace(">", "&gt;");
                if (multiple_title)
                {
                    title = titleResult.GroupTitle.Replace("<", "&lt;").Replace(">", "&gt;");
                    resultsBldr.AppendLine("\t<section class=\"sbkBrv_SingleResult\">");

                }
                else
                    resultsBldr.AppendLine("\t<section class=\"sbkBrv_SingleResult\" onclick=\"window.location.href='" + internal_link + "';\" >");


                // Add the counter as the first column
                resultsBldr.AppendLine("\t\t<div class=\"sbkBrv_SingleResultNum\">" + result_counter + "</div>");
                resultsBldr.Append("\t\t<div class=\"sbkBrv_SingleResultThumb\">");
                //// Is this restricted?
                bool restricted_by_ip = false;
                if ((titleResult.Item_Count == 1) && (firstItemResult.IP_Restriction_Mask > 0))
                {
                    int comparison = firstItemResult.IP_Restriction_Mask & CurrentUserMask;
                    if (comparison == 0)
                    {
                        restricted_by_ip = true;
                    }
                }

                // Calculate the thumbnail
                string thumb = titleResult.BibID.Substring(0, 2) + "/" + titleResult.BibID.Substring(2, 2) + "/" +titleResult.BibID.Substring(4, 2) + "/" + titleResult.BibID.Substring(6, 2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/" + (firstItemResult.MainThumbnail).Replace("\\", "/").Replace("//", "/");

                // Draw the thumbnail 
                if ((thumb.ToUpper().IndexOf(".JPG") < 0) && (thumb.ToUpper().IndexOf(".GIF") < 0))
                {
                    resultsBldr.AppendLine("<a href=\"" + internal_link + "\"><img src=\"" + Static_Resources.Nothumb_Jpg + "\" border=\"0px\" class=\"resultsThumbnail\" alt=\"MISSING THUMBNAIL\" /></a></div>");
                }
                else
                {
                    resultsBldr.AppendLine("<a href=\"" + internal_link + "\"><img src=\"" +UI_ApplicationCache_Gateway.Settings.Image_URL + thumb + "\" class=\"resultsThumbnail\" alt=\"" + title.Replace("\"","") + "\" /></a></div>");
                }


                resultsBldr.AppendLine("\t\t<div class=\"sbkBrv_SingleResultDesc\">");

                // If this was access restricted, add that
                if (restricted_by_ip)
                {
                    resultsBldr.AppendLine("\t\t\t<span class=\"RestrictedItemText\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Access Restricted", RequestSpecificValues.Current_Mode.Language) + "</span>");
                }

                if (multiple_title)
                {
                    resultsBldr.AppendLine("\t\t\t<span class=\"briefResultsTitle\"><a href=\"" + internal_link + "\">" + titleResult.GroupTitle.Replace("<", "&lt;").Replace(">", "&gt;") + "</a></span>");
                }
                else
                {
                    resultsBldr.AppendLine(
                        "\t\t\t<span class=\"briefResultsTitle\"><a href=\"" +
                        internal_link + "\">" + firstItemResult.Title.Replace("<", "&lt;").Replace(">", "&gt;") +
                        "</a></span>");
                }

                // Add each element to this table
                resultsBldr.AppendLine("\t\t\t<dl class=\"sbkBrv_SingleResultDescList\">");



                if ((titleResult.Primary_Identifier_Type.Length > 0) && (titleResult.Primary_Identifier.Length > 0))
                {
                    resultsBldr.AppendLine("\t\t\t\t<dt>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(titleResult.Primary_Identifier_Type, RequestSpecificValues.Current_Mode.Language) + ":</dt><dd>" + titleResult.Primary_Identifier + "</dd>");
                }

                if ((RequestSpecificValues.Current_User != null ) && ( RequestSpecificValues.Current_User.LoggedOn ) && ( RequestSpecificValues.Current_User.Is_Internal_User ))
                {
                    resultsBldr.AppendLine("\t\t\t\t<dt>BibID:</dt><dd>" + titleResult.BibID + "</dd>");

                    if (titleResult.OPAC_Number > 1)
                    {
                        resultsBldr.AppendLine("\t\t\t\t<dt>OPAC:</dt><dd>" +titleResult.OPAC_Number + "</dd>");
                    }

                    if (titleResult.OCLC_Number > 1)
                    {
                        resultsBldr.AppendLine("\t\t\t\t<dt>OCLC:</dt><dd>" + titleResult.OCLC_Number + "</dd>");
                    }
                }

                for (int i = 0; i < RequestSpecificValues.Results_Statistics.Metadata_Labels.Count; i++)
				{
                    string field = RequestSpecificValues.Results_Statistics.Metadata_Labels[i];

                    // Somehow the metadata for this item did not fully save in the database.  Break out, rather than
                    // throw the exception
                    if ((titleResult.Metadata_Display_Values == null) || (titleResult.Metadata_Display_Values.Length <= i))
                        break;

					string value = titleResult.Metadata_Display_Values[i];
					Metadata_Search_Field thisField = UI_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_Name(field);
					string display_field = string.Empty;
					if ( thisField != null )
						display_field = thisField.Display_Term;
					if (display_field.Length == 0)
						display_field = field.Replace("_", " ");

					if (value == "*")
					{
						resultsBldr.AppendLine("\t\t\t\t<dt>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(display_field, RequestSpecificValues.Current_Mode.Language) + ":</dt><dd>" + HttpUtility.HtmlDecode(VARIES_STRING) + "</dd>");
					}
					else if ( value.Trim().Length > 0 )
					{
						if (value.IndexOf("|") > 0)
						{
							bool value_found = false;
							string[] value_split = value.Split("|".ToCharArray());

							foreach (string thisValue in value_split)
							{
								if (thisValue.Trim().Trim().Length > 0)
								{
									if (!value_found)
									{
										resultsBldr.Append("\t\t\t\t<dt>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(display_field, RequestSpecificValues.Current_Mode.Language) + ":</dt>");
										value_found = true;
									}
									resultsBldr.Append("<dd>" + HttpUtility.HtmlDecode(thisValue) + "</dd>");
								}
							}

                            if (value_found)
                            {
                                resultsBldr.AppendLine();
                            }
						}
						else
						{
							resultsBldr.AppendLine("\t\t\t\t<dt>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(display_field, RequestSpecificValues.Current_Mode.Language) + ":</dt><dd>" + HttpUtility.HtmlDecode(value) + "</dd>");
						}
					}
				}

                resultsBldr.AppendLine("\t\t\t</dl>");

                if (titleResult.Snippet.Length > 0)
                {
                    resultsBldr.AppendLine("\t\t\t<div class=\"sbkBrv_SearchResultSnippet\">&ldquo;..." + titleResult.Snippet.Replace("<em>", "<span class=\"texthighlight\">").Replace ("</em>", "</span>") + "...&rdquo;</div>");
                }
                
                // Add children, if there are some
                if (multiple_title)
                {
                    // Add this to the place holder
                    Literal thisLiteral = new Literal
                                              { Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>") };
                    MainPlaceHolder.Controls.Add(thisLiteral);
                    resultsBldr.Remove(0, resultsBldr.Length);

                    Add_Issue_Tree(MainPlaceHolder, titleResult, current_row, textRedirectStem, base_url);
                }

                resultsBldr.AppendLine("\t\t</div>");

                resultsBldr.AppendLine("\t</section>");
                resultsBldr.AppendLine();

                // Increment the result counters
                result_counter++;
                current_row++;
            }

            // End this table
            resultsBldr.AppendLine("</section>");

            // Add this to the HTML page
            Literal mainLiteral = new Literal
                                      { Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>") };
            MainPlaceHolder.Controls.Add(mainLiteral);
        }
    }
}
