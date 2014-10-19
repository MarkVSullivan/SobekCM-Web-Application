#region Using directives

using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Abstract class which all main writer classes must extend </summary>
    public abstract class abstractMainWriter
    {
        /// <summary> Protected field contains the information specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Constructor for a new instance of the abstractMainWriter abstract class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        protected abstractMainWriter( RequestCache RequestSpecificValues )
        {
            this.RequestSpecificValues = RequestSpecificValues;
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        public abstract Writer_Type_Enum Writer_Type { get; }

        /// <summary> Returns a flag indicating whether the navigation form should be included in the page </summary>
        /// <value> This value can be override by child classes, but by default this returns FALSE </value>
        public virtual bool Include_Navigation_Form
        {
            get
            {
                return false;
            }
        }


        /// <summary> Returns a flag indicating whether the additional table of contents place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This value can be override by child classes, but by default this returns FALSE </value>
        public virtual bool Include_TOC_Place_Holder
        {
            get
            {
                return false;
            }
        }

        /// <summary> Returns a flag indicating whether the additional place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This value can be override by child classes, but by default this returns FALSE </value>
        public virtual bool Include_Main_Place_Holder
        {
            get
            {
                return false;
            }
        }

	    /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
	    /// for the current request, or if it can be hidden. </summary>
	    /// <value> This value can be override by child classes, but by default this returns FALSE </value>
	    public virtual bool File_Upload_Possible
		{
			get
			{
				return false;
			}
		}


        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public abstract void Write_Html(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Perform all the work of adding to the response stream back to the web user </summary>
        /// <param name="TOC_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Main_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public virtual void Add_Controls(PlaceHolder TOC_Place_Holder, PlaceHolder Main_Place_Holder, Custom_Tracer Tracer)
        {
            // Do nothing
        }
    }
}
