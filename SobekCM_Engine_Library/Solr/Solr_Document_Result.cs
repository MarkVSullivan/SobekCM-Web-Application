#region Using directives

using System;
using SobekCM.Core.Results;
using SolrNet.Attributes;

#endregion

namespace SobekCM.Engine_Library.Solr
{
    /// <summary> Stores the information relating to a single result from ansearch within a Solr index  </summary>
    /// <remarks> This is populated by the Solr query through SolrNet, using the SolrNet property attributes as guides.  The highlighted snippet
    /// containing the search term(s) for this page is also loaded into this object<br /><br />
    /// This class implements both the <see cref="Results.iSearch_Title_Result" /> and <see cref="Results.iSearch_Item_Result" /> interfaces because
    /// Solr/Lucene search results are a flattened, non-hierarchical structure.  Each result contains all the title and item information within
    /// a single result object.</remarks>
    [Serializable]
    public class Solr_Document_Result : iSearch_Title_Result, iSearch_Item_Result
    {
        private string author;
        private string did;
        private string donor, edition, format, holdinglocation;
        private string mainthumbnail;
        private string maintitle, materialtype;
        private string pubdate;
        private string publisher;
        private string snippet;
        private string sourceinstitution;
        private string url;

        /// <summary> Constructor for a new instance of the Solr_Document_Result class </summary>
        public Solr_Document_Result()
        {
            // Do nothing
            OPAC_Number = -1;
            OCLC_Number = -1;
        }

        /// <summary> Unique identifier for the digital object for this single result from a search within this library </summary>
        [SolrField("did")]
        public string DID
        {
            get
            {
                return did;
            }
            internal set
            {
                did = value;

                string[] split = did.Split(new[] { ':' });
                if (split.Length == 2)
                {
                    BibID = split[0];
                    VID = split[1];
                }
            }
        }

        /// <summary> URL associated with this single result from a search within this library </summary>
        [SolrField("url")]
        public string URL
        {
            get
            {
                return url ?? String.Empty;
            }
            internal set
            {
                url = value;
            }
        }

        /// <summary> Holding location associated with this single result from a search within this library </summary>
        [SolrField("holdinglocation")]
        public string HoldingLocation
        {
            get
            {
                return holdinglocation ?? String.Empty;
            }
            internal set
            {
                holdinglocation = value;
            }
        }

        /// <summary> Source institution associated with this single result from a search within this library </summary>
        [SolrField("sourceinstitution")]
        public string SourceInstitution
        {
            get
            {
                return sourceinstitution ?? String.Empty;
            }
            internal set
            {
                sourceinstitution = value;
            }
        }

        #region iSearch_Item_Result Members

        /// <summary> Volume identifier (VID) associated with this single result from a search within this library </summary>
        public string VID { get; private set; }

        /// <summary> Main title associated with this single result from a search within this library </summary>
        [SolrField("maintitle")]
        public string Title
        {
            get
            {
                return maintitle ?? String.Empty;
            }
            internal set
            {
                maintitle = value;
            }
        }

        /// <summary> Publication date associated with this single result from a search within this library </summary>
        [SolrField("pubdate_display")]
        public string PubDate
        {
            get
            {
                return pubdate ?? String.Empty;
            }
            internal set
            {
                pubdate = value;
            }
        }

        /// <summary> Main thumbnail associated with this single result from a search within this library </summary>
        [SolrField("mainthumbnail")]
        public string MainThumbnail
        {
            get
            {
                return mainthumbnail ?? String.Empty;
            }
            internal set
            {
                mainthumbnail = value;
            }
        }

        #endregion

        #region iSearch_Title_Result Members

        /// <summary> Bibliographic identifier (BibID) associated with this single result from a search within this library </summary>
        public string BibID { get; private set; }

        /// <summary> Material type associated with this single result from a search within this library </summary>
        [SolrField("materialtype")]
        public string MaterialType
        {
            get
            {
                return materialtype ?? String.Empty;
            }
            internal set
            {
                materialtype = value;
            }
        }

        /// <summary> Local OPAC number associated with this single result from a search within this library </summary>
        [SolrField("aleph")]
        public long OPAC_Number { get; internal set; }

