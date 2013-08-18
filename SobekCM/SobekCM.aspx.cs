#region Includes 

using System;
using System.Web;
using SobekCM.Library.Settings;
using SobekCM.Library.Navigation;

#endregion

public partial class SobekMain : System.Web.UI.Page
{
    private SobekCM_Page_Globals Page_Globals;

    #region Page_Load method does the final checks and creates the writer type

    protected void Page_Load(object sender, EventArgs e)
    {
        Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", String.Empty);

        try
        {
            // Process this page request by building the main writer and 
            // analyzing the request's URL
            Page_Globals.On_Page_Load();

            // Is the response completed already?
            if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            {
                fileUploadForm.Visible = false;
                return;
            }


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

            if (SobekCM_Library_Settings.Web_Output_Caching_Minutes > 0)
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
                    Response.Cache.SetMaxAge(new TimeSpan(0, SobekCM_Library_Settings.Web_Output_Caching_Minutes, 0));
                }
                else
                {
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
            }

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
            }

            // The file upload form is only shown in these cases
            if ((Page_Globals.currentMode.Mode != Display_Mode_Enum.My_Sobek) ||
                ((Page_Globals.currentMode.Writer_Type != Writer_Type_Enum.HTML) && (Page_Globals.currentMode.Writer_Type != Writer_Type_Enum.HTML_LoggedIn)) ||
                (((Page_Globals.currentMode.My_Sobek_Type != My_Sobek_Type_Enum.New_Item) || (Page_Globals.currentMode.My_Sobek_SubMode.Length == 0) || (Page_Globals.currentMode.My_Sobek_SubMode[0] != '8')) && (Page_Globals.currentMode.My_Sobek_Type != My_Sobek_Type_Enum.File_Management) && (Page_Globals.currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Page_Images_Management)) ||
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
            Page_Globals.mainWriter.Add_Controls(tocPlaceHolder, mainPlaceHolder, myUfdcUploadPlaceHolder, Page_Globals.tracer);
        }
        catch (OutOfMemoryException ee)
        {
            fileUploadForm.Visible = false;
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", "OutOfMemoryException caught!");

            Page_Globals.Email_Information("SobekCM Out of Memory Exception", ee);
        }
        catch (Exception ee)
        {
            fileUploadForm.Visible = false;

            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", "Exception caught!", SobekCM.Library.Custom_Trace_Type_Enum.Error);
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", ee.Message, SobekCM.Library.Custom_Trace_Type_Enum.Error);
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", ee.StackTrace, SobekCM.Library.Custom_Trace_Type_Enum.Error);

            Page_Globals.currentMode.Mode = Display_Mode_Enum.Error;
            Page_Globals.currentMode.Error_Message = "Unknown error caught while executing your request";
            Page_Globals.currentMode.Caught_Exception = ee;
        }
    }


    #endregion

    #region Methods called during execution of the HTML from UFDC.aspx

    protected void Write_Page_Title()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed ))
            return;

        Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Page_Title", String.Empty);

        // Allow the html writer to add its own title 
        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Response.Output.Write(((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Get_Page_Title(Page_Globals.tracer));
        }

        // For robot crawlers using the HTML ECHO writer, the title is alway in the info browse mode
        if (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            Response.Output.Write(Page_Globals.currentMode.Info_Browse_Mode);
        }
    }

    protected void Write_Within_HTML_Head()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Within_HTML_Head", String.Empty);

        // Only bother writing the style references if this is writing HTML (either logged out or logged in via myUFDC)
        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Write_Within_HTML_Head(Response.Output, Page_Globals.tracer);
        }

        // If this is for the robots, add some generic style statements
        if (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            ((SobekCM.Library.MainWriters.Html_Echo_MainWriter)Page_Globals.mainWriter).Write_Within_HTML_Head(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Write_Body_Attributes()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Body_Attributes", String.Empty);


        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Response.Output.Write(((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Get_Body_Attributes(Page_Globals.tracer));
        }
    }

    protected void Write_Html()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        // Add the HTML to the main section (which sits outside any of the standard fors)
        Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_HTML", String.Empty);
        Page_Globals.mainWriter.Write_Html(Response.Output, Page_Globals.tracer);
    }

    protected void Write_UploadForm_Additional_HTML()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML_Upload_Form", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Write_Additional_HTML(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Write_ItemNavForm_Opening()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Write_ItemNavForm_Opening(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Write_ItemNavForm_Additional_HTML()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Write_Additional_HTML(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Write_ItemNavForm_Closing()
    {


        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Write_ItemNavForm_Closing(Response.Output, Page_Globals.tracer);
        }
    }

    protected void Write_Final_HTML()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        if ((Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (Page_Globals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Page_Globals.tracer.Add_Trace("sobekcm(.aspx).Write_Final_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)Page_Globals.mainWriter).Write_Final_HTML(Response.Output, Page_Globals.tracer);
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
        // If the was a very basic error, or the request was complete, do nothing here
        if ((Page_Globals.currentMode == null) || (Page_Globals.currentMode.Request_Completed))
            return;

        if ( !String.IsNullOrEmpty( SobekCM_Library_Settings.System_Name))
            Response.Output.Write(SobekCM_Library_Settings.System_Name + " : SobekCM Digital Repository");
        else
            Response.Output.Write("SobekCM Digital Repository");
    }
}

