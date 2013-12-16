using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.Fragments
{
    /// <summary> Base class is used for all fragment item viewers, which just write a single fragment 
    /// to the stream, such as the print or send/email forms.  THis is used to allow the page to dynamically
    /// load these forms as needed. </summary>
    public abstract class baseFragment_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This always returns 1, to suppress any pagination from occurring </value>
        public override int PageCount
        {
            get { return 1; }
        }

        /// <summary> Gets the url to go to the first page </summary>
        /// <value> Always returns an empty string </value>
        public override string First_Page_URL
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary> Gets the url to go to the previous page </summary>
        /// <value> Always returns an empty string </value>
        public override string Previous_Page_URL
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary> Gets the url to go to the next page </summary>
        /// <value> Always returns an empty string </value>
        public override string Next_Page_URL
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary> Gets the url to go to the last page </summary>
        /// <value> Always returns an empty string </value>
        public override string Last_Page_URL
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary> Gets the names to show in the Go To combo box </summary>
        /// <value> Always returns an empty string array </value>
        public override string[] Go_To_Names
        {
            get
            {
               return new string[] { };
            }
        }
    }
}
