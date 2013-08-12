#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;
using SobekCM.Resource_Object;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Abstract class which implements the <see cref="iItemViewer"/> interface and which all subsequent
    /// item viewer classes must extend </summary>
    public abstract class abstractItemViewer : Page, iItemViewer
	{

	    /// <summary> Protected field contains the translation object for displaying an item in different languages </summary>
        protected Language_Support_Info translator;

        /// <summary> Empty list of behaviors, returned by default </summary>
        /// <remarks> This just prevents an empty set from having to be created over and over </remarks>
        protected static List<HtmlSubwriter_Behaviors_Enum> emptybehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

	    /// <summary> Constructor for a new instance of the abstractItemViewer class </summary>
	    protected abstractItemViewer()
		{
			FileName = String.Empty;
		}

	    /// <summary>  Language support object which handles simple translational duties  </summary>
		public Language_Support_Info Translator
		{
			get	{	return translator;		}
			set	{	translator = value;		}
		}

	    /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <value> By defaut, this method returns FALSE </value>
        public virtual bool Override_On_Checked_Out { get { return false; } }

	    #region iItemViewer Members

	    /// <summary> Sets the attributes for this viewer (from the database) </summary>
	    public virtual string Attributes { set { return;  } }

	    /// <summary> Sets the filename for this viewer (if this is a simple viewer)</summary>
	    public string FileName { protected get; set; }

	    /// <summary> Sets the mode / navigation information for the current request </summary>
	    public SobekCM_Navigation_Object CurrentMode { protected get; set; }

	    /// <summary> Sets the current item for this viewer to display </summary>
	    public SobekCM_Item CurrentItem { protected get; set; }

        /// <summary> Sets the current user, in case there are any user options to include </summary>
        public User_Object CurrentUser { protected get; set; }

        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public virtual void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing by default
        }

        /// <summary> Adds any viewer_specific information to the item viewer above the standard pagination buttons </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public virtual void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing by default
        }

        /// <summary> Abstract method adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public virtual void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            // Do nothing by default
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public virtual void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing by default
        }

        #region Paging related properties
        
        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This value is override by many of the children classes, but by default this returns the number of pages within the digital resource </value>
		public virtual int PageCount		
		{
			get
			{
                return CurrentItem.Web.Static_PageCount;
			}
		}

		/// <summary> Gets the url to go to the first page </summary>
		public virtual string First_Page_URL		
		{
			get
			{
			    // Only continue if there is an item and mode, and there is previous pages to go to
				if ( CurrentMode.Page > 1 )
				{
					int currSeq = CurrentMode.Page;
					string currView = CurrentMode.ViewerCode;

					// Add the button for the first page
					CurrentMode.ViewerCode = currView.Replace( currSeq.ToString(), "1" );
					string returnVal = CurrentMode.Redirect_URL();
				
					// Restore the original sequence
					CurrentMode.ViewerCode = currView;

					return returnVal;
				}
			    return String.Empty;
			}
		}

		/// <summary> Gets the url to go to the previous page </summary>
		public virtual string Previous_Page_URL		
		{
			get
			{
			    // Only continue if there is an item and mode, and there is previous pages to go to
				if ( CurrentMode.Page > 1 )
				{
					int currSeq = CurrentMode.Page;
					string currView = CurrentMode.ViewerCode;

					// Add the button for the previous page
					CurrentMode.ViewerCode = currView.Replace( currSeq.ToString(), (currSeq - 1).ToString() );
					string returnVal = CurrentMode.Redirect_URL();

					// Restore the original sequence
					CurrentMode.ViewerCode = currView;

					return returnVal;
				}
			    return String.Empty;
			}
		}

		/// <summary> Gets the url to go to the next page </summary>
		public virtual string Next_Page_URL 		
		{
			get
			{
			    // Only continue if there is an item and mode, and there is next pages to go to
				if ( CurrentMode.Page < PageCount )
				{
					int currSeq = CurrentMode.Page;
					string currView = CurrentMode.ViewerCode;

					// Add the button for the previous page
					CurrentMode.ViewerCode = currView.Replace( currSeq.ToString(), (currSeq + 1).ToString() );
					string returnVal = CurrentMode.Redirect_URL();

					// Restore the original sequence
					CurrentMode.ViewerCode = currView;

					return returnVal;
				}
			    return String.Empty;
			}
		}

		/// <summary> Gets the url to go to the last page </summary>
		public virtual string Last_Page_URL		
		{
			get
			{
			    // Only continue if there is an item and mode, and there is next pages to go to
				if ( CurrentMode.Page < PageCount )
				{
					int currSeq = CurrentMode.Page;
					string currView = CurrentMode.ViewerCode;

					// Add the button for the previous page
					CurrentMode.ViewerCode = currView.Replace( currSeq.ToString(), PageCount.ToString() );
					string returnVal = CurrentMode.Redirect_URL();

					// Restore the original sequence
					CurrentMode.ViewerCode = currView;

					return returnVal;
				}
			    return String.Empty;
			}
		}

		/// <summary> Gets the names to show in the Go To combo box </summary>
        /// <value> By default, this is the collection of names for each page of the document </value>
		public virtual string[] Go_To_Names		
		{
			get
			{
                bool some_pages_named = false;
				string[] page_names = new string[ CurrentItem.Web.Static_PageCount ];
				for( int i = 0 ; i < page_names.Length ; i++ )
				{
                    if (CurrentItem.Web.Pages_By_Sequence[i].Label.Length > 0)
                    {
                        page_names[i] = CurrentItem.Web.Pages_By_Sequence[i].Label;
                        some_pages_named = true;
                    }
                    else
                    {
                        page_names[i] = "Unnumbered " + (i + 1).ToString();
                    }
				}
                if (!some_pages_named)
                {
                    for (int i = 0; i < page_names.Length; i++)
                    {
                        page_names[i] = "Page " + (i + 1).ToString();
                    }
                }
				return page_names;
			}
		}

        #endregion

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This value is override by many of the children classes, but by default this returns the value -1 </value>
		public virtual int Viewer_Width
		{
			get
			{
				return -1;
			}
		}

        /// <summary> Height for the main viewer section to adjusted to accomodate this viewer</summary>
        public virtual int Viewer_Height
        {
            get
            {
                return -1;
            }
        }

        /// <summary> Gets the current page for paging purposes </summary>
        /// <value> By default this returns the page value from the current reqeust mode</value>
        public virtual int Current_Page
        {
            get { return CurrentMode.Page; }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This value is override by some of the children classes, but by default this returns TRUE </value>
        public virtual ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.DropDownList;
            }
        }

        /// <summary> Abstract property gets the type of item viewer </summary>
        public abstract ItemViewer_Type_Enum ItemViewer_Type { get; }

        /// <summary> This provides an opportunity for the viewer to perform any pre-display work
        /// which is necessary before entering any of the rendering portions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing by default, but can be overridden by classes that extend this class </remarks>
        public virtual void Perform_PreDisplay_Work( Custom_Tracer Tracer )
        {
            // Do nothing by default
        }

	    #endregion

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public virtual void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            // Do nothing by default
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public virtual void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
	    public virtual List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
	    {
            get { return emptybehaviors; }
	    }

        protected static string Convert_String_To_XML_Safe(string element)
        {
           // System.Xml.Linq .Xml.Linq .Linq.XElement  newElement = new XmlElement()
            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("[", "").Replace("]", "");
        }
	}
}
