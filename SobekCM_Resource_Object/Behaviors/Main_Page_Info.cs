#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Stores the information about the main page with a digital resource.  This is used for the FULL VIEW on SobekCM. </summary>
    /// <remarks>Object created by Mark V Sullivan (2009) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Main_Page_Info : XML_Writing_Base_Type
    {
        private string filename;
        private bool next_page_exists;
        private string pagename;
        private bool previous_page_exists;

        /// <summary> Constructor for a new instance of the main page info object </summary>
        public Main_Page_Info()
        {
            previous_page_exists = false;
            next_page_exists = false;
        }

        /// <summary> Gets or sets the file name for the main page of this item </summary>
        public string FileName
        {
            get { return filename ?? String.Empty; }
            set { filename = value; }
        }

        /// <summary> Gets or sets the page name for the main page of this item </summary>
        public string PageName
        {
            get { return pagename ?? String.Empty; }
            set { pagename = value; }
        }

        /// <summary> Gets or sets the flag indicating there are previous pages from the main page of this item </summary>
        /// <remarks>This is used to determine whether to show the FIRST and PREVIOUS buttons on the SobekCM Full Result View</remarks>
        public bool Previous_Page_Exists
        {
            get { return previous_page_exists; }
            set { previous_page_exists = value; }
        }

        /// <summary> Gets or sets the flag indicating there are next pages from the main page of this item </summary>
        /// <remarks>This is used to determine whether to show the NEXT and LAST buttons on the SobekCM Full Result View</remarks>
        public bool Next_Page_Exists
        {
            get { return next_page_exists; }
            set { next_page_exists = value; }
        }

        /// <summary> Add the METS for this main page information to the METS page (under SobekCM namespace) </summary>
        /// <param name="SobekcmNamespace">METS extension schema namespace to use</param>
        /// <param name="Results">Results stream to write the METS-encoded serial information </param>
        internal void Add_METS(string SobekcmNamespace, TextWriter Results)
        {
            if (String.IsNullOrEmpty(filename))
                return;

            Results.Write("<" + SobekcmNamespace + ":MainPage previous=\"" + previous_page_exists.ToString().ToLower() + "\" pagename=\"");
            Results.Write(!String.IsNullOrEmpty(pagename) ? Convert_String_To_XML_Safe(pagename) : Convert_String_To_XML_Safe((new FileInfo(filename)).Name));
            Results.Write("\" next=\"" + next_page_exists.ToString().ToLower() + "\">" + Convert_String_To_XML_Safe(filename) + "</" + SobekcmNamespace + ":MainPage>\r\n");
        }
    }
}