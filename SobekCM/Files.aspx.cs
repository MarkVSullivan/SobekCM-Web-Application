using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Library;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;

public partial class Files : System.Web.UI.Page
{
    protected string file_url;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Pull out the http request
        HttpRequest request = HttpContext.Current.Request;

        // Get the base url
        string base_url = request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "");
        if (base_url.IndexOf("?") > 0)
            base_url = base_url.Substring(0, base_url.IndexOf("?"));

        if (String.IsNullOrEmpty(SobekCM_Database.Connection_String))
        {
            Custom_Tracer tracer = new Custom_Tracer();
            try
            {

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
            catch (Exception ee)
            {
                // Send to the dashboard
                if ((HttpContext.Current.Request.UserHostAddress == "127.0.0.1") || (HttpContext.Current.Request.UserHostAddress == HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]) || (HttpContext.Current.Request.Url.ToString().IndexOf("localhost") >= 0))
                {
                    // Create an error message 
                    string errorMessage = "Error caught while validating application state";
                    if (String.IsNullOrEmpty(SobekCM_Library_Settings.Database_Connection_String))
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
        }


        string BibID = null;
        string VID = null;

        // Get any url rewrite which occurred
        if (Request.QueryString["urlrelative"] != null)
        {
            string urlrewrite = Request.QueryString["urlrelative"].ToLower();
            if (urlrewrite.Length > 4)
            {
                // Split the url relative list
                string[] url_relative_info = urlrewrite.Split("/".ToCharArray());
                List<string> url_relative_list = (from thisPart in url_relative_info where thisPart.Length > 0 select thisPart.ToLower()).ToList();

                // Now, look for BIBID and VID
                //if ((SobekCM_Database.Verify_Item_Lookup_Object(true, ref Global.Item_List, null)) && (Global.Item_List.Contains_BibID(url_relative_list[2].ToUpper())))
                if ( url_relative_list[2].Length == 10 )
                {
                    // This is a BibID for an existing title with at least one public item
                    BibID = url_relative_list[2].ToUpper();

                    // Is the next part a VID?
                    string possible_vid = url_relative_list[3].Trim().PadLeft(5, '0');
                    //if (Global.Item_List.Contains_BibID_VID(BibID, possible_vid))
                    //{
                    int vid_as_int = -1;
                    if (Int32.TryParse(possible_vid, out vid_as_int))
                        VID = possible_vid;
                    //}

                }

                // Only continue if there is a BibID / VID
                if ((!String.IsNullOrEmpty(BibID)) && (!String.IsNullOrEmpty(VID)))
                {
                    // Determine the new URL
                    StringBuilder urlBuilder = new StringBuilder(SobekCM_Library_Settings.Image_Server_Network + BibID.Substring(0, 2) + "\\" + BibID.Substring(2, 2) + "\\" + BibID.Substring(4, 2) + "\\" + BibID.Substring(6, 2) + "\\" + BibID.Substring(8) + "\\" + VID + "\\" + url_relative_list[4], 250);
                    for (int i = 5; i < url_relative_list.Count; i ++)
                    {
                        urlBuilder.Append("\\" + url_relative_list[i]);
                    }

                    file_url = urlBuilder.ToString();


                    // Get the extension
                    string Extension = Path.GetExtension(file_url);
                    if (Extension != null)
                    {
                        // Lookup the MIME type by extension
                        Mime_Type_Info MimeType = null;
                        if (Global.Mime_Types.ContainsKey(Extension.ToLower()))
                            MimeType = Global.Mime_Types[Extension.ToLower()];

                        if ((MimeType != null) && (!MimeType.isBlocked))
                        {
                            // Should this be forwarded for this mimetype?
                            if (MimeType.shouldForward)
                            {
                                StringBuilder forwardBuilder = new StringBuilder(SobekCM_Library_Settings.Image_URL + BibID.Substring(0, 2) + "/" + BibID.Substring(2, 2) + "/" + BibID.Substring(4, 2) + "/" + BibID.Substring(6, 2) + "/" + BibID.Substring(8) + "/" + VID + "/" + url_relative_list[4], 250);
                                for (int i = 5; i < url_relative_list.Count; i++)
                                {
                                    forwardBuilder.Append("/" + url_relative_list[i]);
                                }
                                Response.Redirect(forwardBuilder.ToString());
                            }
                            else
                            {
                                Response.Clear();
                                Response.ContentType = MimeType.MIME_Type;

                                string filename = file_url;

                                if (File.Exists(filename))
                                {
                                    using (FileStream SourceStream = File.OpenRead(filename))
                                    {
                                        SourceStream.CopyTo(Response.OutputStream, 32768);
                                    }
                                }

                                Response.End();
                            }
                        }
                    }
                }
            }
        }

        //public static async Task CopyToAsync(this Stream source, Stream destination)
        //{
        //    int i = 0;
        //    var buffers = new [] { new byte[0x1000], new byte[0x1000] };
        //    Task writeTask = null;
        //    while(true)
        //    {
        //        var readTask = source.ReadAsync(buffers[i], 0, buffers[i].Length))>0;
        //        if (writeTask != null) await Task.WhenAll(readTask, writeTask);
        //        int bytesRead = await readTask;
        //        if (bytesRead == 0) break;
        //        writeTask = destination.WriteAsync(buffers[i], 0, bytesRead);
        //        i ^= 1; // swap buffers
        //    }
        //}

    }
}