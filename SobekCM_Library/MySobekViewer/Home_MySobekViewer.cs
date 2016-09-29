// HTML5 10/30/2013

#region Using directives

using System;
using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated RequestSpecificValues.Current_User to view their home page, with all their options in a menu  </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the RequestSpecificValues.Current_User's home page </li>
    /// </ul></remarks>
    public class Home_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the Home_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Home_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Home_MySobekViewer.Constructor", String.Empty);
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> The value of this message changes, depending on if this is the RequestSpecificValues.Current_User's first time here.  It is always a welcoming message though </value>
        public override string Web_Title
        {
            get
            {
                if (RequestSpecificValues.Current_User.Is_Just_Registered)
                {
                    return (RequestSpecificValues.Current_User.Nickname.Length > 0) ? "Welcome to my" + RequestSpecificValues.Current_Mode.Instance_Abbreviation + ", " + RequestSpecificValues.Current_User.Nickname : "Welcome to my" + RequestSpecificValues.Current_Mode.Instance_Abbreviation + ", " + RequestSpecificValues.Current_User.Given_Name;
                }

                return (RequestSpecificValues.Current_User.Nickname.Length > 0) ? "Welcome Back, " + RequestSpecificValues.Current_User.Nickname : "Welcome Back, " + RequestSpecificValues.Current_User.Given_Name;
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Home_MySobekViewer.Write_HTML", String.Empty);

            string sobek_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation;
            string my_sobek = "my" + sobek_text;

			Output.WriteLine("<h1>" + Web_Title + "</h1>");
			Output.WriteLine();

			Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
            Output.WriteLine("  <p>Welcome to " + my_sobek + ".  This feature allows you to add items to your bookshelves, organize your bookshelves, and email your bookshelves to friends.</p>");
			Output.WriteLine();
            Output.WriteLine("  <div id=\"sbkHmv_WhatWouldDiv\">What would you like to do today?</div>");
			Output.WriteLine();
            Output.WriteLine("  <table id=\"sbkHmv_Table\">");

            // If a RequestSpecificValues.Current_User can submit, add a link to start a new item
            if (RequestSpecificValues.Current_User.Can_Submit)
            {
                if (UI_ApplicationCache_Gateway.Settings.Resources.Online_Item_Submit_Enabled)
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                    Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.New_Item_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" >Start a new item</a></td></tr>");

                    // Check to see if the TEI extension should be added here
                    // Ensure the plug-in list exists and contains the TEI plug-in
                    if ((UI_ApplicationCache_Gateway.Configuration.Extensions != null) &&
                        (UI_ApplicationCache_Gateway.Configuration.Extensions.Get_Extension("TEI") != null) &&
                        (UI_ApplicationCache_Gateway.Configuration.Extensions.Get_Extension("TEI").Enabled))
                    {
                        // Ensure the user is TEI enabled
                        string user_tei_enabled = RequestSpecificValues.Current_User.Get_Setting("TEI.Enabled", "false");
                        if (String.Compare(user_tei_enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_TEI_Item;
                            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                            Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"http://cdn.sobekrepository.org/images/misc/add_tei.png\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" >Add a new TEI item</a></td></tr>");
                        }
                    }

                }
                else
                {
					Output.WriteLine("    <tr><td style=\"width:35px\"><img src=\"" + Static_Resources_Gateway.New_Item_Img + "\" /></td><td><span style=\"color:gray\"><i>Online submittals are temporarily disabled</i></span></td></tr>");
                }
            }

            // If the RequestSpecificValues.Current_User has already submitted stuff, add a link to all submitted items (otherwise a grayed out link)
            if (RequestSpecificValues.Current_User.Items_Submitted_Count > 0)
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Submitted Items";
				Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Submitted_Items_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">View all my submitted items</a></td></tr>");

            }
            else
            {
                if (RequestSpecificValues.Current_User.Can_Submit)
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Submitted Items";
					Output.WriteLine("    <tr><td style=\"width:35px\"><img src=\"" + Static_Resources_Gateway.Submitted_Items_Gif + "\" /></td><td><span style=\"color:Gray\">View all my submitted items</span></td></tr>");
                }
            }

            // If this RequestSpecificValues.Current_User is linked to item statistics, add that link as well
            if (RequestSpecificValues.Current_User.Has_Item_Stats)
            {
                // Add link to folder management
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.User_Usage_Stats;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Usage_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">View usage for my items</a></td></tr>");
            }

            // If the RequestSpecificValues.Current_User has submitted some descriptive tags, or has the kind of rights that let them
            // view lists of tags, add that
            if ((RequestSpecificValues.Current_User.Has_Descriptive_Tags) || (RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_A_Collection_Manager_Or_Admin))
            {
                // Add link to folder management
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Chat_Png + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">View my descriptive tags</a></td></tr>");
            }

            // Add link to folder management
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Bookshelf_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">View and organize my bookshelves</a></td></tr>");

            // Add a link to view all saved searches
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Saved_Searches_Img + "\"border=\"0px\"  /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">View my saved searches</a></td></tr>");

            // Add a link to edit your preferences
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Settings_Img + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit my account preferences</a></td></tr>");

            //If the RequestSpecificValues.Current_User is a scanning/processing technician, add a link for item tracking here
            RequestSpecificValues.Current_Mode.My_Sobek_Type=My_Sobek_Type_Enum.Item_Tracking;
            Output.WriteLine("<tr><td style=\"width:35px\"><a href=\""+UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode)+"\"><img src=\""+ Static_Resources_Gateway.Track2_Gif + "\"/></a></td><td><a href=\""+UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode)+"\">Track Item Scanning/Processing</a></td></tr>");

            // If a return URL was provided, add a link to return there
            if ((!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Return_URL)) && ( RequestSpecificValues.Current_Mode.Return_URL.IndexOf("my") < 0 ))
            {
                Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + RequestSpecificValues.Current_Mode.Return_URL + "\"><img src=\"" + Static_Resources_Gateway.Return_Img + "\" /></a></td><td><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + RequestSpecificValues.Current_Mode.Return_URL + "\">Return to previous " + sobek_text + " page</a></td></tr>");
            }
            
            // Add a log out link
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources_Gateway.Exit_Gif + "\" /></a></td><td><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Log Out</a></td></tr>");
            Output.WriteLine("  </table>");
			Output.WriteLine();
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;

            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Contact;
	        Output.WriteLine("  <p>Comments or recommendations?  Please <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">contact us</a>.</p>");
            if (!RequestSpecificValues.Current_User.Can_Submit)
            {
				Output.WriteLine("  <p>If you would like to contribute materials through the online system, please <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">contact us</a> as well.</p>");
            }

			Output.WriteLine("  <br />");

            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
            Output.WriteLine("</div>");
			Output.WriteLine();
        }
    }
}
