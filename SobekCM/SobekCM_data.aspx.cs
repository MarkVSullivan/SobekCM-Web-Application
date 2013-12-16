using System;
using SobekCM.Library.Navigation;

namespace SobekCM
{
	public partial class SobekCM_Data : System.Web.UI.Page
	{
		private SobekCM_Page_Globals pageGlobals;

		#region Page_Load method does the final checks and creates the writer type

		protected void Page_Load(object Sender, EventArgs E)
		{
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

		#region Methods called during execution of the page

		protected void Write_Html_XML_or_JSON()
		{
			// Add the HTML and controls to start this off
			pageGlobals.mainWriter.Write_Html(Response.Output, pageGlobals.tracer);
		}

		#endregion

		protected override void OnInit(EventArgs E)
		{
			pageGlobals = new SobekCM_Page_Globals(IsPostBack, "SOBEKCM_DATA");

			base.OnInit(E);
		}
	}
}