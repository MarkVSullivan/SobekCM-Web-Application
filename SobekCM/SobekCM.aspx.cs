#region Includes 

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
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

using darrenjohnstone.net.FileUpload;

#endregion

public partial class UFDC : System.Web.UI.Page
{
    private SobekCM_Page_Globals Page_Globals;

    #region Page_Load method does the final checks and creates the writer type

    protected void Page_Load(object sender, EventArgs e)
    {
        //SobekCM.Library.MARC_Record_Z3950_Retriever.Test();

        Page_Globals.tracer.Add_Trace("System.Web.UI.Page.Page_Load (ufdc.Page_Load)", String.Empty);

         try
        {
            Page_Globals.On_Page_Load();

            if (HttpContext.Current.Items.Contains("Original_URL"))
            {
                string original_url = HttpContext.Current.Items["Original_URL"].ToString();
                itemNavForm.Action = original_url;
                fileUploadForm.Action = original_url;

                // Save this as the return spot, if it is not preferences
                if ((Page_Globals.currentMode.Mode != Display_Mode_Enum.Preferences) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Contact))
                {
                    Session["Last_Mode"] = original_url;
                }
            }
            else
            {
                // Save this as the return spot, if it is not preferences
                if ((Page_Globals.currentMode.Mode != Display_Mode_Enum.Preferences) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Contact))
                {
                    string url = HttpContext.Current.Request.Url.ToString();
                    Session["Last_Mode"] = url;
                }
            }

