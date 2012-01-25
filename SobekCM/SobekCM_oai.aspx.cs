#region Includes

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;

#endregion

public partial class SobekCM_oai : System.Web.UI.Page
{
    private SobekCM_Page_Globals Page_Globals;

    #region Page_Load method does the final checks and creates the writer type

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page_Globals.currentMode.Writer_Type != Writer_Type_Enum.OAI)
        {
            Page_Globals.currentMode.Writer_Type = Writer_Type_Enum.OAI;
        }

        try
        {
            Page_Globals.On_Page_Load();
        }
        catch (OutOfMemoryException ee)
        {
            Page_Globals.Email_Information("UFDC Out of Memory Exception", ee);
        }
        catch (Exception ee)
        {
            Page_Globals.currentMode.Mode = Display_Mode_Enum.Error;
            Page_Globals.currentMode.Error_Message = "Unknown error caught while executing your request";
            Page_Globals.currentMode.Caught_Exception = ee;
        }
    }

    #endregion

    #region Methods called during execution of the HTML from UFDC.aspx

    protected void Add_Html()
    {
        // Add the HTML and controls to start this off
        Page_Globals.mainWriter.Add_Text_To_Page(Response.Output, Page_Globals.tracer);
    }

    #endregion

    protected override void OnInit(EventArgs e)
    {
        Page_Globals = new SobekCM_Page_Globals(IsPostBack, "SOBEKCM_OAI");

        base.OnInit(e);
    }
}
