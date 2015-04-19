#region Using directives

using System;
using System.IO;
using System.Web;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM
{
	public class Global : HttpApplication
	{
   

		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object Sender, EventArgs E)
		{
			// Get the exception
			Exception objErr = Server.GetLastError();
			if (objErr == null)
				return;

			objErr = objErr.GetBaseException();

			try
			{
				// Justs clear the error for a number of common errors, caused by invalid requests to the server
				if ((objErr.Message.IndexOf("potentially dangerous") >= 0) || (objErr.Message.IndexOf("a control with id ") >= 0) || (objErr.Message.IndexOf("Padding is invalid and cannot be removed") >= 0) || (objErr.Message.IndexOf("This is an invalid webresource request") >= 0) ||
					((objErr.Message.IndexOf("File") >= 0) && (objErr.Message.IndexOf("does not exist") >= 0)))
				{
					// Clear the error
					Server.ClearError();
				}
				else
				{
					try
					{
						StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\temp\\exceptions.txt", true);
						writer.WriteLine();
						writer.WriteLine("Error Caught in Application_Error event ( " + DateTime.Now.ToString() + ")");
						writer.WriteLine("User Host Address: " + Request.UserHostAddress);
						writer.WriteLine("Requested URL: " + Request.Url);
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
						// Nothing else to do here.. no other known way to log this error
					}
				}
			}
			catch (Exception)
			{
				// Nothing else to do here.. no other known way to log this error
			}
			finally
			{
				// Clear the error
				Server.ClearError();

				string error_message = objErr.Message;
				if (objErr is SobekCM_Traced_Exception)
				{
					SobekCM_Traced_Exception sobekException = (SobekCM_Traced_Exception)objErr;
					error_message = sobekException.InnerException.Message;

				}

				try
				{
					if ((HttpContext.Current.Request.UserHostAddress == "127.0.0.1") || (HttpContext.Current.Request.UserHostAddress == HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]) || (HttpContext.Current.Request.Url.ToString().IndexOf("localhost") >= 0))
					{
						Response.Redirect("error_echo.html?text=" + error_message.Replace(" ", "_").Replace("&", "and").Replace("?", ""), false);
					}
					else
					{
						// Forward if there is a place to forward to.
						if (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.System_Error_URL))
						{
							Response.Redirect(UI_ApplicationCache_Gateway.Settings.System_Error_URL, false);
						}
						else
						{
							Response.Redirect("http://ufdc.ufl.edu/sobekcm/missing_config", false);
						}
					}
				}
				catch (Exception)
				{
					// Nothing else to do here.. no other known way to log this error
				}
			}

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}