        /// <summary> OCLC associated with this single result from a search within this library </summary>
        [SolrField("oclc")]
        public long OCLC_Number { get; internal set; }

        /// <summary> Type of the primary alternate identifier for this resource ( i.e. 'Accession Number', etc.. )</summary>
        /// <remarks> This is currently not in use for Solr/Lucene results </remarks>
        public string Primary_Identifier_Type { get { return String.Empty; } }

        /// <summary> Primary alternate identifier for this resource</summary>
        /// <remarks> This is currently not in use for Solr/Lucene results </remarks>
        public string Primary_Identifier { get { return String.Empty; } }

        /// <summary> Gets the number of items contained within this title result </summary>
        /// <remarks> This always returns one in this implementation, since each Solr/Lucene result
        /// contains the flattened title and item information and is not displayed hierarchically </remarks>
        public int Item_Count { get { return 1; }}

        /// <summary> Gets the item indicated by the provided index </summary>
        /// <param name="Index"> Index of the item requested </param>
        /// <returns> Item result requested, or NULL </returns>
        /// <remarks> Since this class implements the <see cref="Results.iSearch_Item_Result" /> interface this
        /// method actually returns this object because each Solr/Lucene result contains the flattened
        /// title and item information and is not displayed hierarchically </remarks>
        public iSearch_Item_Result Get_Item(int Index)
        {
            return this;
        }

        /// <summary> Gets the item tree view used for showing all the items under this title in a tree type html display </summary>
        /// <remarks> Since Solr/Lucene results only ever contain one item per title, this always returns NULL </remarks>
        public Search_Result_Item_Tree Item_Tree 
        {
            get { return null; }
        }

        /// <summary> Builds the tree of items under this title, for multiple item titles </summary>
        /// <remarks> Since Solr/Lucene results only ever contain one item per title, this does nothing </remarks>
        public void Build_Item_Tree(string ResultsIndex)
        {
            // Do nothing
        }

        /// <summary> Highlighted snippet of text from this document </summary>
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

	    /// <summary> Metadata values to display for this item title result </summary>
	    public string[] Metadata_Display_Values
	    {
		    get
		    {
				string[] returnVal = new string[6];
			    returnVal[0] = Author;
			    returnVal[1] = Publisher;
			    returnVal[2] = Format;
			    returnVal[3] = Edition;
			    returnVal[4] = Institution;
			    returnVal[5] = Donor;
			    return returnVal;
		    }

	    }

        #endregion

		#region Old iSearch_Title_Result_Members, eventually to be deprecated?

		/// <summary> Donor's name associated with this single result from a search within this library </summary>
		[SolrField("donor")]
		public string Donor
		{
			get
			{
				return donor ?? String.Empty;
			}
			internal set
			{
				donor = value;
			}
		}

		/// <summary> Edition associated with this single result from a search within this library </summary>
		[SolrField("edition")]
		public string Edition
		{
			get
			{
				return edition ?? String.Empty;
			}
			internal set
			{
				edition = value;
			}
		}

		/// <summary> Format/Physical Description associated with this single result from a search within this library </summary>
		[SolrField("format")]
		public string Format
		{
			get
			{
				return format ?? String.Empty;
			}
			internal set
			{
				format = value;
			}
		}

		/// <summary> Author (display version) associated with this single result from a search within this library </summary>
		[SolrField("author_display")]
		public string Author
		{
			get
			{
				return author ?? String.Empty;
			}
			internal set
			{
				author = value;
			}
		}

		/// <summary> Publisher (display version) associated with this single result from a search within this library </summary>
		[SolrField("publisher_display")]
		public string Publisher
		{
			get
			{
				return publisher ?? String.Empty;
			}
			internal set
			{
				publisher = value;
			}
		}

		/// <summary> Spatial coverage associated with this single result from a search within this library </summary>
		public string Spatial_Coverage
		{
			get { return String.Empty; }
		}

		/// <summary> Spatial coverage for this title result in terms of coordinates for map display </summary>
		public string Spatial_Coordinates
		{
			get { return String.Empty; }
		}

		/// <summary> Institution associated with this single result from a search within this library </summary>
		public string Institution
		{
			get { return String.Empty; }
		}

		/// <summary> Materials used in creation of a search result from a search within this library </summary>
		public string Material
		{
			get { return String.Empty; }
		}

