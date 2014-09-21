#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Settings;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer displays the message (and links) in the case the result set is empty.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class No_Results_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("No_Results_ResultsWriter.Add_HTML", "Adding no result text");
            }

            Literal thisLiteral = new Literal();

            StringBuilder noResultsTextBuilder = new StringBuilder();

         //   noResultsTextBuilder.Append("<div class=\"SobekHomeText\">");
            noResultsTextBuilder.AppendLine("<span class=\"SobekNoResultsText\"><br />Your search returned no results.<br /><br /></span>");

            // Get the list of search terms
            string terms = CurrentMode.Search_String.Replace(",", " ").Trim();

            // Try to search out into the Union catalog
            int union_catalog_matches = 0;
            try
            {
                // the html retrieved from the page
                String strResult;
                WebRequest objRequest = WebRequest.Create( SobekCM_Library_Settings.Mango_Union_Search_Base_URL + "&term=" + terms );
                objRequest.Timeout = 2000;
                WebResponse objResponse = objRequest.GetResponse();

                // the using keyword will automatically dispose the object once complete
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd().Trim();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                if (strResult.Length > 0)
                {
                    bool isNumber = strResult.All(Char.IsNumber);
                    if (isNumber)
                    {
                        union_catalog_matches = Convert.ToInt32(strResult);
                    }
                }
            }
            catch (Exception)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("No_Results_ResultsWriter.Add_HTML", "Exception caught while querying Mango state union catalog", Custom_Trace_Type_Enum.Error);
            }

            string union_library_link = "http://uf.catalog.fcla.edu/uf.jsp";
            if ((union_catalog_matches > 0) || ((CurrentMode.Aggregation.Length > 0) && (CurrentMode.Aggregation.ToUpper() != "ALL") && (Results_Statistics.All_Collections_Items > 0) && (CurrentMode.Default_Aggregation == "all" )))
            {
                noResultsTextBuilder.AppendLine("The following matches were found:<br /><br />");

                if ((CurrentMode.Aggregation.Length > 0) && (CurrentMode.Aggregation.ToUpper() != "ALL") && (Results_Statistics.All_Collections_Items > 0) && ( CurrentMode.Default_Aggregation == "all" ))
                {
                    string aggregation = CurrentMode.Aggregation;
                    CurrentMode.Aggregation = String.Empty;
                    if ( Results_Statistics.All_Collections_Items > 1 )
                        noResultsTextBuilder.AppendLine("<a href=\"" + CurrentMode.Redirect_URL() + "\">" + number_to_string(Results_Statistics.All_Collections_Items) + " found in the " + CurrentMode.SobekCM_Instance_Name + "</a><br /><br />");
                    else
                        noResultsTextBuilder.AppendLine("<a href=\"" + CurrentMode.Redirect_URL() + "\">" + Results_Statistics.All_Collections_Items + " found in the " + CurrentMode.SobekCM_Instance_Name + "</a><br /><br />");

                    CurrentMode.Aggregation = aggregation;
                }

                if (union_catalog_matches > 0)
                {
                    union_library_link = union_library_link + "?st=" + HttpUtility.HtmlEncode( terms ) + "&ix=kw";
                    if ( union_catalog_matches > 1 )
                        noResultsTextBuilder.AppendLine("<a href=\"" + union_library_link + "\" target=\"_BLANK\">" + number_to_string(union_catalog_matches) + " found in the University of Florida Library Catalog</a><br /><br />");
                    else
                        noResultsTextBuilder.AppendLine("<a href=\"" + union_library_link + "\" target=\"_BLANK\">One found in the University of Florida Library Catalog</a><br /><br />");
                    }

                noResultsTextBuilder.AppendLine("<br />");
            }

            noResultsTextBuilder.AppendLine("Consider searching one of the following:<br /><br />");

            noResultsTextBuilder.AppendLine("Online Resource: <a href=\"http://scholar.google.com\" target=\"_BLANK\">Google Scholar</a> or <a href=\"http://books.google.com\" target=\"_BLANK\">Google Books</a><br />");
            noResultsTextBuilder.AppendLine("Physical Holdings: the <a href=\"" + union_library_link + "\" target=\"_BLANK\">Library Catalog</a> or <a href=\"http://www.worldcat.org\" target=\"_BLANK\">Worldcat</a><br />");
            noResultsTextBuilder.AppendLine("  <br /><br /><br /><br />");
            noResultsTextBuilder.AppendLine("</td></tr></table>");
       //   noResultsTextBuilder.Append("</div>");

            noResultsTextBuilder.AppendLine();
            noResultsTextBuilder.AppendLine("<!-- place holder for the load() event in the body -->");
            noResultsTextBuilder.AppendLine("<script type=\"text/javascript\"> ");
            noResultsTextBuilder.AppendLine("  //<![CDATA[");
            noResultsTextBuilder.AppendLine("    function load() { } ");
            noResultsTextBuilder.AppendLine("  //]]>");
            noResultsTextBuilder.AppendLine("</script>");
            noResultsTextBuilder.AppendLine();

            thisLiteral.Text = noResultsTextBuilder.ToString();
            MainPlaceHolder.Controls.Add(thisLiteral);
        }

        private static string number_to_string(int Number)
        {
            switch (Number)
            {
                case 1: return "One";
                case 2: return "Two";
                case 3: return "Three";
                case 4: return "Four";
                case 5: return "Five";
                case 6: return "Six";
                case 7: return "Seven";
                case 8: return "Eight";
                case 9: return "Nine";
                case 10: return "Ten";
                case 11: return "Eleven";
                case 12: return "Twelve";
                default: return Number.ToString();

            }
        }
    }
}
