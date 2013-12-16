#region Using directives

using System;
using SolrNet.Attributes;

#endregion

namespace SobekCM.Library.Solr
{
    /// <summary> Stores the information relating to a single page result from an in-document search against a Solr index  </summary>
    /// <remarks> This is populated by the Solr query through SolrNet, using the SolrNet property attributes as guides.  The highlighted snippet
    /// containing the search term(s) for this page is also loaded into this object </remarks>
    [Serializable]
    public class Solr_Page_Result
    {
        private string pageid, pagename;
        private string snippet, thumbnail;

        /// <summary> Constructor for a new instancee of the Solr_Page_Result class </summary>
        public Solr_Page_Result()
        {
            PageOrder = -1;
        }

        /// <summary> Unique PageID for this single page result from a search within a document </summary>
        [SolrField("pageid")]
        public string PageID
        {
            get
            {
                return pageid ?? String.Empty;
            }
            internal set
            {
                pageid = value;
            }
        }

        /// <summary> Page order for this single page result from a search within a document </summary>
        [SolrField("pageorder")]
        public int PageOrder { get; internal set; }

        /// <summary> Name of this page for this single page result from a search within a document </summary>
        [SolrField("pagename")]
        public string PageName
        {
            get
            {
                return pagename ?? String.Empty;
            }
            internal set
            {
                pagename = value;
            }
        }

        /// <summary> Thumbnail image for this page results </summary>
        [SolrField("thumbnail")]
        public string Thumbnail
        {
            get
            {
                return thumbnail ?? String.Empty;
            }
            internal set
            {
                thumbnail = value;
            }
        }

        /// <summary> Highlighted snippet of text for this single page result from a search within a document </summary>
        public string Snippet
        {
            get
            {
                return snippet ?? String.Empty;
            }
            internal set
            {
                snippet = value;
            }
        }
    }
}
