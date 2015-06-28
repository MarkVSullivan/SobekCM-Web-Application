#region Using directives

using SolrNet.Attributes;

#endregion

namespace SobekCM.Resource_Object.Solr
{
    /// <summary> Object stores the basic information about a page within this digital object and makes
    /// the basic attributes and full text available for Solr indexing </summary>
    public class SolrPage
    {
        private readonly string bibid;
        private readonly string pagename;
        private readonly int pageorder;
        private readonly string pagetext;
        private readonly string thumbnail;
        private readonly string vid;

        /// <summary> Constructor creates a new instance of the SolrPage class </summary>
        public SolrPage()
        {
        }

        /// <summary> Constructor creates a new instance of the SolrPage class </summary>
        /// <param name="BibID"> Bibliograhpic identifier (BibID) for the parent digital object </param>
        /// <param name="VID"> Volume identifier (VID) for the parent digital object </param>
        /// <param name="PageOrder"> Order this page appears within the parent digital object ( first page = 1 )</param>
        /// <param name="PageName"> Label for this page from the METS file ( i.e., 'Page 1', 'Cover 1', etc..) </param>
        /// <param name="PageText"> Full text for this page</param>
        /// <param name="Thumbnail"> Thumbnail image to be displayed within the search results of a single-document search </param>
        public SolrPage(string BibID, string VID, int PageOrder, string PageName, string PageText, string Thumbnail)
        {
            bibid = BibID;
            vid = VID;
            pageorder = PageOrder;
            pagename = PageName;
            pagetext = PageText;
            thumbnail = Thumbnail;
        }

        /// <summary> Returns the unique PageID for the Solr engine to index for this page </summary>
        [SolrUniqueKey("pageid")]
        public string PageID
        {
            get { return bibid + ":" + vid + ":" + pageorder.ToString().PadLeft(5, '0'); }
        }

        /// <summary> Returns the DID for the Solr engine to index for this page </summary>
        [SolrField("did")]
        public string Did
        {
            get { return bibid + ":" + vid; }
        }

        /// <summary> Returns the bibliographic identifier (BibID) for the Solr engine to index for this page </summary>
        [SolrField("bibid")]
        public string BibID
        {
            get { return bibid; }
        }

        /// <summary> Returns the volume identifier (VID) for the Solr engine to index for this page </summary>
        [SolrField("vid")]
        public string VID
        {
            get { return vid; }
        }

        /// <summary> Returns the page order for this page within the greater document for the Solr engine to store for this page </summary>
        [SolrField("pageorder")]
        public int PageOrder
        {
            get { return pageorder; }
        }

        /// <summary> Returns the name of this page for the Solr engine to store for this page </summary>
        [SolrField("pagename")]
        public string PageName
        {
            get { return pagename; }
        }

        /// <summary> Returns the name of the thumbnail associated with this page for the Solr engine to store for this page </summary>
        [SolrField("thumbnail")]
        public string Thumbnail
        {
            get { return thumbnail; }
        }

        /// <summary> Returns the full text for this page for the Solr engine to index for this page </summary>
        [SolrField("pagetext")]
        public string PageText
        {
            get { return pagetext; }
        }
    }
}