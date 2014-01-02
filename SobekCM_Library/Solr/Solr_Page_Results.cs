#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SobekCM.Library.Settings;
using SolrNet;
using SolrNet.Commands.Parameters;

#endregion

namespace SobekCM.Library.Solr
{
    /// <summary> Stores a group of page results from an in-document search against a Solr full text index </summary>
    [Serializable]
    public class Solr_Page_Results
    {
        private readonly List<Solr_Page_Result> results;

        /// <summary> Constructor for a new instance of the Solr_Page_Results class </summary>
        public Solr_Page_Results()
        {
            results = new List<Solr_Page_Result>();
            Page_Number = -1;
            TotalResults = 0;
            QueryTime = -1;
            Sort_By_Score = true;
        }

        /// <summary> Gets the collection of single page search results associated with this search </summary>
        public ReadOnlyCollection<Solr_Page_Result> Results
        {
            get
            {
                return new ReadOnlyCollection<Solr_Page_Result>(results);
            }
        }

        /// <summary> Time, in millseconds, required for this query on the Solr search engine </summary>
        public int QueryTime { get; internal set; }

        /// <summary> Number of total results in the complete result set </summary>
        public int TotalResults { get; internal set; }

        /// <summary> Actual query string conducted by the SobekCM user and then processed against the Solr search engine </summary>
        public string Query { get; internal set; }

        /// <summary> Flag indicates if these search results were sorted by score, rather than the default sorting by page order </summary>
        public bool Sort_By_Score { get; internal set; }

        /// <summary> Page number of these results within the complete results </summary>
        /// <remarks> The Solr/Lucene searches only return a "page" of results at a time.  The page number is a one-based indexed, with the first page being number 1 (not zero).</remarks>
        public int Page_Number { get; internal set; }

        /// <summary> Add the next single page result from an in-document search against a Solr full-text index </summary>
        /// <param name="Result"></param>
        internal void Add_Result(Solr_Page_Result Result)
        {
            results.Add(Result);
        }

        /// <summary> Perform an in-document search for pages with matching full-text </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for the item to search </param>
        /// <param name="VID"> Volume identifier for the item to search </param>
        /// <param name="Search_Terms"> Terms to search for within the page text </param>
        /// <param name="ResultsPerPage"> Number of results to display per a "page" of results </param>
        /// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Sort_By_Score"> Flag indicates whether to sort the results by relevancy score, rather than the default page order </param>
        /// <returns> Page search result object with all relevant result information </returns>
        public static Solr_Page_Results Search(string BibID, string VID, List<string> Search_Terms, int ResultsPerPage, int ResultsPage, bool Sort_By_Score)
        {
            // Ensure page is not erroneously set to zero or negative
            if (ResultsPage <= 0)
                ResultsPage = 1;

            // Create the solr worker to query the page index
            var solrWorker = Solr_Operations_Cache<Solr_Page_Result>.GetSolrOperations( SobekCM_Library_Settings.Page_Solr_Index_URL);

            // Create the query options
            QueryOptions options = new QueryOptions
            {
                Rows = ResultsPerPage,
                Start = (ResultsPage - 1) * ResultsPerPage,
                Fields = new [] { "pageid", "pagename", "pageorder", "score", "thumbnail" },
                Highlight = new HighlightingParameters { Fields = new[] { "pagetext" }, },
                ExtraParams = new Dictionary<string, string> { { "hl.useFastVectorHighlighter", "true" } } 
            };
            
            // If this is not the default Solr sort (by score) request sort by the page order
            if (!Sort_By_Score)
                options.OrderBy = new[] { new SortOrder("pageorder", Order.ASC) };

            // Build the query string
            StringBuilder queryStringBuilder = new StringBuilder("(bibid:" + BibID + ")AND(vid:" + VID + ")AND(");
            bool first_value = true;
            foreach (string searchTerm in Search_Terms)
            {
                if (searchTerm.Length > 1)
                {
                    // Skip any AND NOT for now
                    if (searchTerm[0] != '-')
                    {
                        // Find the joiner
                        if (first_value)
                        {
                            if (searchTerm.IndexOf(" ") > 0)
                            {
                                if ((searchTerm[0] == '+') || (searchTerm[0] == '=') || (searchTerm[0] == '-'))
                                {
                                    queryStringBuilder.Append("(pagetext:\"" + searchTerm.Substring(1).Replace(":","") + "\")" );
                                }
                                else
                                {
                                    queryStringBuilder.Append("(pagetext:\"" + searchTerm.Replace(":", "") + "\")");
                                }
                            }
                            else
                            {
                                if ((searchTerm[0] == '+') || (searchTerm[0] == '=') || (searchTerm[0] == '-'))
                                {
                                    queryStringBuilder.Append("(pagetext:" + searchTerm.Substring(1).Replace(":", "") + ")");
                                }
                                else
                                {
                                    queryStringBuilder.Append("(pagetext:" + searchTerm.Replace(":", "") + ")");
                                }
                            }
                            first_value = false;
                        }
                        else
                        {
                            if ((searchTerm[0] == '+') || (searchTerm[0] == '=') || (searchTerm[0] == '-'))
                            {
                                queryStringBuilder.Append(searchTerm[0] == '=' ? " OR " : " AND ");

                                if (searchTerm.IndexOf(" ") > 0)
                                {
                                    queryStringBuilder.Append("(pagetext:\"" + searchTerm.Substring(1).Replace(":", "") + "\")");
                                }
                                else
                                {
                                    queryStringBuilder.Append("(pagetext:" + searchTerm.Substring(1).Replace(":", "") + ")");
                                }
                            }
                            else
                            {
                                if (searchTerm.IndexOf(" ") > 0)
                                {
                                    queryStringBuilder.Append(" AND (pagetext:\"" + searchTerm.Replace(":", "") + "\")");
                                }
                                else
                                {
                                    queryStringBuilder.Append(" AND (pagetext:" + searchTerm.Replace(":", "") + ")");
                                }
                            }
                        }
                    }
                }
            }
            queryStringBuilder.Append(")");


            // Perform this search
            SolrQueryResults<Solr_Page_Result> results = solrWorker.Query(queryStringBuilder.ToString(), options);

            // Create the results object to pass back out
            var searchResults = new Solr_Page_Results
            {
                QueryTime = results.Header.QTime,
                TotalResults = results.NumFound,
                Query = queryStringBuilder.ToString(),
                Sort_By_Score = Sort_By_Score,
                Page_Number = ResultsPage
            };

            // Pass all the results into the List and add the highlighted text to each result as well
            foreach (Solr_Page_Result thisResult in results)
            {
                // Add the highlight snipper
                if ((results.Highlights.ContainsKey(thisResult.PageID)) && (results.Highlights[thisResult.PageID].Count > 0) && (results.Highlights[thisResult.PageID].ElementAt(0).Value.Count > 0))
                {
                    thisResult.Snippet = results.Highlights[thisResult.PageID].ElementAt(0).Value.ElementAt(0);
                }

                // Add this results
                searchResults.Add_Result( thisResult );
            }

            return searchResults;
        }
    }
}