            if (SobekCM.Library.SobekCM_Library_Settings.Web_Output_Caching_Minutes > 0)
            {
                if ((Page_Globals.currentMode.Mode != Display_Mode_Enum.Error) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Administrative) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Contact) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Contact_Sent) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Item_Print) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Item_Cache_Reload) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Reset) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Internal) &&
                    (Page_Globals.currentMode.Mode != Display_Mode_Enum.Public_Folder) &&
                    ((Page_Globals.currentMode.Mode != Display_Mode_Enum.Aggregation_Home) || (Page_Globals.currentMode.Home_Type != Home_Type_Enum.Personalized)) &&
                    (Page_Globals.currentMode.Result_Display_Type != Result_Display_Type_Enum.Export) &&
                    ((Page_Globals.currentMode.Mode != Display_Mode_Enum.Item_Display) || ((Page_Globals.currentMode.ViewerCode.Length > 0) && (Page_Globals.currentMode.ViewerCode.ToUpper().IndexOf("citation") < 0) && (Page_Globals.currentMode.ViewerCode.ToUpper().IndexOf("allvolumes3") < 0))))
                {
                    Response.Cache.SetCacheability(HttpCacheability.Private);
                    Response.Cache.SetMaxAge(new TimeSpan(0, SobekCM.Library.SobekCM_Library_Settings.Web_Output_Caching_Minutes, 0));
                }
                else
                {
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
            }

          

            // If there is maintenance, load nothing into the place holder
            if (Page_Globals.currentMode != null)
            {
                // Check if the item nav form should be shown
                if (!Page_Globals.mainWriter.Include_Navigation_Form)
                {
                    itemNavForm.Visible = false;
                }
                else
                {
                    if (!Page_Globals.mainWriter.Include_Main_Place_Holder)
                        mainPlaceHolder.Visible = false;
                    if (!Page_Globals.mainWriter.Include_TOC_Place_Holder)
                        tocPlaceHolder.Visible = false;
                    if (!Page_Globals.mainWriter.Include_Navigation_Place_Holder)
                        navigationPlaceHolder.Visible = false;
                }

                // The file upload form is only shown in ONE case
                if ((Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) || 
                    ((Page_Globals.currentMode.Writer_Type != Writer_Type_Enum.HTML) && (Page_Globals.currentMode.Writer_Type != Writer_Type_Enum.HTML_LoggedIn)) || 
                    (((Page_Globals.currentMode.My_Sobek_Type != My_Sobek_Type_Enum.New_Item) || (Page_Globals.currentMode.My_Sobek_SubMode.Length == 0) || (Page_Globals.currentMode.My_Sobek_SubMode[0] != '8')) && ( Page_Globals.currentMode.My_Sobek_Type != My_Sobek_Type_Enum.File_Management ))  || 
                    (Session["user"] == null))
                {
                    fileUploadForm.Visible = false;
                }
                else
                {
                    itemNavForm.Visible = false;
                    fileUploadForm.Visible = true;
                    fileUploadForm.Enctype = "multipart/form-data";                        

                }

                // Add the controls now
                Page_Globals.mainWriter.Add_Controls(navigationPlaceHolder, tocPlaceHolder, mainPlaceHolder, myUfdcUploadPlaceHolder, Page_Globals.tracer);
            }
        }
        catch (OutOfMemoryException ee)
        {
            Page_Globals.tracer.Add_Trace("System.Web.UI.Page.Page_Load (ufdc.Page_Load)", "OutOfMemoryException caught!");

            Page_Globals.Email_Information("UFDC Out of Memory Exception", ee);
        }
        catch (Exception ee)
        {
            Page_Globals.tracer.Add_Trace("System.Web.UI.Page.Page_Load (ufdc.Page_Load)", "Exception caught!", SobekCM.Library.Custom_Trace_Type_Enum.Error);
            Page_Globals.tracer.Add_Trace("System.Web.UI.Page.Page_Load (ufdc.Page_Load)", ee.Message, SobekCM.Library.Custom_Trace_Type_Enum.Error);
            Page_Globals.tracer.Add_Trace("System.Web.UI.Page.Page_Load (ufdc.Page_Load)", ee.StackTrace, SobekCM.Library.Custom_Trace_Type_Enum.Error);

            Page_Globals.currentMode.Mode = Display_Mode_Enum.Error;
            Page_Globals.currentMode.Error_Message = "Unknown error caught while executing your request";
            Page_Globals.currentMode.Caught_Exception = ee;
        }
    }


    #endregion

    #region Methods called during execution of the HTML from UFDC.aspx

    public void Add_Body_Attributes()
    {
        if ((Page_Globals.currentMode.isPostBack) && ((Page_Globals.currentMode == null) || ((Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Aggregation_Browse_Info) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Results))))
            return;

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Response.Output.Write(((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Get_Body_Attributes(Page_Globals.tracer));
        }
    }

    public void Set_Page_Title()
    {
        if ((Page_Globals.currentMode.isPostBack) && ((Page_Globals.currentMode == null) || ((Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Aggregation_Browse_Info) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Results))))
            return;

        if (Page_Globals.mainWriter == null)
        {
            Page_Globals.Set_Main_Writer();
        }

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Response.Output.Write(((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Get_Page_Title(Page_Globals.tracer));
        }

        // For robot crawlers using the HTML ECHO writer, the title is alway in the info browse mode
        if (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            Response.Output.Write( Page_Globals.currentMode.Info_Browse_Mode );
        }
    }

    public void Add_Style_References()
    {
        //Response.Output.WriteLine("
        //Response.Output.WriteLine("    <style type=\"text/css\" media=\"screen\">");
        //Response.Output.WriteLine("        @import url( " + Global.Application_Settings.Base_URL + "default/SobekCM.css );");
        //Response.Output.WriteLine("    </style>");

        // This statement returns if this is a postback and EITHER: current mode is null or this is not a My UFDC return value
        // My UFDC takes advantage of a lot of postbacks with the editing and self-submittal form, so the styles and
        // scripts SHOULD be added in that case.  And if the current mode is null for some reason (error) then there
        // is no main writer to write the style reference.

        if ((Page_Globals.currentMode.isPostBack) && ((Page_Globals.currentMode == null) || ((Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Aggregation_Browse_Info) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Results))))
            return;

        // Only bother writing the style references if this is writing HTML (either logged out or logged in via myUFDC)
        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Add_Style_References(Response.Output, Page_Globals.tracer);
        }

        // If this is for the robots, add some generic style statements
        if (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            ((SobekCM.Library.MainWriters.Html_Echo_MainWriter)Page_Globals.mainWriter).Add_Style_References(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Add_Html()
    {

        //Response.Output.WriteLine("128.227.58.79 &nbsp; &nbsp; &nbsp; " + Global.IP_Restrictions.Restrictive_Range_Membership("128.227.58.79") + "<br />");
        //Response.Output.WriteLine("128.227.103.116 &nbsp; &nbsp; &nbsp; " + Global.IP_Restrictions.Restrictive_Range_Membership("128.227.103.116") + "<br />");
        //Response.Output.WriteLine("128.227.83.43 &nbsp; &nbsp; &nbsp; " + Global.IP_Restrictions.Restrictive_Range_Membership("128.227.83.43") + "<br />");



        if ((Page_Globals.currentMode.isPostBack) && ((Page_Globals.currentMode == null) || ((Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Aggregation_Browse_Info) && (Page_Globals.currentMode.Mode != Display_Mode_Enum.Results))))
            return;

        // Add the HTML and controls to start this off
        Page_Globals.mainWriter.Add_Text_To_Page(Response.Output, Page_Globals.tracer);
    }

    protected void Add_Additional_HTML_Upload_Form()
    {
        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("ufdc.Add_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Add_Additional_HTML(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Add_Additional_HTML()
    {
        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("ufdc.Add_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Add_Additional_HTML(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Add_Final_HTML()
    {
        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("ufdc.Add_Final_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Add_Final_HTML(Response.Output, Page_Globals.tracer);
        }
    }


    protected override void OnUnload(EventArgs e)
    {
        if ( HttpContext.Current.Session["Last_Exception"] == null )
            SobekCM.Library.Database.SobekCM_Database.Verify_Item_Lookup_Object(true, ref Global.Item_List, null);

        base.OnUnload(e);
    }

    #endregion



    protected override void OnInit(EventArgs e)
    {
        Page_Globals = new SobekCM_Page_Globals(IsPostBack, "SOBEKCM");

        base.OnInit(e);
    }

    protected void Repository_Title()
    {
        if ( !String.IsNullOrEmpty( SobekCM.Library.SobekCM_Library_Settings.System_Name))
            Response.Output.Write(SobekCM.Library.SobekCM_Library_Settings.System_Name + " : SobekCM Digital Repository");
        else
            Response.Output.Write("SobekCM Digital Repository");
    }
}

