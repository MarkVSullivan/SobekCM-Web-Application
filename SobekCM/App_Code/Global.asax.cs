#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Library;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Skins;
using darrenjohnstone.net.FileUpload;

#endregion

/// <summary>
/// Summary description for Global
/// </summary>
public class Global : HttpApplication
{
    // Values used by the SobekCM Library, as well as the SobekCM Web Application
    public static Checked_Out_Items_List Checked_List;
    public static Aggregation_Code_Manager Codes;
    public static Dictionary<string, string> Collection_Aliases;
    public static Dictionary<string, Wordmark_Icon> Icon_List;
    public static IP_Restriction_Ranges IP_Restrictions;
    public static Item_Lookup_Object Item_List;
    public static DateTime? Last_Refresh;
    public static Recent_Searches Search_History;
    public static SobekCM_Skin_Collection Skins;
    public static Statistics_Dates Stats_Date_Range;
    public static List<Thematic_Heading> Thematic_Headings;
    public static Language_Support_Info Translation;
    public static Portal_List URL_Portals;


    //void Application_Start(object sender, EventArgs e)
    //{
    //        // This sets up the default processor
    //        UploadManager.Instance.ProcessorType = typeof(DummyProcessor);
    //        UploadManager.Instance.ProcessorInit += Processor_Init;
    //}

    ///// <summary>
    ///// Initialises the file processor.
    ///// </summary>
    ///// <param name="sender">Sender</param>
    ///// <param name="args">Arguments</param>
    //void Processor_Init(object sender, FileProcessorInitEventArgs args)
    //{
    //}



    void Application_Error(object sender, EventArgs e)
    {
        // Get the exception
        Exception objErr = Server.GetLastError().GetBaseException();

        try
        {


            if (( objErr.Message.IndexOf("potentially dangerous") >= 0) || ( objErr.Message.IndexOf("a control with id ") >= 0 ) || (objErr.Message.IndexOf("Padding is invalid and cannot be removed") >= 0 ) || ( objErr.Message.IndexOf("This is an invalid webresource request") >= 0 ))
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
                   

                //try
                //{
                //    string err = Request.UserHostAddress + "\n\nError Caught in Application_Error event\nError in: " + Request.Url.ToString();
                //    if (objErr is SobekCM.Library.SobekCM_Traced_Exception)
                //    {
                //        SobekCM.Library.SobekCM_Traced_Exception sobekException = (SobekCM.Library.SobekCM_Traced_Exception)objErr;

                //        err = err + "\nError Message:" + sobekException.InnerException.Message.ToString() +
                //            "\nStack Trace:" + sobekException.InnerException.StackTrace.ToString() + "\n\n" +
                //            sobekException.Trace_Route;
                //    }
                //    else
                //    {
                //        // Build the error message
                //        err = err + "\nError Message:" + objErr.Message.ToString() +
                //            "\nStack Trace:" + objErr.StackTrace.ToString();
                //    }

                //    // Email the error message
                //    System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage("ufdc_error@uflib.ufl.edu", System.Configuration.ConfigurationSettings.AppSettings["Error_Emails"]);
                //    if ((objErr.Message.IndexOf("Timeout expired") >= 0) && (objErr.StackTrace.IndexOf("Database") >= 0))
                //    {
                //        myMail.Subject = "SobekCM Database Timeout Error";
                //    }
                //    else
                //    {
                //        myMail.Subject = "SobekCM Exception Caught";
                //    }
                //    myMail.Body = err;

                //    // Mail this
                //    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.ufl.edu");

                //    client.Send(myMail);
                //}
                //catch
                //{

                //}
            }
        }
        catch ( Exception )
        {
            // Nothing else to do here.. no other known way to log this error
        }
        finally
        {
            string error_message = objErr.Message;
            if (objErr is SobekCM_Traced_Exception)
            {
                SobekCM_Traced_Exception sobekException = (SobekCM_Traced_Exception)objErr;
                error_message = sobekException.InnerException.Message;

            }

            // Clear the error
            Server.ClearError();

            try
            {

                   Response.Redirect("http://ufdc.ufl.edu/error_echo.html?text=" + error_message.Replace(" ", "_").Replace("&", "and").Replace("?", ""), true);
                // Forward to our error message
               // Response.Redirect(System.Configuration.ConfigurationSettings.AppSettings["Error_HTML_Page"]);
            }
            catch (Exception)
            {
                // Nothing else to do here.. no other known way to log this error
            }
        }

    }

    void Application_BeginRequest()
    {


    }
}
