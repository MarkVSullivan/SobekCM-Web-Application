#region Using directives

using System;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Abstract class which implements the <see cref="iItemViewer"/> interface and which all subsequent
    /// item viewer classes must extend </summary>
	public abstract class abstractItemViewer : iItemViewer
	{
	    /// <summary> Protected field contains the translation object for displaying an item in different languages </summary>
        protected Language_Support_Info translator;

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
	    public virtual SobekCM_Item CurrentItem { protected get; set; }

        /// <summary> Sets the current user, in case there are any user options to include </summary>
        public virtual User_Object CurrentUser { protected get; set; }

	    /// <summary> Abstract method adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if this viewer added something to the left navigational bar, otherwise FALSE</returns>
        public abstract bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer);

		/// <summary> Gets any HTML for a Navigation Row above the image or text </summary>
        /// <value> This value can be override by child classes, but by default this returns an empty string </value>
		public virtual string NavigationRow
		{
			get
			{
				return string.Empty;
			}
		}

        /// <summary> Abstract method adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public abstract void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer);
		
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

        /// <summary> Flag indicates if the header (with the title, group title, etc..) should be displayed </summary>
        /// <value> This value is override by many of the children classes, but by default this returns TRUE </value>
		public virtual bool Show_Header
		{ 
			get
			{
				return true;
			}
		}

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This value is override by many of the children classes, but by default this returns the value -1 </value>
		public virtual int Viewer_Width
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

	    /// <summary> Flag indicates if the item viewer should add the standard item menu, or
	    /// if this item viewer overrides that menu and will write its own menu </summary>
	    /// <remarks> By default, this returns TRUE.  The QC and the spatial editing itemviewers create their own custom menus
	    /// due to the complexity of the work being done in those viewers. </remarks>
	    public virtual bool Include_Standard_Item_Menu
	    {
            get { return true;  }
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

	    /// <summary> Event is called when an item viewer requests a redirect </summary>
	    public event Redirect_Requested Redirect;

	    /// <summary> Method allows children classes to indirectly invoke the <see cref="Redirect"/> event </summary>
	    /// <param name="new_url"> URL to which to redirect the user </param>
	    protected void OnRedirect( string new_url )
	    {
	        if ( Redirect != null )
	            Redirect( new_url );
	    }
	}
}