		/// <summary> Measurements for this  single result from a search within this library </summary>
		public string Measurement
		{
			get { return String.Empty; }
		}

		/// <summary> Style/period associated with this single result from a search within this library </summary>
		public string Style_Period
		{
			get { return String.Empty; }
		}

		/// <summary> Technique associated with this single result from a search within this library </summary>
		public string Technique
		{
			get { return String.Empty; }
		}

		/// <summary> Subjects associated with this title result </summary>
		public string Subjects
		{
			get { return String.Empty; }
		}

		#endregion

		#region Unimplemented portions of the iSearch_Title_Result interface

		/// <summary> Group title for this title result </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Title_Result" /> interface, but is not really implement in this instance.  The empty string is always returned. </remarks>
        public string GroupTitle
        {
            get { return String.Empty; }
        }

        /// <summary> Group-wide thumbnail for this title result </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Title_Result" /> interface, but is not really implement in this instance.  The empty string is always returned. </remarks>
        public string GroupThumbnail
        {
            get { return String.Empty; }
        }

        /// <summary> User notes for this title result, if it is in a bookshelf </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Title_Result" /> interface, but is not really implement in this instance.  The empty string is always returned. </remarks>
        public string UserNotes
        {
            get { return String.Empty; }
        }


        #endregion

        #region Unimplemented portions of the iSearch_Item_Result interface

        /// <summary> IP restriction mask for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this instance.  Zero (public) is always returned. </remarks>
        public short IP_Restriction_Mask
        {
            get { return 0; }
        }

        /// <summary> Number of pages within this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this instance.  -1 is always returned. </remarks>
        public int PageCount
        {
            get { return -1; }
        }

        /// <summary> External URL for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this instance.  The empty string is always returned. </remarks>
        public string Link
        {
            get { return String.Empty; }
        }

        /// <summary> Index of the first serial hierarchy level for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this 
        /// instance since Solr/Lucene results are always a single item/title, rather than a collection 
        /// of items within a title which would require serial hierarchy.  -1 is always returned. </remarks>
        public short Level1_Index
        {
            get { return -1; }
        }

        /// <summary> Text of the first serial hierarchy level for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this 
        /// instance since Solr/Lucene results are always a single item/title, rather than a collection 
        /// of items within a title which would require serial hierarchy.  The empty string is always returned. </remarks>
        public string Level1_Text
        {
            get { return String.Empty; }
        }

        /// <summary> Index of the second serial hierarchy level for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this 
        /// instance since Solr/Lucene results are always a single item/title, rather than a collection 
        /// of items within a title which would require serial hierarchy.  -1 is always returned. </remarks>
        public short Level2_Index
        {
            get { return -1; }
        }

        /// <summary> Text of the second serial hierarchy level for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this 
        /// instance since Solr/Lucene results are always a single item/title, rather than a collection 
        /// of items within a title which would require serial hierarchy.  The empty string is always returned. </remarks>
        public string Level2_Text
        {
            get { return String.Empty; }
        }

        /// <summary> Index of the third serial hierarchy level for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this 
        /// instance since Solr/Lucene results are always a single item/title, rather than a collection 
        /// of items within a title which would require serial hierarchy.  -1 is always returned. </remarks>
        public short Level3_Index
        {
            get { return -1; }
        }

        /// <summary> Text of the third serial hierarchy level for this item within a title within a collection of results </summary>
        /// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this 
        /// instance since Solr/Lucene results are always a single item/title, rather than a collection 
        /// of items within a title which would require serial hierarchy.  The empty string is always returned. </remarks>
        public string Level3_Text
        {
            get { return String.Empty; }
        }

		/// <summary> Spatial coverage as KML for this item within a title result for map display </summary>
		/// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this instance.  The empty string is always returned. </remarks>
		public string Spatial_KML
		{
			get { return String.Empty; }
		}

		/// <summary> COinS OpenURL format of citation for citation sharing </summary>
		/// <remarks> This is required by the <see cref="Results.iSearch_Item_Result" /> interface, but is not really implement in this instance.  The empty string is always returned. </remarks>
		public string COinS_OpenURL
		{
			get { return String.Empty; }
		}

        #endregion
    }
}
