#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SobekCM.Library.Results;
using SolrNet;
using SolrNet.Commands.Parameters;

#endregion

namespace SobekCM.Library.Solr
{
    /// <summary> Static class performs a search against a set of documents within a Solr/Lucene index </summary>
    [Serializable]
    public class Solr_Documents_Searcher
    {
        #region Static method to query the Solr/Lucene for a collection of results

        /// <summary> Perform an search for documents with matching parameters </summary>
        /// <param name="AggregationCode"> Aggregation code within which to search </param>
        /// <param name="QueryString"> Quert string for the actual search to perform aggainst the Solr/Lucene engine </param>
        /// <param name="resultsPerPage"> Number of results to display per a "page" of results </param>
        /// <param name="Page_Number"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Sort"> Sort to apply before returning the results of the search </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Complete_Result_Set_Info"> [OUT] Information about the entire set of results </param>
        /// <param name="Paged_Results"> [OUT] List of search results for the requested page of results </param>
        /// <returns> Page search result object with all relevant result information </returns>
        public static bool Search(string AggregationCode, string QueryString, int resultsPerPage, int Page_Number, ushort Sort, Custom_Tracer Tracer, out Search_Results_Statistics Complete_Result_Set_Info, out List<iSearch_Title_Result> Paged_Results)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Solr_Documents_Searcher.Search", String.Empty);
            }

            // Set output initially to null
            Paged_Results = new List<iSearch_Title_Result>();
            Complete_Result_Set_Info = null;

