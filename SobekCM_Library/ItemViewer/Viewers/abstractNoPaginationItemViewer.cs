using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.HTML;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Abstract item viewer is a good base class to extend for item viewers that
    /// do not use the built-in paging features that are available from the default item HTML subwriters </summary>
    /// <remarks> This abstract class implements the <see cref="iItemViewer" /> interface, although 
    /// several of the main methods are left abstract here and MUST be implemented by any classes that
    /// extend this abstract class.  </remarks>
    public abstract class abstractNoPaginationItemViewer : iItemViewer
    {
        /// <summary> Protected collection of behaviors for this item viewer </summary>
        protected List<HtmlSubwriter_Behaviors_Enum> Behaviors;

        /// <summary> Current digital resource item to display </summary>
        protected BriefItemInfo BriefItem;

        /// <summary> Current navigation information for this individual HTML request </summary>
        protected Navigation_Object CurrentRequest;

        /// <summary> Current user, which can help determine how things should display </summary>
        protected User_Object CurrentUser;

        /// <summary> Empty list of behaviors, returned by default </summary>
        /// <remarks> This just prevents an empty set from having to be created over and over </remarks>
        protected static List<HtmlSubwriter_Behaviors_Enum> EmptyBehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <remarks> By default this return NULL, but can be overwritten by all the individual item viewers </remarks>
        public virtual string ViewerBox_CssId
        {
            get { return null; }
        }

        /// <summary> Any additional inline style for this viewer that affects the main box around this</summary>
        /// <remarks> By default this return NULL, but can be overwritten by all the individual item viewers </remarks>
        public virtual string ViewerBox_InlineStyle
        {
            get { return null; }
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default this returns the contents of the protected behaviors property, but this can
        /// also be overwritten by individual item viewers if the behaviors needs to be determined
        /// more dynamically </remarks>
        public virtual List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get { return Behaviors;  }
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public virtual void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing by default, but can override in any classes that extend this abstract class
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public virtual void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            // Do nothing by default, but can override in any classes that extend this abstract class
        }

        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public virtual void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing by default, but can override in any classes that extend this abstract class
        }

        /// <summary> Adds any viewer_specific information to the item viewer above the standard pagination buttons </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public virtual void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing by default, but can override in any classes that extend this abstract class

            // NOTE: The only classes that use this are:
            //           - QC viewer (should probably be a mySobek viewer)
            //           - Related Images (could probably be moved into html viewer writing spot)

            // TODO: Get rid of this method
        }

        /// <summary> Abstract method writes the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This MUST be implemented by all classes that implement this abstract class </remarks>
        public abstract void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Abstract method allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This MUST be implemented by all classes that implement this abstract class </remarks>
        public abstract void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);

        #region Paging related properties ( no paging enabled )

        /// <summary> Gets the flag that indicates if the page selector should be shown, and which page selector </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get { return ItemViewer_PageSelector_Type_Enum.NONE; }
        }


        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public int PageCount
        {
            get { return 1; }
        }

        /// <summary> Gets the current page for paging purposes </summary>
        /// <value> This always returns the value 1, since this is for single page viewers</value>
        public int Current_Page
        {
            get { return 1; }
        }

        /// <summary> Gets the url to go to the first page </summary>
        /// <value> This always returns NULL, since this does not have paging enabled </value>
        public string First_Page_URL 
        {
            get {  return null; }
        }

        /// <summary> Gets the url to go to the previous page </summary>
        /// <value> This always returns NULL, since this does not have paging enabled </value>
        public string Previous_Page_URL
        {
            get { return null; }
        }

        /// <summary> Gets the url to go to the next page </summary>
        /// <value> This always returns NULL, since this does not have paging enabled </value>
        public string Next_Page_URL
        {
            get { return null; }
        }

        /// <summary> Gets the url to go to the last page </summary>
        /// <value> This always returns NULL, since this does not have paging enabled </value>
        public string Last_Page_URL
        {
            get { return null; }
        }

        /// <summary> Gets the names to show in the Go To combo box </summary>
        /// <value> This always returns NULL, since this does not have paging enabled </value>
        public string[] Go_To_Names
        {
            get { return null; }
        }

        #endregion
    }
}
