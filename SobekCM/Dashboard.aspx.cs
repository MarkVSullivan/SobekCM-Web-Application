using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Dashboard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    protected void Add_Html()
    {
        if (Session["Last_Exception"] != null)
        {
            Exception lastException = (Exception)Session["Last_Exception"];
            if (lastException is SobekCM.Library.SobekCM_Traced_Exception)
            {
                SobekCM.Library.SobekCM_Traced_Exception tracedException = (SobekCM.Library.SobekCM_Traced_Exception)lastException;

                Response.Output.WriteLine("<h1>EXCEPTION CAUGHT</h1><br />");
                Response.Output.WriteLine("<h2>SobekCM Message</h2>");
                Response.Output.WriteLine("<blockquote>" + tracedException.Message + "</blockquote>");
                Response.Output.WriteLine("<h2>Inner Message</h2>");
                Response.Output.WriteLine("<blockquote>" + tracedException.InnerException.Message + "</blockquote>");
                if (tracedException.InnerException.StackTrace.Length > 0)
                {
                    Response.Output.WriteLine("<h2>Stack Trace</h2>");
                    Response.Output.WriteLine("<blockquote>" + tracedException.InnerException.StackTrace.Replace("\n", "<br />") + "</blockquote>");
                }
                Response.Output.WriteLine("<h2>SobekCM Tracer</h2>");

                Response.Output.WriteLine("<style type=\"text/css\">");
                Response.Output.WriteLine("table.Traceroute { border-width: 2px; border-style: solid; border-color: gray; border-collapse: collapse; background-color: white; font-size: small; }");
                Response.Output.WriteLine("table.Traceroute th { border-width: 2px; padding: 3px; border-style: solid; border-color: gray; background-color: gray; color: white; }");
                Response.Output.WriteLine("table.Traceroute td { border-width: 2px; padding: 3px; border-style: solid; border-color: gray;	background-color: white; }");
                Response.Output.WriteLine("</style>");
                Response.Output.WriteLine("<blockquote>");
                Response.Output.WriteLine(tracedException.Trace_Route_HTML);
                Response.Output.WriteLine("</blockquote>");
            }
            else
            {
                Response.Output.WriteLine("<h1>EXCEPTION CAUGHT</h1><br /><br />");
                Response.Output.WriteLine("<h2>Message</h2>");
                Response.Output.WriteLine("<blockquote>" + lastException.Message + "</blockquote>");
                if (lastException.StackTrace.Length > 0)
                {
                    Response.Output.WriteLine("<h2>Stack Trace</h2>");
                    Response.Output.WriteLine("<blockquote>" + lastException.StackTrace + "</blockquote>");
                }
            }

            // Clear this exception now
            HttpContext.Current.Session.Remove("Last_Exception");
        }
        else
        {
            Response.Output.WriteLine("<h1>SobekCM Dashboard</h1>");
            Response.Output.WriteLine("This dashboard is currently only used to display exceptions when the application is run locally.<br /><br />");
            Response.Output.WriteLine("In upcoming versions this screen will be used to assess the overall health of the SobekCM system and spot possible problems.");
        }
    }
}