            try
            {
                // Ensure page is not erroneously set to zero or negative
                if (Page_Number <= 0)
                    Page_Number = 1;

                // Create the solr worker to query the document index
                var solrWorker = Solr_Operations_Cache<Solr_Document_Result>.GetSolrOperations(SobekCM_Library_Settings.Document_Solr_Index_URL);

                // Create the query options
                QueryOptions options = new QueryOptions
                                           {
                                               Rows = resultsPerPage,
                                               Start = (Page_Number - 1) * resultsPerPage,
                                               Fields = new[] { "did", "score","url","aleph","donor","edition","format","holdinglocation","sourceinstitution","maintitle","materialtype","oclc","pubdate_display","author_display","publisher_display","mainthumbnail"  },
                                               Highlight = new HighlightingParameters { Fields = new[] { "fulltext" }, },
                                               ExtraParams = new Dictionary<string, string> { { "hl.useFastVectorHighlighter", "true" } } 
                                           };

                // Set the sort value
                if (Sort != 0)
                {
                    options.OrderBy.Clear();
                    switch (Sort)
                    {
                        case 1:
                            options.OrderBy.Add(new SortOrder("maintitle_sort"));
                            break;

                        case 2:
                            options.OrderBy.Add(new SortOrder("bibid", Order.ASC) );
                            break;

                        case 3:
                            options.OrderBy.Add(new SortOrder("bibid", Order.DESC));
                            break;

                        case 10:
                            options.OrderBy.Add(new SortOrder("pubdate", Order.ASC));
                            break;

                        case 11:
                            options.OrderBy.Add(new SortOrder("pubdate", Order.DESC));
                            break;

                    }
                }

                // If there was an aggregation code included, put that at the beginning of the search
                if ((AggregationCode.Length > 0) && (AggregationCode.ToUpper() != "ALL"))
                {
                    QueryString = "(aggregation_code:" + AggregationCode.ToUpper() + ")AND(" + QueryString + ")";
                }

                // Perform this search
                ISolrQueryResults<Solr_Document_Result> results = solrWorker.Query(QueryString, options);

                // Create the search statistcs
                Complete_Result_Set_Info = new Search_Results_Statistics
                                               {
                                                   Total_Titles = results.NumFound,
                                                   Total_Items = results.NumFound,
                                                   QueryTime = results.Header.QTime
                                               };

                // Pass all the results into the List and add the highlighted text to each result as well
                foreach (Solr_Document_Result thisResult in results)
                {
                    // Add the highlight snipper
                    if ((results.Highlights.ContainsKey(thisResult.DID)) && (results.Highlights[thisResult.DID].Count > 0) && (results.Highlights[thisResult.DID].ElementAt(0).Value.Count > 0))
                    {
                        thisResult.Snippet = results.Highlights[thisResult.DID].ElementAt(0).Value.ElementAt(0);
                    }

                    // Add this results
                    Paged_Results.Add(thisResult);
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        #endregion

        #region Method to split the complex search term string into a collection of search terms and fields

        /// <summary> Method splits the search string and field string into seperate collections of strings </summary>
        /// <param name="term_string"> String containing all of the search terms</param>
        /// <param name="field"> String containing all of the search field codes</param>
        /// <param name="terms_builder"> Collection of seperate search terms</param>
        /// <param name="fields_builder"> Collection of seperate saerch field codes</param>
        /// <remarks> This code is here to handle quotation marks, so quoted terms are not erroneously split </remarks>
        public static void Split_Multi_Terms(string term_string, string field, List<string> terms_builder, List<string> fields_builder)
        {
            string termsStr = term_string + " ";
            int first_term = terms_builder.Count;
            //		int current_index = 0;


            int last_index = 0;
            int quote_index = termsStr.IndexOf("\"", last_index);
            int space_index = termsStr.IndexOf(" ", last_index);
            while ((last_index < termsStr.Length) && ((quote_index >= 0) || (space_index >= 0)))
            {
                // If there is a quote, and it is before the space, find the end quote
                string thisTerm;
                if ((quote_index >= 0) && (quote_index < space_index))
                {
                    last_index = quote_index;
                    quote_index = termsStr.IndexOf("\"", last_index + 1);
                    if (quote_index < 0)
                        quote_index = termsStr.Length - 1;
                    thisTerm = termsStr.Substring(last_index, quote_index - last_index + 1).Trim().Replace(" ", "+");
                    last_index = quote_index + 1;
                    if (thisTerm.Length > 0)
                    {
                        terms_builder.Add(thisTerm);
                    }
                }
                else
                {
                    // Divide at the space then
                    // Was there a first term to include?
                    thisTerm = termsStr.Substring(last_index, space_index - last_index).Trim();
                    last_index = space_index;
                    // Only add this if there is a character or number
                    bool isValid = thisTerm.Any(Char.IsLetterOrDigit);
                    if (isValid)
                    {
                        terms_builder.Add(thisTerm);
                    }
                }

                // Find the next spaces or quotes
                if (last_index < termsStr.Length)
                {
                    quote_index = termsStr.IndexOf("\"", last_index + 1);
                    space_index = termsStr.IndexOf(" ", last_index + 1);
                }
            }


            // If there was no fields provided, search anywhere
            if (field.Trim().Length == 0)
            {
                field = "+ZZ";
            }

            // Now, build the fields and string
            for (int i = first_term; i < terms_builder.Count; i++)
            {
                if ((field[0] != '-') && (field[0] != '=') && (field[0] != '+'))
                {
                    fields_builder.Add("+" + field);
                }
                else
                {
                    fields_builder.Add(field);
                }

                // If this should have been NOT, utilize that
                if (terms_builder[i][0] == '-')
                {
                    terms_builder[i] = terms_builder[i].Substring(1);
                    if ((fields_builder[i][0] == '-') || (fields_builder[i][0] == '=') || (fields_builder[i][0] == '+'))
                    {
                        switch (fields_builder[i][0])
                        {
                            case '-':
                                fields_builder[i] = "+" + fields_builder[i].Substring(1);
                                break;
                            case '+':
                                fields_builder[i] = "-" + fields_builder[i].Substring(1);
                                break;
                            case '=':
                                fields_builder[i] = "-" + fields_builder[i].Substring(1);
                                break;
                        }
                    }
                    else
                    {
                        fields_builder[i] = "-" + field;
                    }
                }

                // If this should have been OR, utilize that
                if (terms_builder[i][0] == '=')
                {
                    terms_builder[i] = terms_builder[i].Substring(1);
                    if ((fields_builder[i][0] == '-') || (fields_builder[i][0] == '=') || (fields_builder[i][0] == '+'))
                    {
                        switch (fields_builder[i][0])
                        {
                            case '-':
                                fields_builder[i] = "-" + fields_builder[i].Substring(1);
                                break;
                            case '+':
                                fields_builder[i] = "=" + fields_builder[i].Substring(1);
                                break;
                            case '=':
                                fields_builder[i] = "=" + fields_builder[i].Substring(1);
                                break;
                        }
                    }
                    else
                    {
                        fields_builder[i] = "=" + field;
                    }
                }
            }
        }

        #endregion
    }
}
