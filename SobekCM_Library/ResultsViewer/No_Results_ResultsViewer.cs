#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer displays the message (and links) in the case the result set is empty.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class No_Results_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the No_Results_ResultsViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public No_Results_ResultsViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
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
                Tracer.Add_Trace("No_Results_ResultsWriter.Add_HTML", "Adding no result text");
            }

            // Get the no results text
            string noResultsText = Get_NoResults_Text();

            Literal thisLiteral = new Literal();

            // Get the list of search terms
            string terms = RequestSpecificValues.Current_Mode.Search_String.Replace(",", " ").Trim();

            // Try to search out into the Union catalog
            int union_catalog_matches = 0;
            string susMangoSearchQuery = String.Empty;
            if ((noResultsText.Contains("[%SusMangoSpanDisplay%]")) && (UI_ApplicationCache_Gateway.Settings.Florida != null ) && (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Florida.Mango_Union_Search_Base_URL)))
            {
                try
                {
                    // the html retrieved from the page
                    String strResult;
                    WebRequest objRequest = WebRequest.Create(UI_ApplicationCache_Gateway.Settings.Florida.Mango_Union_Search_Base_URL + "&term=" + terms);
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
            }

            // Show or hide the links
            if ((union_catalog_matches > 0) || ((RequestSpecificValues.Current_Mode.Aggregation.Length > 0) && (RequestSpecificValues.Current_Mode.Aggregation.ToUpper() != "ALL") && (RequestSpecificValues.Results_Statistics.All_Collections_Items > 0) && (RequestSpecificValues.Current_Mode.Default_Aggregation == "all")))
            {
                noResultsText = noResultsText.Replace("[%MatchesFoundDivDisplay%]", "block");

                if ((RequestSpecificValues.Current_Mode.Aggregation.Length > 0) && (RequestSpecificValues.Current_Mode.Aggregation.ToUpper() != "ALL") && (RequestSpecificValues.Results_Statistics.All_Collections_Items.HasValue) && (RequestSpecificValues.Results_Statistics.All_Collections_Items.Value > 0) && (RequestSpecificValues.Current_Mode.Default_Aggregation == "all"))
                {
                    string aggregation = RequestSpecificValues.Current_Mode.Aggregation;
                    RequestSpecificValues.Current_Mode.Aggregation = String.Empty;
                    string instance_search_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                    RequestSpecificValues.Current_Mode.Aggregation = aggregation;


                    noResultsText = noResultsText.Replace("[%WithinInstanceSpanDisplay%]", "inline-block").Replace("[%WithinInstanceUrl%]", instance_search_url).Replace("[%WithinInstanceCount%]", number_to_string(RequestSpecificValues.Results_Statistics.All_Collections_Items));
                }
                else
                {
                    noResultsText = noResultsText.Replace("[%WithinInstanceSpanDisplay%]", "none");
                }

                if (union_catalog_matches > 0)
                {
                    susMangoSearchQuery = "?st=" + HttpUtility.HtmlEncode(terms) + "&ix=kw";
                    noResultsText = noResultsText.Replace("[%SusMangoSpanDisplay%]", "inline-block").Replace("[%SusMangoSearchEnding%]", susMangoSearchQuery).Replace("[%SusMangoCount%]", number_to_string(union_catalog_matches));
                }
                else
                {
                    noResultsText = noResultsText.Replace("[%SusMangoSpanDisplay%]", "none").Replace("[%SusMangoSearchEnding%]", String.Empty);
                }
            }
            else
            {
                noResultsText = noResultsText.Replace("[%MatchesFoundDivDisplay%]", "none").Replace("[%SusMangoSearchEnding%]", String.Empty);
            }

            // Show the final data
            StringBuilder noResultsTextBuilder = new StringBuilder(noResultsText.Replace("[%SusMangoSearchEnding%]", String.Empty).Replace("[%BaseName%]", RequestSpecificValues.Current_Mode.Instance_Name));

            noResultsTextBuilder.AppendLine("</td></tr></table>");

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

        public static string Get_NoResults_Text()
        {
            string noResultsText = HttpContext.Current.Application["NORESULTS"] as string;
            if (String.IsNullOrEmpty(noResultsText))
            {
                try
                {
                    string file = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location, "webcontent", "noresults.html");
                    if (File.Exists(file))
                    {
                        noResultsText = File.ReadAllText(file);
                        HttpContext.Current.Application["NORESULTS"] = noResultsText;
                    }
                    else
                    {
                        noResultsText = "NOTPRESENT";
                        HttpContext.Current.Application["NORESULTS"] = "NOTPRESENT";
                    }
                }
                catch
                {
                    noResultsText = "NOTPRESENT";
                    HttpContext.Current.Application["NORESULTS"] = "NOTPRESENT";
                }
            }

            // Now, if still NULL, build it the way we used to
            if ((String.IsNullOrEmpty(noResultsText)) || (noResultsText == "NOTPRESENT"))
            {
                StringBuilder sampleFileContent = new StringBuilder();


                sampleFileContent.AppendLine("<span class=\"SobekNoResultsText\"><br />Your search returned no results.<br /><br /></span>");
                sampleFileContent.AppendLine("<div style=\"display:[%MatchesFoundDivDisplay%]\">");
                sampleFileContent.AppendLine("    The following matches were found:<br /><br />");
                sampleFileContent.AppendLine("      <span style=\"display:[%WithinInstanceSpanDisplay%]\"><a href=\"[%WithinInstanceUrl%]\">[%WithinInstanceCount%] found in [%BaseName%]</a><br /><br /></span>");
                sampleFileContent.AppendLine("      <span style=\"display:[%SusMangoSpanDisplay%]\"><a href=\"http://uf.catalog.fcla.edu/uf.jsp[%SusMangoSearchEnding%]\" target=\"_BLANK\">[%SusMangoCount%] found in the University of Florida Library Catalog</a><br /><br /></span>");
                sampleFileContent.AppendLine("</div>");

                sampleFileContent.AppendLine("Consider searching one of the following:<br /><br />");

                sampleFileContent.AppendLine("Online Resource: <a href=\"http://scholar.google.com\" target=\"_BLANK\">Google Scholar</a> or <a href=\"http://books.google.com\" target=\"_BLANK\">Google Books</a><br />");
                sampleFileContent.AppendLine("Physical Holdings: the <a href=\"http://uf.catalog.fcla.edu/uf.jsp[%SusMangoSearchEnding%]\" target=\"_BLANK\">Library Catalog</a> or <a href=\"http://www.worldcat.org\" target=\"_BLANK\">Worldcat</a><br />");
                sampleFileContent.AppendLine("  <br /><br /><br /><br />");

                string sampleBuild = sampleFileContent.ToString();
                HttpContext.Current.Application["NORESULTS"] = sampleBuild;

                noResultsText = sampleBuild;
            }

            return noResultsText;
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

        private static string number_to_string(int? Number)
        {
            if (!Number.HasValue)
                return "No";

            int value = Number.Value;
            switch (value)
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
