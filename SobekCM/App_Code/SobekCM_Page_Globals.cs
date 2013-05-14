#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.Items;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.SiteMap;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;
using SobekCM.Library.WebContent;

#endregion

public class SobekCM_Page_Globals
{
    #region Private class members

    public string browse_info_display_text;
    public SobekCM_Item currentItem;
    public SobekCM_Navigation_Object currentMode;
    public Page_TreeNode currentPage;
    public User_Object currentUser;
    public Item_Aggregation hierarchyObject;
    public SobekCM_Skin_Object htmlSkin;
    public SobekCM_Items_In_Title itemsInTitle;
    public abstractMainWriter mainWriter;
    public List<iSearch_Title_Result> pagedSearchResults;
    public Public_User_Folder publicFolder;
    public Search_Results_Statistics searchResultStatistics;
    public SobekCM_SiteMap siteMap;
    public HTML_Based_Content staticWebContent;
    public Item_Aggregation_Browse_Info thisBrowseObject;
    public Custom_Tracer tracer;

    #endregion

    #region Constructor for this class 

    public SobekCM_Page_Globals(bool isPostBack, string page_name)
    {
        // Pull out the http request
        HttpRequest request = HttpContext.Current.Request;

        // Get the base url
        string base_url = request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "");
        if (base_url.IndexOf("?") > 0)
            base_url = base_url.Substring(0, base_url.IndexOf("?"));

