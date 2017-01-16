#region Using directives

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM
{
	public partial class SobekCM : Page
	{
		private SobekCM_Page_Globals pageGlobals;

		#region Page_Load method does the final checks and creates the writer type

		protected void Page_Load(object Sender, EventArgs E)
		{
			pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", String.Empty);

			try
			{
				// Process this page request by building the main writer and 
				// analyzing the request's URL
				pageGlobals.On_Page_Load();

				// Is the response completed already?
				if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				{
					return;
				}


				if (HttpContext.Current.Items.Contains("Original_URL"))
				{
					string original_url = HttpContext.Current.Items["Original_URL"].ToString();
					itemNavForm.Action = original_url;

					// Save this as the return spot, if it is not preferences
					if ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Preferences) && (pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact))
					{
						Session["Last_Mode"] = original_url;
					}
				}
				else
				{
					// Save this as the return spot, if it is not preferences
					if ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Preferences) && (pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact))
					{
						string url = HttpContext.Current.Request.Url.ToString();
						Session["Last_Mode"] = url;
					}
				}

				if ((UI_ApplicationCache_Gateway.Settings.Servers.Web_Output_Caching_Minutes > 0) && (String.IsNullOrEmpty(Request.QueryString["refresh"])))
				{
					if ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Error) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.My_Sobek) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Administrative) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact_Sent) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Item_Print) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Item_Cache_Reload) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Reset) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Internal) &&
						(pageGlobals.currentMode.Mode != Display_Mode_Enum.Public_Folder) &&
                        ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Simple_HTML_CMS) || ((pageGlobals.currentMode.WebContent_Type != WebContent_Type_Enum.Edit) && (pageGlobals.currentMode.WebContent_Type != WebContent_Type_Enum.Milestones))) &&
						((pageGlobals.currentMode.Mode != Display_Mode_Enum.Aggregation) || (pageGlobals.currentMode.Aggregation_Type != Aggregation_Type_Enum.Home_Edit)) &&
                        ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Aggregation) || (pageGlobals.currentMode.Aggregation_Type != Aggregation_Type_Enum.Work_History)) &&
                        ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Aggregation) || (pageGlobals.currentMode.Aggregation_Type != Aggregation_Type_Enum.User_Permissions)) &&
						((pageGlobals.currentMode.Mode != Display_Mode_Enum.Aggregation) || (pageGlobals.currentMode.Aggregation_Type != Aggregation_Type_Enum.Child_Page_Edit)) &&
						((pageGlobals.currentMode.Mode != Display_Mode_Enum.Aggregation) || (pageGlobals.currentMode.Aggregation_Type != Aggregation_Type_Enum.Home) || (pageGlobals.currentMode.Home_Type != Home_Type_Enum.Personalized)) &&
						(pageGlobals.currentMode.Result_Display_Type != Result_Display_Type_Enum.Export) &&
						((pageGlobals.currentMode.Mode != Display_Mode_Enum.Item_Display) || (( !String.IsNullOrEmpty(pageGlobals.currentMode.ViewerCode)) && (pageGlobals.currentMode.ViewerCode.ToUpper().IndexOf("citation") < 0) && (pageGlobals.currentMode.ViewerCode.ToUpper().IndexOf("allvolumes3") < 0))))
					{
						Response.Cache.SetCacheability(HttpCacheability.Private);
						Response.Cache.SetMaxAge(new TimeSpan(0, UI_ApplicationCache_Gateway.Settings.Servers.Web_Output_Caching_Minutes, 0));
					}
					else
					{
						Response.Cache.SetCacheability(HttpCacheability.NoCache);
					}
				}
				else
				{
					Response.Cache.SetCacheability(HttpCacheability.NoCache);
				}

				// Check if the item nav form should be shown
				if (!pageGlobals.mainWriter.Include_Navigation_Form)
				{
					itemNavForm.Visible = false;
				}
				else
				{
					if (!pageGlobals.mainWriter.Include_Main_Place_Holder)
						mainPlaceHolder.Visible = false;
				}

				// The file upload form is only shown in these cases
				if ((pageGlobals.mainWriter != null) && (pageGlobals.mainWriter.File_Upload_Possible))
				{
					itemNavForm.Enctype = "multipart/form-data";
				}

				// Add the controls now
				pageGlobals.mainWriter.Add_Controls( mainPlaceHolder, pageGlobals.tracer);
			}
			catch (OutOfMemoryException ee)
			{
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", "OutOfMemoryException caught!");

				pageGlobals.Email_Information("SobekCM Out of Memory Exception", ee);
			}
			catch (Exception ee)
			{
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", "Exception caught!", Custom_Trace_Type_Enum.Error);
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", ee.Message, Custom_Trace_Type_Enum.Error);
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", ee.StackTrace, Custom_Trace_Type_Enum.Error);

				if (pageGlobals.currentMode != null)
				{
					pageGlobals.currentMode.Mode = Display_Mode_Enum.Error;
					pageGlobals.currentMode.Error_Message = "Unknown error caught while executing your request";
					pageGlobals.currentMode.Caught_Exception = ee;
				}
			}
		}


		#endregion

		#region Methods called during execution of the HTML from SobekCM.aspx

	    protected void Write_Lang_Code()
	    {
            // If the was a very basic error, or the request was complete, do nothing here
            if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
                return;

	        if (pageGlobals.currentMode.Language == Web_Language_Enum.DEFAULT)
	        {
	            Response.Output.Write(Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.System.Default_UI_Language));
	        }
	        else
	        {
                Response.Output.Write(Web_Language_Enum_Converter.Enum_To_Code(pageGlobals.currentMode.Language));
	        }
	    }

		protected void Write_Page_Title()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Page_Title", String.Empty);

			// Allow the html writer to add its own title 
			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				Response.Output.Write(((Html_MainWriter)pageGlobals.mainWriter).Get_Page_Title(pageGlobals.tracer));
			}

			// For robot crawlers using the HTML ECHO writer, the title is alway in the info browse mode
			if (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
			{
				Response.Output.Write(pageGlobals.currentMode.Info_Browse_Mode);
			}
		}

		protected void Write_Within_HTML_Head()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Within_HTML_Head", String.Empty);

			// Only bother writing the style references if this is writing HTML (either logged out or logged in via mySobekCM)
			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				((Html_MainWriter)pageGlobals.mainWriter).Write_Within_HTML_Head(Response.Output, pageGlobals.tracer);
			}

			// If this is for the robots, add some generic style statements
			if (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
			{
				((Html_Echo_MainWriter)pageGlobals.mainWriter).Write_Within_HTML_Head(Response.Output, pageGlobals.tracer);
			}
		}

		protected void Write_Body_Attributes()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Body_Attributes", String.Empty);


			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				Response.Output.Write(((Html_MainWriter)pageGlobals.mainWriter).Get_Body_Attributes(pageGlobals.tracer));
			}

			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo) && (pageGlobals.currentMode.Mode == Display_Mode_Enum.Item_Display))
			{
				Response.Output.Write("id=\"itembody\"");
			}
		}

		protected void Write_Html()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			// Add the HTML to the main section (which sits outside any of the standard fors)
			pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_HTML", String.Empty);
			pageGlobals.mainWriter.Write_Html(Response.Output, pageGlobals.tracer);
		}

		protected void Write_ItemNavForm_Opening()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
				((Html_MainWriter)pageGlobals.mainWriter).Write_ItemNavForm_Opening(Response.Output, pageGlobals.tracer);
			}
		}


		protected void Write_ItemNavForm_Closing()
		{


			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
				((Html_MainWriter)pageGlobals.mainWriter).Write_ItemNavForm_Closing(Response.Output, pageGlobals.tracer);
			}
		}

		protected void Write_Final_HTML()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Final_HTML", String.Empty);
				((Html_MainWriter)pageGlobals.mainWriter).Write_Final_HTML(Response.Output, pageGlobals.tracer);
			}
		}



		#endregion



		protected override void OnInit(EventArgs E)
		{
            // Ensure there is a base URL
		    if (String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL))
		    {
		        string base_url = Request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "");
	            if (base_url.IndexOf("?") > 0)
	                base_url = base_url.Substring(0, base_url.IndexOf("?"));
		        if (base_url[base_url.Length - 1] != '/')
		            base_url = base_url + "/";
                UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL = base_url;
                UI_ApplicationCache_Gateway.Settings.Servers.Base_URL = base_url;
		    }

            // Initializee the Sobek file system abstraction
            SobekFileSystem.Initialize(UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network, UI_ApplicationCache_Gateway.Settings.Servers.Image_URL);

            // Ensure the microservices client has read the configuration file
		    if (!SobekEngineClient.Config_Read_Attempted)
		    {
#if DEBUG
                string base_url = Request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "");
            	if (base_url.IndexOf("localhost:") > 0)
			    {
                    if (base_url.IndexOf("?") > 0)
				        base_url = base_url.Substring(0, base_url.IndexOf("?"));
			        UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL = base_url;
			        UI_ApplicationCache_Gateway.Settings.Servers.Base_URL = base_url;

			    }
#endif

                // Get the base URL
                string path = Server.MapPath("config/default/sobekcm_microservices.config");
                SobekEngineClient.Read_Config_File(path, UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL);
		    }


		    pageGlobals = new SobekCM_Page_Globals(IsPostBack, "SOBEKCM");

			base.OnInit(E);
		}

		protected void Repository_Title()
		{
			// If the was a very basic error, or the request was complete, do nothing here
			if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
				return;

			if (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.System.System_Name))
				Response.Output.Write(UI_ApplicationCache_Gateway.Settings.System.System_Name + " : SobekCM Digital Repository");
			else
				Response.Output.Write("SobekCM Digital Repository");
		}
	}
}