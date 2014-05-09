// HTML5 10/30/2013

#region Using directives

using System;
using System.IO;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated user to view their home page, with all their options in a menu  </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the user's home page </li>
    /// </ul></remarks>
    public class Home_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the Home_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Home_MySobekViewer(User_Object User, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Home_MySobekViewer.Constructor", String.Empty);
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> The value of this message changes, depending on if this is the user's first time here.  It is always a welcoming message though </value>
        public override string Web_Title
        {
            get
            {
                if (user.Is_Just_Registered)
                {
                    return (user.Nickname.Length > 0) ? "Welcome to my" + currentMode.SobekCM_Instance_Abbreviation + ", " + user.Nickname : "Welcome to my" + currentMode.SobekCM_Instance_Abbreviation + ", " + user.Given_Name;
                }

                return (user.Nickname.Length > 0) ? "Welcome Back, " + user.Nickname : "Welcome Back, " + user.Given_Name;
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Home_MySobekViewer.Write_HTML", String.Empty);

            string sobek_text = currentMode.SobekCM_Instance_Abbreviation;
            string my_sobek = "my" + sobek_text;

			Output.WriteLine("<h1>" + Web_Title + "</h1>");
			Output.WriteLine();

			Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
            Output.WriteLine("  <p>Welcome to " + my_sobek + ".  This feature allows you to add items to your bookshelves, organize your bookshelves, and email your bookshelves to friends.</p>");
			Output.WriteLine();
            Output.WriteLine("  <div id=\"sbkHmv_WhatWouldDiv\">What would you like to do today?</div>");
			Output.WriteLine();
            Output.WriteLine("  <table id=\"sbkHmv_Table\">");

            // If a user can submit, add a link to start a new item
            if (user.Can_Submit)
            {
                if (SobekCM_Library_Settings.Online_Edit_Submit_Enabled)
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "new_item.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\" >Start a new item</a></td></tr>");
                }
                else
                {
					Output.WriteLine("    <tr><td style=\"width:35px\"><img src=\"" + currentMode.Default_Images_URL + "new_item.gif\" /></td><td><span style=\"color:gray\"><i>Online submittals are temporarily disabled</i></span></td></tr>");
                }
            }

            // If the user has already submitted stuff, add a link to all submitted items (otherwise a grayed out link)
            if (user.Items_Submitted_Count > 0)
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                currentMode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                currentMode.My_Sobek_SubMode = "Submitted Items";
				Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "submitted_items.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">View all my submitted items</a></td></tr>");

            }
            else
            {
                if (user.Can_Submit)
                {
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                    currentMode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                    currentMode.My_Sobek_SubMode = "Submitted Items";
					Output.WriteLine("    <tr><td style=\"width:35px\"><img src=\"" + currentMode.Default_Images_URL + "submitted_items.gif\" /></td><td><span style=\"color:Gray\">View all my submitted items</span></td></tr>");
                }
            }

            // If this user is linked to item statistics, add that link as well
            if (user.Has_Item_Stats)
            {
                // Add link to folder management
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.User_Usage_Stats;
                currentMode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "usage.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">View usage for my items</a></td></tr>");
            }

            // If the user has submitted some descriptive tags, or has the kind of rights that let them
            // view lists of tags, add that
            if ((user.Has_Descriptive_Tags) || (user.Is_System_Admin) || (user.Is_A_Collection_Manager_Or_Admin))
            {
                // Add link to folder management
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
                currentMode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "chat.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">View my descriptive tags</a></td></tr>");
            }

            // Add link to folder management
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
            currentMode.My_Sobek_SubMode = String.Empty;
            currentMode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "bookshelf.png\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">View and organize my bookshelves</a></td></tr>");

            // Add a link to view all saved searches
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "saved_searches.gif\"border=\"0px\"  /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">View my saved searches</a></td></tr>");

            //// Add a link to the test collecton if the user is linked to it directly
            //currentMode.Mode = SobekCM.Library.Navigation.Display_Mode_Enum.Aggregation_Home;
            //currentMode.Aggregation = String.Empty;
            //currentMode.Home_Type = SobekCM.Library.Navigation.Home_Type_Enum.Personalized;
            //Output.WriteLine("<tr valign=\"middle\" height=\"40px\"><td width=\"35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "home_folder.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Go to my collections</a></td></tr>");
            //currentMode.Mode = SobekCM.Library.Navigation.Display_Mode_Enum.My_Sobek;

            // Add a link to edit your preferences
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "settings.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Edit my account preferences</a></td></tr>");

            // If a return URL was provided, add a link to return there
            if ((currentMode.Return_URL.Length > 0) && ( currentMode.Return_URL.IndexOf("my") < 0 ))
            {
                Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Base_URL + currentMode.Return_URL + "\"><img src=\"" + currentMode.Default_Images_URL + "return.gif\" /></a></td><td><a href=\"" + currentMode.Base_URL + currentMode.Return_URL + "\">Return to previous " + sobek_text + " page</a></td></tr>");
            }
            
            // Add a log out link
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
			Output.WriteLine("    <tr><td style=\"width:35px\"><a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Default_Images_URL + "exit.gif\" /></a></td><td><a href=\"" + currentMode.Redirect_URL() + "\">Log Out</a></td></tr>");
            Output.WriteLine("  </table>");
			Output.WriteLine();
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;

            currentMode.Mode = Display_Mode_Enum.Contact;
	        Output.WriteLine("  <p>Comments or recommendations?  Please <a href=\"" + currentMode.Redirect_URL() + "\">contact us</a>.</p>");
            if (!user.Can_Submit)
            {
				Output.WriteLine("  <p>If you would like to contribute materials through the online system, please <a href=\"" + currentMode.Redirect_URL() + "\">contact us</a> as well.</p>");
            }

			Output.WriteLine("  <br />");

            currentMode.Mode = Display_Mode_Enum.My_Sobek;
            Output.WriteLine("</div>");
			Output.WriteLine();
        }
    }
}