        try
        {
            tracer = new Custom_Tracer();
            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", String.Empty);

            // Don't really need to *build* these, so just define them as a new ones if null
            if (Global.Checked_List == null)
                Global.Checked_List = new Checked_Out_Items_List();
            if (Global.Search_History == null)
                Global.Search_History = new Recent_Searches();

            // Make sure all the needed data is loaded into the Application State
            Application_State_Builder.Build_Application_State(tracer, false, ref Global.Skins, ref Global.Translation,
                                                              ref Global.Codes, ref Global.Item_List, ref Global.Icon_List,
                                                              ref Global.Stats_Date_Range, ref Global.Thematic_Headings, ref Global.Collection_Aliases, ref Global.IP_Restrictions,
                                                              ref Global.URL_Portals, ref Global.Mime_Types);

            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Application State validated or built");

            // Check that something is saved for the original requested URL (may not exist if not forwarded)
            if (!HttpContext.Current.Items.Contains("Original_URL"))
                HttpContext.Current.Items["Original_URL"] = request.Url.ToString();
        }
        catch ( Exception ee )
        {
            // Send to the dashboard
            if ((HttpContext.Current.Request.UserHostAddress == "127.0.0.1")  || ( HttpContext.Current.Request.UserHostAddress == HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"] ) || (HttpContext.Current.Request.Url.ToString().IndexOf("localhost") >= 0))
            {
                // Create an error message 
                string errorMessage = "Error caught while validating application state";
                if ( String.IsNullOrEmpty(SobekCM_Library_Settings.Database_Connection_String ))
                {
                    errorMessage = "No database connection string found!";
                    string configFileLocation = AppDomain.CurrentDomain.BaseDirectory + "config/sobekcm.xml";
                    try
                    {
                        if (!File.Exists(configFileLocation))
                        {
                            errorMessage = "Missing config/sobekcm.xml configuration file on the web server.<br />Ensure the configuration file 'sobekcm.xml' exists in a 'config' subfolder directly under the web application.<br />Example configuration is:" +
                           "<div style=\"background-color: #bbbbbb; margin-left: 30px; margin-top:10px; padding: 3px;\">&lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot;  ?&gt;<br /> &lt;configuration&gt;<br /> &nbsp; &nbsp &lt;connection_string type=&quot;MSSQL&quot;&gt;data source=localhost\\instance;initial catalog=SobekCM;integrated security=Yes;&lt;/connection_string&gt;<br /> &nbsp; &nbsp &lt;error_emails&gt;marsull@uflib.ufl.edu&lt;/error_emails&gt;<br /> &nbsp; &nbsp &lt;error_page&gt;http://ufdc.ufl.edu/error.html&lt;/error_page&gt;<br />&lt;/configuration&gt;</div>";
                        }
                    }
                    catch
                    {
                        errorMessage = "No database connection string found.<br />Likely an error reading the configuration file due to permissions on the web server.<br />Ensure the configuration file 'sobekcm.xml' exists in a 'config' subfolder directly under the web application.<br />Example configuration is:" +
                            "<div style=\"background-color: #bbbbbb; margin-left: 30px; margin-top:10px; padding: 3px;\">&lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot;  ?&gt;<br /> &lt;configuration&gt;<br /> &nbsp; &nbsp &lt;connection_string type=&quot;MSSQL&quot;&gt;data source=localhost\\instance;initial catalog=SobekCM;integrated security=Yes;&lt;/connection_string&gt;<br /> &nbsp; &nbsp &lt;error_emails&gt;marsull@uflib.ufl.edu&lt;/error_emails&gt;<br /> &nbsp; &nbsp &lt;error_page&gt;http://ufdc.ufl.edu/error.html&lt;/error_page&gt;<br />&lt;/configuration&gt;</div>";
                    }
                }
                else
                {
                    if (ee.Message.IndexOf("The EXECUTE permission") >= 0)
                    {
                        errorMessage = "Permissions error while connecting to the database and pulling necessary data.<br /><br />Confirm the following:<ul><li>IIS is configured correctly to use anonymous authentication</li><li>Anonymous user (or service account) is part of the sobek_users role in the database.</li></ul>";
                    }
                    else
                    {
                        errorMessage = "Error connecting to the database and pulling necessary data.<br /><br />Confirm the following:<ul><li>Database connection string is correct ( " + SobekCM_Library_Settings.Database_Connection_String + ")</li><li>IIS is configured correctly to use anonymous authentication</li><li>Anonymous user (or service account) is part of the sobek_users role in the database.</li></ul>";
                    }
                }
                // Wrap this into the SobekCM Exception
                SobekCM_Traced_Exception newException = new SobekCM_Traced_Exception(errorMessage, ee, tracer);

                // Save this to the session state, and then forward to the dashboard
                HttpContext.Current.Session["Last_Exception"] = newException;
                HttpContext.Current.Response.Redirect("dashboard.aspx", true);
            }
            else
            {
                throw ee;
            }
        }

        // Analyze the response and get the mode
        try
        {
            currentMode = new SobekCM_Navigation_Object(request.QueryString, base_url, request.UserLanguages, Global.Codes, Global.Collection_Aliases, ref Global.Item_List, Global.URL_Portals, tracer)
                              {
                                  Base_URL = base_url,
                                  isPostBack = isPostBack,
                                  Browser_Type = request.Browser.Type.ToUpper()
                              };
            currentMode.Set_Robot_Flag(request.UserAgent, request.UserHostAddress);
        }
        catch
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", base_url);
            HttpContext.Current.Response.End();
        }

        //// Perform some redirects
        //if ((currentMode.Writer_Type == Writer_Type_Enum.OAI) && ( page_name != "SOBEKCM_OAI"))
        //{
        //    HttpContext.Current.Response.Redirect("sobekcm_oai.aspx?" + request.QueryString);
        //}
        //if (((currentMode.Writer_Type == Writer_Type_Enum.XML) || (currentMode.Writer_Type == Writer_Type_Enum.DataSet) || (currentMode.Writer_Type == Writer_Type_Enum.Text) || ( currentMode.Writer_Type == Writer_Type_Enum.JSON)) && ( page_name != "SOBEKCM_DATA" ))
        //{
        //    HttpContext.Current.Response.Redirect("sobekcm_data.aspx?" + request.QueryString);
        //}

        // If this was for HTML, but was at the data, just convert to XML 
        if (( page_name == "SOBEKCM_DATA" ) && (currentMode.Writer_Type != Writer_Type_Enum.XML) && (currentMode.Writer_Type != Writer_Type_Enum.JSON) && (currentMode.Writer_Type != Writer_Type_Enum.DataSet))
            currentMode.Writer_Type = Writer_Type_Enum.XML;


        tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Navigation Object created from URI query string");

       
        // Need to ensure the list of items was pulled for several modes
        if ((( currentMode.Writer_Type != Writer_Type_Enum.HTML ) && ( currentMode.Writer_Type != Writer_Type_Enum.HTML_Echo ) && ( currentMode.Writer_Type != Writer_Type_Enum.HTML_LoggedIn )) ||
            (currentMode.Mode == Display_Mode_Enum.Item_Display) ||
            (currentMode.Mode == Display_Mode_Enum.Item_Print) ||
            (currentMode.Mode == Display_Mode_Enum.Results) ||
            (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info) ||
            (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Map) ||
            (currentMode.Mode == Display_Mode_Enum.Public_Folder) ||
            ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Delete_Item) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Behaviors) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Folder_Management) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Add_Volume) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_AutoFill_Volumes) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Mass_Update_Items) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item))))
        {
            SobekCM_Database.Verify_Item_Lookup_Object(true, ref Global.Item_List, tracer);
        }


        try
        {
            if (currentMode.Aggregation.ToUpper() == "EPCSANB")
            {
                HttpContext.Current.Response.Redirect(@"http://www.uflib.ufl.edu/epc/");
            }

            // If this was an error, redirect now
            if (currentMode.Mode == Display_Mode_Enum.Error)
            {
                return;
            }

            // All the user stuff can be skipped if this was from a robot
            if (!currentMode.Is_Robot)
            {
                // Determine which IP Ranges this IP address belongs to, if not already determined.
                if (HttpContext.Current.Session["IP_Range_Membership"] == null)
                {
                    int ip_mask = Global.IP_Restrictions.Restrictive_Range_Membership(request.UserHostAddress);
                    HttpContext.Current.Session["IP_Range_Membership"] = ip_mask;
                }

                // Set the Session TOC, if provided
                if (currentMode.TOC_Display != TOC_Display_Type_Enum.Undetermined)
                {
                    if (currentMode.TOC_Display == TOC_Display_Type_Enum.Hide)
                    {
                        HttpContext.Current.Session["Show TOC"] = false;
                    }
                    else
                    {
                        HttpContext.Current.Session["Show TOC"] = true;
                    }
                }

                // Only do any of the user stuff if this is from the main SobekCM page
                if (page_name == "SOBEKCM")
                {
                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Checking for logged on user by cookie or session");
                    perform_user_checks(isPostBack);
                }

                // If this is a system admin, they can run as a different user actually
                if ((currentUser != null) && (currentUser.Is_System_Admin) && (request.QueryString["userid"] != null))
                {
                    try
                    {
                        int userid = Convert.ToInt32(request.QueryString["userid"]);
                        User_Object mirroredUser = SobekCM_Database.Get_User(userid, tracer);
                        if (mirroredUser != null)
                        {
                            // Replace the user information in the session state
                            HttpContext.Current.Session["user"] = mirroredUser;
                            currentUser = mirroredUser;
                        }
                    }
                    catch (Exception)
                    {
                        // Nothing to do here.. shouldn't ever really be here..
                    }
                }

                // If this was a call for RESET, clear the memory
                if ((currentMode.Mode == Display_Mode_Enum.Administrative) && (currentMode.Admin_Type == Admin_Type_Enum.Reset))
                {
                    Reset_Memory();

                    // Since this reset, send to the admin, memory management portion
                    currentMode.Mode = Display_Mode_Enum.Internal;
                    currentMode.Internal_Type = Internal_Type_Enum.Cache;
                }
            }
            else // THIS IS A ROBOT REQUEST
            {
                Perform_Search_Engine_Robot_Checks(currentMode, request.QueryString);
            }

            // If this is for a public folder, get the data
            if (currentMode.Mode == Display_Mode_Enum.Public_Folder)
            {
                Public_Folder();
            }

            // Get the item now, so that you can set the collection code, if there was none listed
            if ((currentMode.Mode == Display_Mode_Enum.Item_Display) || (currentMode.Mode == Display_Mode_Enum.Item_Print) || ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && ((currentMode.My_Sobek_Type ==My_Sobek_Type_Enum.Edit_Item_Metadata ) || (currentMode.My_Sobek_Type ==My_Sobek_Type_Enum.Edit_Item_Behaviors )))
                || ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Behaviors) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Add_Volume) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_AutoFill_Volumes) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Mass_Update_Items) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management))))
            {
                Display_Item();
            }

            // Get the group, collection, or subcollection from the database or cache.
            // This also makes sure they are a proper hierarchy. 
            Get_Entire_Collection_Hierarchy();

            // Run the search if this should be done now
            if (currentMode.Mode == Display_Mode_Enum.Results)
            {
                Search_Block();
            }

            // Run the browse/info work if it is of those modes
            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info) 
            {
                Browse_Info_Block();
            }

            if (currentMode.Mode == Display_Mode_Enum.My_Sobek)
            {
                MySobekCM_Block();
            }

            // Run the simple text block if this is that mode
            if (currentMode.Mode == Display_Mode_Enum.Simple_HTML_CMS)
            {
                Simple_Web_Content_Text_Block();
            }
        }
        catch (OutOfMemoryException ee)
        {
            if (currentMode != null)
            {
                currentMode.Mode = Display_Mode_Enum.Error;
                currentMode.Error_Message = "Out of memory exception caught";
                currentMode.Caught_Exception = ee;
            }
            else
            {
                Email_Information("Fatal Out of memory exception caught", ee);
            }
        }
        catch (Exception ee)
        {
            if (currentMode != null)
            {
                currentMode.Mode = Display_Mode_Enum.Error;
                currentMode.Error_Message = "Unknown error occurred";
                currentMode.Caught_Exception = ee;
            }
            else
            {
                Email_Information("Unknown Fatal Error Occurred", ee);
            }
        }
    }

    #endregion

    #region Special checks for search engine robot URL behaviors

    private void Perform_Search_Engine_Robot_Checks(SobekCM_Navigation_Object currentModeCheck, NameValueCollection QueryString )
    {
        // Some writers should not be selected yet
        if ((currentModeCheck.Writer_Type != Writer_Type_Enum.HTML) && (currentModeCheck.Writer_Type != Writer_Type_Enum.HTML_Echo) && (currentModeCheck.Writer_Type != Writer_Type_Enum.OAI))
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Base_URL);
            HttpContext.Current.Response.End();
        }

        // There are some spots which robots are never allowed to go, just
        // by virtue of the fact they don't logon
        if ((currentModeCheck.Mode == Display_Mode_Enum.Internal) || (currentModeCheck.Mode == Display_Mode_Enum.My_Sobek) || (currentModeCheck.Mode == Display_Mode_Enum.Reset) || (currentModeCheck.Mode == Display_Mode_Enum.Item_Cache_Reload) || (currentModeCheck.Mode == Display_Mode_Enum.Results) || (currentModeCheck.Mode == Display_Mode_Enum.Public_Folder) || (currentModeCheck.Mode == Display_Mode_Enum.Aggregation_Browse_By) || (currentModeCheck.Mode == Display_Mode_Enum.Item_Print))
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Base_URL);
            HttpContext.Current.Response.End();
        }

        // Browse are okay, except when it is the NEW
        if ((currentModeCheck.Mode == Display_Mode_Enum.Aggregation_Browse_Info) && (currentModeCheck.Info_Browse_Mode == "new"))
        {
            currentModeCheck.Info_Browse_Mode = "all";

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
            HttpContext.Current.Response.End();
        }

        // Going to the search page is okay, except for ADVANCED searches ( results aren't okay, but going to the search page is okay )
        if ((currentModeCheck.Mode == Display_Mode_Enum.Search) && (currentModeCheck.Search_Type == Search_Type_Enum.Advanced))
        {
            currentModeCheck.Mode = Display_Mode_Enum.Aggregation_Home;
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
            HttpContext.Current.Response.End();
        }

        // If this was a legacy type request, forward to the new URL
        if ((QueryString["b"] != null) || (QueryString["m"] != null) || (QueryString["g"] != null) || (QueryString["c"] != null) || (QueryString["s"] != null) || (QueryString["a"] != null))
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
            HttpContext.Current.Response.End();
        }

        // Get the depth of the url relative 
        // Try to determine if this is a legacy type URL and how deep the urlrelative is
        int url_relative_depth = 0;
        string[] url_relative_info = null;
        if (QueryString["urlrelative"] != null)
        {
            string urlrewrite = QueryString["urlrelative"].ToLower();
            if (urlrewrite.Length > 0)
            {
                // Split the url relative list
                url_relative_info = urlrewrite.Split("/".ToCharArray());
                url_relative_depth = url_relative_info.Length;
            }
        }

        // For STATISTICS, handle some specific cases and enforce appropriate URLs
        if (currentModeCheck.Mode == Display_Mode_Enum.Statistics)
        {
            // Some submodes are off limites
            if ((currentModeCheck.Statistics_Type != Statistics_Type_Enum.Item_Count_Growth_View) && (currentModeCheck.Statistics_Type != Statistics_Type_Enum.Item_Count_Standard_View) && (currentModeCheck.Statistics_Type != Statistics_Type_Enum.Item_Count_Text) && (currentModeCheck.Statistics_Type != Statistics_Type_Enum.Usage_Definitions) && (currentModeCheck.Statistics_Type != Statistics_Type_Enum.Usage_Overall))
            {
                currentModeCheck.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                HttpContext.Current.Response.End();
            }

            // Ensure the URL behaved correctly
            switch (currentModeCheck.Statistics_Type)
            {
                case Statistics_Type_Enum.Item_Count_Text:
                case Statistics_Type_Enum.Item_Count_Growth_View:
                case Statistics_Type_Enum.Usage_Definitions:
                    if (url_relative_depth > 3)
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Status = "301 Moved Permanently";
                        HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                        HttpContext.Current.Response.End();
                    }
                    break;

                case Statistics_Type_Enum.Usage_Overall:
                    if (url_relative_depth > 2)
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Status = "301 Moved Permanently";
                        HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                        HttpContext.Current.Response.End();
                    }
                    break;

                case Statistics_Type_Enum.Item_Count_Standard_View:
                    if (url_relative_depth > 2)
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Status = "301 Moved Permanently";
                        HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                        HttpContext.Current.Response.End();
                    }
                    else if (url_relative_depth == 2)
                    {
                        if (url_relative_info[1] != "itemcount")
                        {
                            HttpContext.Current.Response.Clear();
                            HttpContext.Current.Response.Status = "301 Moved Permanently";
                            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                            HttpContext.Current.Response.End();
                        }
                    }
                    break;
            }
        }

        // For AGGREGATION HOME handle some cases
        if (currentModeCheck.Mode == Display_Mode_Enum.Aggregation_Home)
        {
            // Different code depending on if this is an aggregation or not
            if ((currentModeCheck.Aggregation.Length == 0) || (currentModeCheck.Aggregation == "all"))
            {
                switch (currentModeCheck.Home_Type)
                {
                    case Home_Type_Enum.List:
                        if (url_relative_depth > 0)
                        {
                            HttpContext.Current.Response.Clear();
                            HttpContext.Current.Response.Status = "301 Moved Permanently";
                            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                            HttpContext.Current.Response.End();
                        }
                        break;

                    case Home_Type_Enum.Descriptions:
                    case Home_Type_Enum.Tree_Collapsed:
                    case Home_Type_Enum.Partners_List:
                        if (url_relative_depth > 1)
                        {
                            HttpContext.Current.Response.Clear();
                            HttpContext.Current.Response.Status = "301 Moved Permanently";
                            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                            HttpContext.Current.Response.End();
                        }
                        break;

                    case Home_Type_Enum.Tree_Expanded:
                    case Home_Type_Enum.Partners_Thumbnails:
                        if (url_relative_depth > 2)
                        {
                            HttpContext.Current.Response.Clear();
                            HttpContext.Current.Response.Status = "301 Moved Permanently";
                            HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                            HttpContext.Current.Response.End();
                        }
                        break;

                    case Home_Type_Enum.Personalized:
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Status = "301 Moved Permanently";
                        HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                        HttpContext.Current.Response.End();
                        break;
                }
            }
        }

        // Ensure this is requesting the item without a viewercode and without extraneous information
        if (currentModeCheck.Mode == Display_Mode_Enum.Item_Display)
        {
            if ((currentModeCheck.ViewerCode.Length > 0) || (url_relative_depth > 2))
            {
                currentModeCheck.ViewerCode = String.Empty;

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", currentModeCheck.Redirect_URL());
                HttpContext.Current.Response.End();
            }
        }
    }

    #endregion

    #region Method performs user checks against session, cookie, database, etc..

    private void perform_user_checks( bool isPostBack )
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Perform_User_Checks", "In user checks portion");

        // If this is to log out of my sobekcm, clear user id and forward back to sobekcm
        if ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type ==My_Sobek_Type_Enum.Log_Out))
        {
            tracer.Add_Trace("SobekCM_Page_Globals.Perform_User_Checks", "User logged out");

            // Delete any user cookie
            HttpCookie userCookie = new HttpCookie("SobekUser");
            userCookie.Values["userid"] = String.Empty;
            userCookie.Values["security_hash"] = String.Empty;
            userCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(userCookie);

            // Delete from memory
            HttpContext.Current.Session["userid"] = 0;
            HttpContext.Current.Session["user"] = null;

            HttpContext.Current.Response.Redirect( currentMode.Base_URL + currentMode.Return_URL);
        }

        // If this is a responce from Shibboleth/Gatorlink, get the user information and register them if necessary
        string shibboleth_id = HttpContext.Current.Request.ServerVariables["HTTP_UFID"];
        if (shibboleth_id == null)
        {
            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "HTTP_UFID server variable NOT found");
        }
        else
            //if ((currentModeCheck.Mode == Display_Mode_Enum.My_Sobek) && (currentModeCheck.My_Sobek_Type ==My_Sobek_Type_Enum.Gatorlink_Landing) && (System.Web.HttpContext.Current.Session["user"] == null) && (ufid != null))
        {
            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "HTTP_UFID server variable found");
            // string ufid = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_UFID"].ToString();

            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "HTTP_UFID server variable = '" + shibboleth_id + "'");

            if (shibboleth_id.Length == 8)
            {
                tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Pulling from database by ufid");

                User_Object possible_user_by_shibboleth_id = SobekCM_Database.Get_User(shibboleth_id, tracer);

                // Check to see if we got a valid user back
                if (possible_user_by_shibboleth_id != null)
                {
                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Setting session user from ufid");
                    HttpContext.Current.Session["user"] = possible_user_by_shibboleth_id;
                }
                else
                {
                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "User from ufid was null.. adding user");

                    // First pull the data from the Shibboleth session
                    string email = String.Empty;
                    string lastname = String.Empty;
                    string firstname = String.Empty;
                    if (HttpContext.Current.Request.ServerVariables["HTTP_EPPN"] != null)
                        email = HttpContext.Current.Request.ServerVariables["HTTP_EPPN"];
                    if (HttpContext.Current.Request.ServerVariables["HTTP_SN"] != null)
                        lastname = HttpContext.Current.Request.ServerVariables["HTTP_SN"];
                    if (HttpContext.Current.Request.ServerVariables["HTTP_GIVENNAME"] != null)
                        firstname = HttpContext.Current.Request.ServerVariables["HTTP_GIVENNAME"];

                    // Now build the user object
                    User_Object newUser = new User_Object();
                    if ((HttpContext.Current.Request.ServerVariables["HTTP_PRIMARY-AFFILIATION"] != null) && (HttpContext.Current.Request.ServerVariables["HTTP_PRIMARY-AFFILIATION"].IndexOf("F") >= 0))
                        newUser.Can_Submit = true;
                    else
                        newUser.Can_Submit = false;
                    newUser.Send_Email_On_Submission = true;
                    newUser.Email = email;
                    newUser.Family_Name = lastname;
                    newUser.Given_Name = firstname;
                    newUser.Organization = "University of Florida";
                    newUser.UFID = shibboleth_id;
                    newUser.UserID = -1;
                    if (email.Length > 0)
                        newUser.UserName = email;
                    else
                        newUser.UserName = newUser.Family_Name + shibboleth_id;

                    // Set a random password
                    StringBuilder passwordBuilder = new StringBuilder();
                    Random randomGenerator = new Random(DateTime.Now.Millisecond);
                    for (int i = 0; i < 5; i++)
                    {
                        int randomNumber = randomGenerator.Next(97, 122);
                        passwordBuilder.Append((char)randomNumber);

                        int randomNumber2 = randomGenerator.Next(65, 90);
                        passwordBuilder.Append((char)randomNumber2);
                    }
                    string password = passwordBuilder.ToString();

                    // Now, save this user
                    SobekCM_Database.Save_User(newUser, password, tracer);

                    // Now, pull back out of the database
                    User_Object possible_user_by_ufid2 = SobekCM_Database.Get_User(shibboleth_id, tracer);
                    possible_user_by_ufid2.Is_Just_Registered = true;
                    HttpContext.Current.Session["user"] = possible_user_by_ufid2;
                }

                if (HttpContext.Current.Session["user"] != null)
                {
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type =My_Sobek_Type_Enum.Home;
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                    currentMode.Aggregation = String.Empty;
                }
                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());

            }
        }

        // If the user information is still missing , but the UfdcUser cookie exists, then pull 
        // the user information from the UfdcUser cookie in the current requests
        if ((HttpContext.Current.Session["user"] == null) && (HttpContext.Current.Request.Cookies["SobekUser"] != null))
        {
            string userid_string = HttpContext.Current.Request.Cookies["SobekUser"]["userid"];
            int userid = -1;

                bool valid_perhaps = userid_string.All(Char.IsNumber);
                if (valid_perhaps)
                    Int32.TryParse(userid_string, out userid);

            if (userid > 0)
            {
                User_Object possible_user = SobekCM_Database.Get_User(userid, tracer);
                if (possible_user != null)
                {
                    string cookie_security_hash = HttpContext.Current.Request.Cookies["SobekUser"]["security_hash"];
                    if (cookie_security_hash == possible_user.Security_Hash(HttpContext.Current.Request.UserHostAddress))
                    {
                        HttpContext.Current.Session["user"] = possible_user;
                    }
                    else
                    {
                        // Security hash did not match, so clear the cookie
                        HttpCookie userCookie = new HttpCookie("SobekUser");
                        userCookie.Values["userid"] = String.Empty;
                        userCookie.Values["security_hash"] = String.Empty;
                        userCookie.Expires = DateTime.Now.AddDays(-1);
                        HttpContext.Current.Response.Cookies.Add(userCookie);
                    }
                }
            }
        }

        // If this is not a post back, set the html writer code to 'l' or 'h' depending on whether logged on
        if (!isPostBack)
        {
            if (HttpContext.Current.Session["user"] != null)
            {
                if (currentMode.Writer_Type == Writer_Type_Enum.HTML)
                {
                    // If this is really a deprecated URL, don't try to forwaard
                    if (( currentMode.Mode != Display_Mode_Enum.Item_Display ) || (currentMode.BibID.Length > 0) || (currentMode.ItemID_DEPRECATED <= 0))
                    {
                        currentMode.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
                        HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                    }
                }
                else
                {
                    if ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type ==My_Sobek_Type_Enum.Logon))
                    {
                        currentMode.My_Sobek_Type =My_Sobek_Type_Enum.Home;
                    }
                }
            }
            else
            {
                if ((currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn) && (currentMode.My_Sobek_Type !=My_Sobek_Type_Enum.Logon) && (currentMode.My_Sobek_Type !=My_Sobek_Type_Enum.Preferences))
                {
                    switch (currentMode.Mode)
                    {
                        case Display_Mode_Enum.My_Sobek:
                            currentMode.My_Sobek_Type =My_Sobek_Type_Enum.Logon;
                            break;

                        case Display_Mode_Enum.Item_Display:
                            currentMode.Writer_Type = Writer_Type_Enum.HTML;
                            // If this is really a deprecated URL, don't try to forwaard
                            if (( currentMode.BibID.Length > 0 ) || (currentMode.ItemID_DEPRECATED <= 0))
                            {
                                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                            }
                            break;

                        default:
                            currentMode.Writer_Type = Writer_Type_Enum.HTML;
                            HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                            break;

                    }
                }

                // If this is requesting an internal page and there is no user, send to the logon page
                if (currentMode.Mode == Display_Mode_Enum.Internal)
                {
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type =My_Sobek_Type_Enum.Logon;
                }
            }
        }
        else // This IS a postback
        {
            // If this is a postback from the logon page being inserted in front of the INTERNAL pages,
            // then the postback request needs to be handled by the logon page
            if ((currentMode.Mode == Display_Mode_Enum.Internal) && ( HttpContext.Current.Session["user"] == null )) 
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type =My_Sobek_Type_Enum.Logon;
            }
        }

        //// Also, if this is a robot not doing postback and requests a search, forward to the collection home
        //if (currentModeCheck.Is_Robot)
        //{
        //    if (currentModeCheck.Mode == Display_Mode_Enum.Results)
        //    {
        //        currentModeCheck.Mode = Display_Mode_Enum.Home;
        //        currentModeCheck.Search_Terms.Clear();
        //        currentModeCheck.Search_String = String.Empty;
        //        System.Web.HttpContext.Current.Response.Redirect(currentModeCheck.Redirect_URL());
        //    }
        //}

        // Set the internal DLC flag
        if (HttpContext.Current.Session["user"] != null)
        {
            currentUser = (User_Object)HttpContext.Current.Session["user"];
            currentMode.Internal_User = currentUser.Is_Internal_User;

            // Check if this is an administrative task that the current user does not have access to
            if ((!currentUser.Is_System_Admin) && ( !currentUser.Is_Portal_Admin ) && ( currentMode.Mode == Display_Mode_Enum.Administrative ))
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
            }
        }
        else
        {
            if ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type !=My_Sobek_Type_Enum.Preferences))
            {
                currentMode.Logon_Required = true;
            }

            if ((currentMode.Mode == Display_Mode_Enum.Aggregation_Home) && (currentMode.Home_Type == Home_Type_Enum.Personalized))
                currentMode.Home_Type = Home_Type_Enum.List;
        }
    }

    #endregion

    #region Method called during Page Load

    public void On_Page_Load()
    {
        if (currentMode != null)
        {
            // If this is not a post back, log it
            if (!currentMode.isPostBack)
            {
                tracer.Add_Trace("SobekCM_Page_Globals.Constructor.On_Page_Load", String.Empty);
            }

            Set_Main_Writer();
        }
    }

    #endregion

    public void Set_Main_Writer()
    {
        // Load the html writer
        string current_skin_code = currentMode.Skin.ToUpper();
        if ((currentMode.Writer_Type == Writer_Type_Enum.HTML) || ( currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn ))
        {
            // Check if a different skin should be used if this is an item display
            if ((currentMode.Mode == Display_Mode_Enum.Item_Display) || (currentMode.Mode == Display_Mode_Enum.Item_Print))
            {
                if ((currentItem != null) && (currentItem.Behaviors.Web_Skin_Count > 0))
                {                    
                    if (!currentItem.Behaviors.Web_Skins.Contains(current_skin_code))
                    {
                        string new_skin_code = currentItem.Behaviors.Web_Skins[0];
                        if ( new_skin_code != "UFDC" )
                        {
                            current_skin_code = new_skin_code;
                        }
                    }
                }
            }

            // Check if a differente skin should be used if this is a collection display
            if ((hierarchyObject != null) && (hierarchyObject.Web_Skins.Count > 0))
            {
                if (!hierarchyObject.Web_Skins.Contains(current_skin_code.ToLower()))
                {
                    current_skin_code = hierarchyObject.Web_Skins[0];
                }
            }

            SobekCM_Assistant assistant = new SobekCM_Assistant();
            htmlSkin = assistant.Get_HTML_Skin(current_skin_code, currentMode, Global.Skins, tracer);

            mainWriter = new Html_MainWriter(currentMode, hierarchyObject, searchResultStatistics, pagedSearchResults, thisBrowseObject,
                currentItem, currentPage, htmlSkin, currentUser, Global.Translation, Global.Codes, 
                Global.Item_List, Global.Stats_Date_Range,
                Global.Search_History, Global.Icon_List, Global.Thematic_Headings, publicFolder, Global.Collection_Aliases, Global.Skins, Global.Checked_List,
                Global.IP_Restrictions, Global.URL_Portals, siteMap, itemsInTitle, staticWebContent);
        }

        // Load the OAI writer
        if (currentMode.Writer_Type == Writer_Type_Enum.OAI)
        {
            mainWriter = new Oai_MainWriter(HttpContext.Current.Request.QueryString, Global.Item_List );
        }

        // Load the DataSet writer
        if (currentMode.Writer_Type == Writer_Type_Enum.DataSet)
        {
            mainWriter = new Dataset_MainWriter(currentMode, hierarchyObject, searchResultStatistics, pagedSearchResults,thisBrowseObject,   currentItem, currentPage);
        }

        // Load the XML writer
        if (currentMode.Writer_Type == Writer_Type_Enum.XML)
        {
            mainWriter = new Xml_MainWriter(currentMode, hierarchyObject, searchResultStatistics, pagedSearchResults, thisBrowseObject, currentItem, currentPage);
        }

        // Load the JSON writer
        if (currentMode.Writer_Type == Writer_Type_Enum.JSON)
        {
            mainWriter = new Json_MainWriter(currentMode, hierarchyObject, searchResultStatistics, pagedSearchResults, thisBrowseObject,  currentItem, currentPage, Global.Item_List, SobekCM_Library_Settings.Image_URL);
        }

        // Load the HTML ECHO writer
        if (currentMode.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            mainWriter = new Html_Echo_MainWriter(currentMode, browse_info_display_text);
        }

        // Default to HTML
        if (mainWriter == null)
        {
            SobekCM_Assistant assistant = new SobekCM_Assistant();
            htmlSkin = assistant.Get_HTML_Skin(currentMode, Global.Skins, tracer);

            mainWriter = new Html_MainWriter(currentMode, hierarchyObject, searchResultStatistics, pagedSearchResults, thisBrowseObject,
                currentItem, currentPage, htmlSkin, currentUser, Global.Translation, Global.Codes, 
                Global.Item_List,
                Global.Stats_Date_Range, Global.Search_History, Global.Icon_List, Global.Thematic_Headings, publicFolder,
                Global.Collection_Aliases, Global.Skins, Global.Checked_List, Global.IP_Restrictions, Global.URL_Portals, siteMap, itemsInTitle, staticWebContent);
        }
    }

    #region Block for displaying a public folder

    private void Public_Folder()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Public_Folder", "Retrieving public folder information and browse");

        SobekCM_Assistant assistant = new SobekCM_Assistant();
        bool result = assistant.Get_Public_User_Folder(currentMode.FolderID, currentMode.Page, tracer, out publicFolder, out searchResultStatistics, out pagedSearchResults );

        if ((!result) || (!publicFolder.isPublic))
        {
            currentMode.Error_Message = "Invalid or private bookshelf";
            currentMode.Mode = Display_Mode_Enum.Error;
        }            
    }

    #endregion

    #region Block for displaying a single item

    private void Display_Item()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Display_Item", "Retrieving item or group information");

        //// If the current request is from a robot, forward them to the appropriate static page
        //if ((( currentModeCheck.Writer_Type == Writer_Type_Enum.HTML ) || ( currentModeCheck.Writer_Type == Writer_Type_Enum.HTML_LoggedIn )) && (currentModeCheck.Is_Robot))
        //{
        //    if ((currentModeCheck.BibID.Length >= 10) && (currentModeCheck.VID.Length >= 5))
        //    {
        //        string bibid= currentModeCheck.BibID;
        //        string vid = currentModeCheck.VID;
        //        System.Web.HttpContext.Current.Response.Redirect("http://www.uflib.ufl.edu/ufdc2/items/" + bibid.Substring(0,2) + "/" + bibid.Substring(2,2) + "/" + bibid.Substring(4,2) + "/" + bibid.Substring(6,2) + "/" + bibid.Substring(8,2) + "/" + bibid + "_" + vid + ".html" );
        //    }

        //    // Send them to the robots entrance page
        //    System.Web.HttpContext.Current.Response.Redirect("http://www.uflib.ufl.edu/ufdc2/robot_entrance.html");
        //}

        // Build the SobekCM assistant
        SobekCM_Assistant assistant = new SobekCM_Assistant();

        // If this is a robot, then get the text from the static page
        if ( currentMode.Is_Robot )
        {
            browse_info_display_text = assistant.Get_Item_Static_HTML(currentMode, Global.Item_List, tracer);
            currentMode.Writer_Type = Writer_Type_Enum.HTML_Echo;
        }
        else
        {
            if (!assistant.Get_Item(currentMode, Global.Item_List, SobekCM_Library_Settings.Image_URL,
                                    Global.Icon_List, currentUser, tracer, out currentItem, out currentPage, out itemsInTitle ))
            {
                if ((currentMode.Mode == Display_Mode_Enum.Legacy_URL) || (currentMode.Invalid_Item))
                {
                    if (currentMode.Mode != Display_Mode_Enum.Legacy_URL)
                    {
                        currentMode.Mode = Display_Mode_Enum.Error;
                        currentMode.Error_Message = "Invalid Item Requested";
                    }
                }
                else
                {
                    Email_Information("Unable to find metadata for valid item", null);
                    currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                    HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                }
            }
        }


    }

    #endregion

    #region Block for displaying simple text with an interface

    private void Simple_Web_Content_Text_Block()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Simple_Web_Content_Text_Block", "Retrieiving Simple Web Content Object");

        SobekCM_Assistant assistant = new SobekCM_Assistant();
        if (!assistant.Get_Simple_Web_Content_Text(currentMode, SobekCM_Library_Settings.Base_Directory, tracer,
                                                   out staticWebContent, out siteMap))
        {
            currentMode.Mode = Display_Mode_Enum.Error;
            return;
        }

        // If the web skin is indicated in the browse file, set that
        if (staticWebContent.Web_Skin.Length > 0)
        {
            currentMode.Default_Skin = staticWebContent.Web_Skin;
            currentMode.Skin = staticWebContent.Web_Skin;
        }
    }

    #endregion

    #region Block for browsing and info

    private void Browse_Info_Block()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Browse_Info_Block", "Retrieiving Browse/Info Object");

        SobekCM_Assistant assistant = new SobekCM_Assistant();

        // If this is a robot, then get the text from the static page
        if ((currentMode.Is_Robot) && ( currentMode.Info_Browse_Mode == "all" ))
        {
            browse_info_display_text = assistant.Get_All_Browse_Static_HTML(currentMode, tracer);
            currentMode.Writer_Type = Writer_Type_Enum.HTML_Echo;
        }
        else
        {
            if (!assistant.Get_Browse_Info(currentMode, hierarchyObject, SobekCM_Library_Settings.Base_Directory, tracer,
                                           out thisBrowseObject, out searchResultStatistics, out pagedSearchResults, out staticWebContent))
            {
                currentMode.Mode = Display_Mode_Enum.Error;
            }

            //if ((currentModeCheck.Result_Display_Type == Result_Display_Type_Enum.Default) && (pagedSearchResults != null))
            //{
            //    currentModeCheck.Result_Display_Type = hierarchyObject.Default_Result_View;
            //}
        }
    }

    #endregion

    #region Block for searching

    private void Search_Block()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Search_Block", "Retreiving search results");

        try
        {
            // If there is no search term, forward back to the collection
            if ((currentMode.Search_String.Length == 0) && ( currentMode.Coordinates.Length == 0 ))
            {
                currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
                HttpContext.Current.Response.Redirect( currentMode.Redirect_URL() );
            }

            SobekCM_Assistant assistant = new SobekCM_Assistant();
            assistant.Get_Search_Results(currentMode, Global.Item_List, hierarchyObject,
                                         tracer, out searchResultStatistics, out pagedSearchResults);

            if ((!currentMode.isPostBack) && (Global.Search_History != null))
            {
                Global.Search_History.Add_New_Search(currentMode, HttpContext.Current.Request.UserHostAddress, currentMode.Search_Type, hierarchyObject.Name, currentMode.Search_String);
            }
        }
        catch (Exception ee)
        {
            currentMode.Mode = Display_Mode_Enum.Error;
            currentMode.Error_Message = "Unable to perform search at this time";
            currentMode.Caught_Exception = ee;
        }
    }

    #endregion

    #region Block for MySobek

    private void MySobekCM_Block()
    {
        if ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Folder_Management) && (HttpContext.Current.Session["user"] != null) && (currentMode.My_Sobek_SubMode.Length > 0))
        {
            tracer.Add_Trace("SobekCM_Page_Globals.MySobekCM_Block", "Retrieiving Browse/Info Object");

            User_Object userObj = (User_Object)HttpContext.Current.Session["user"];

            // For EXPORT option, include ALL the items
            int results_per_page = 20;
            int current_page = currentMode.Page;
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Export)
            {
                results_per_page = 10000;
                current_page = 1;
            }

            // Get the folder
            SobekCM_Assistant assistant = new SobekCM_Assistant();
            if (!assistant.Get_User_Folder(currentMode.My_Sobek_SubMode, userObj.UserID, results_per_page, current_page, tracer, out searchResultStatistics, out pagedSearchResults))
            {
                currentMode.Mode = Display_Mode_Enum.Error;
            }
        }
    }


    #endregion

    #region Methods retrieve groups, collections, or subcollections from the database or cache

    private void Get_Entire_Collection_Hierarchy()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Get_Entire_Collection_Hierarchy", "Retrieving hierarchy information");

        // Check that the current aggregation code is valid
        if (!Global.Codes.isValidCode(currentMode.Aggregation))
        {
            // Is there a "forward value"
            if (Global.Collection_Aliases.ContainsKey(currentMode.Aggregation))
            {
                currentMode.Aggregation = Global.Collection_Aliases[currentMode.Aggregation];
            }
        }

        SobekCM_Assistant assistant = new SobekCM_Assistant();
        if (!assistant.Get_Entire_Collection_Hierarchy(currentMode, Global.Codes, tracer, out hierarchyObject))
        {
            currentMode.Mode = Display_Mode_Enum.Error;
        }
    }

    #endregion

    #region Methods to reset the memory and the item cache

    private void Reset_Memory()
    {
        tracer.Add_Trace("SobekCM_Page_Globals.Reset_Memory", "Clearing cache and application of data");

        // Clear the cache
        Cached_Data_Manager.Clear_Cache();

        // Clear the application portions as well
        HttpContext.Current.Application.RemoveAll();

        // Refresh the application settings
        SobekCM_Library_Settings.Refresh(SobekCM_Database.Get_Settings_Complete(tracer));

        // Reload all the information
        Application_State_Builder.Build_Application_State(tracer, true, ref Global.Skins, ref Global.Translation,
                                                          ref Global.Codes, ref Global.Item_List, ref Global.Icon_List,
                                                          ref Global.Stats_Date_Range, ref Global.Thematic_Headings, ref Global.Collection_Aliases, ref Global.IP_Restrictions,
                                                          ref Global.URL_Portals, ref Global.Mime_Types  );

        // Since this reset, send to the admin, memory management portion
        currentMode.Mode = Display_Mode_Enum.Internal;
        currentMode.Internal_Type = Internal_Type_Enum.Cache;
    }

    #endregion

    #region Method to email information during an error

    public void Email_Information(string email_title, Exception objErr)
    {
        Email_Information(email_title, objErr, true);
    }

    public void Email_Information(string email_title, Exception objErr, bool redirect )
    {
        try
        {
            StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\temp\\exceptions.txt", true);
            writer.WriteLine();
            writer.WriteLine("Error logged in SobekCM_Page_Globals.Email_Information ( " + DateTime.Now.ToString() + ")");
            writer.WriteLine("User Host Address: " + HttpContext.Current.Request.UserHostAddress);
            writer.WriteLine("Requested URL: " + HttpContext.Current.Request.Url);
            if (objErr is SobekCM_Traced_Exception)
            {
                SobekCM_Traced_Exception sobekException = (SobekCM_Traced_Exception)objErr;

                writer.WriteLine("Error Message: " + sobekException.InnerException.Message);
                writer.WriteLine("Stack Trace: " + objErr.StackTrace);
                writer.WriteLine("Error Message:" + sobekException.InnerException.StackTrace);
                writer.WriteLine();
                writer.WriteLine(sobekException.Trace_Route);
            }
            else
            {

                writer.WriteLine("Error Message: " + objErr.Message);
                writer.WriteLine("Stack Trace: " + objErr.StackTrace);
            }

            writer.WriteLine();
            writer.WriteLine("------------------------------------------------------------------");
            writer.Flush();
            writer.Close();
        }
        catch (Exception)
        {
            // Already catching errors.. nothing else to realy do here if this causes an error as well
        }

        try
        {
            // Build the error message
            string err;
            if (objErr != null)
            {
                err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
                      "Error in: " + HttpContext.Current.Request.Url + "<br />" +
                      "Error Message: " + objErr.Message + "<br /><br />" +
                      "Stack Trace: " + objErr.StackTrace + "<br /><br />";

                if (objErr.Message.IndexOf("Timeout expired") >= 0)
                    email_title = "Database Timeout Expired";
            }
            else
            {
                err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
                      "Error in: " + HttpContext.Current.Request.Url + "<br />" +
                      "Error Message: " + email_title;
            }

            SobekCM_Database.Send_Database_Email(ConfigurationManager.AppSettings["Error_Emails"], email_title, err, true, false, -1);

        }
        catch (Exception)
        {
            // Already catching errors.. nothing else to realy do here if this causes an error as well
        }

        // Forward to our error message
        if (redirect)
        {
            HttpContext.Current.Response.Redirect(ConfigurationManager.AppSettings["Error_HTML_Page"]);
        }
    }

    private void send_error_email()
    {
        try
        {
            // Start the body
            StringBuilder builder = new StringBuilder();
            builder.Append("\n\nSUBMISSION INFORMATION\n");
            builder.Append("\tDate:\t\t\t\t" + DateTime.Now.ToString() + "\n");
            builder.Append("\tIP Address:\t\t\t" + HttpContext.Current.Request.UserHostAddress + "\n");
            builder.Append("\tHost Name:\t\t\t" + HttpContext.Current.Request.UserHostName + "\n");
            builder.Append("\tBrowser:\t\t\t" + HttpContext.Current.Request.Browser.Browser + "\n");
            builder.Append("\tBrowser Platform:\t\t" + HttpContext.Current.Request.Browser.Platform + "\n");
            builder.Append("\tBrowser Version:\t\t" + HttpContext.Current.Request.Browser.Version + "\n");
            builder.Append("\tBrowser Language:\t\t");
            bool first = true;
            string[] languages = HttpContext.Current.Request.UserLanguages;
            if (languages != null)
            {
                foreach (string thisLanguage in languages)
                {
                    if (first)
                    {
                        builder.Append(thisLanguage);
                        first = false;
                    }
                    else
                    {
                        builder.Append(", " + thisLanguage);
                    }
                }
            }

            builder.AppendLine("HISTORY");
            if (HttpContext.Current.Session["LastSearch"] != null)
                builder.AppendLine("\tLast Search:\t\t" + HttpContext.Current.Session["LastSearch"]);
            if (HttpContext.Current.Session["LastResults"] != null)
                builder.AppendLine("\tLast Results:\t\t" + HttpContext.Current.Session["LastResults"]);
            if (HttpContext.Current.Session["Last_Mode"] != null)
                builder.AppendLine("\tLast Mode:\t\t\t?" + HttpContext.Current.Session["Last_Mode"]);
            if ( HttpContext.Current.Items.Contains("Original_URL"))
                builder.AppendLine("\tURL:\t\t\t\t" + HttpContext.Current.Items["Original_URL"]);
            else
                builder.AppendLine("\tURL:\t\t\t\t" + HttpContext.Current.Request.Url);

            // Send this email
            try
            {
                SobekCM_Database.Send_Database_Email(ConfigurationManager.AppSettings["Error_Emails"], "SobekCM Exception Caught  [Invalid Item Requested]", builder.ToString(), false, false, -1);
            }
            catch (Exception)
            {
                // Already catching errors.. nothing else to realy do here if this causes an error as well
            }

        }
        catch (Exception)
        {
            // Already catching errors.. nothing else to realy do here if this causes an error as well
        }

        // Forward to our error message
        HttpContext.Current.Response.Redirect(ConfigurationManager.AppSettings["Error_HTML_Page"]);
    }

    #endregion
}
