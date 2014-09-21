#region Includes

using System;
using SobekCM.Library.Navigation;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM
{
	public partial class SobekCM_OAI : System.Web.UI.Page
	{
		private SobekCM_Page_Globals pageGlobals;

		#region Page_Load method does the final checks and creates the writer type

		protected void Page_Load(object Sender, EventArgs E)
		{
			// This should already be true
			pageGlobals.currentMode.Writer_Type = Writer_Type_Enum.OAI;

			try
			{
				pageGlobals.On_Page_Load();
			}
			catch (OutOfMemoryException ee)
			{
				pageGlobals.Email_Information("UFDC Out of Memory Exception", ee);
			}
			catch (Exception ee)
			{
				pageGlobals.currentMode.Mode = Display_Mode_Enum.Error;
				pageGlobals.currentMode.Error_Message = "Unknown error caught while executing your request";
				pageGlobals.currentMode.Caught_Exception = ee;
			}
		}

		#endregion

		#region Methods called during execution of the HTML from UFDC.aspx

		protected void Write_Html()
		{
			// Add the HTML and controls to start this off
			pageGlobals.mainWriter.Write_Html(Response.Output, pageGlobals.tracer);
		}

		#endregion

		protected override void OnInit(EventArgs E)
		{
			pageGlobals = new SobekCM_Page_Globals(IsPostBack, "SOBEKCM_OAI");

			base.OnInit(E);
		}
	}
}