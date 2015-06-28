#region Using directives

using System;
using System.Web;

#endregion

namespace SobekCM.Engine_Library
{
    /// <summary> Rewrites the incomig request to the engine in a way that the handler can then work with </summary>
    public class MicroserviceRewriter : IHttpModule
    {
        /// <summary> Handles the BeginRequest event of the RewriteModule control </summary>
        /// <param name="Sender">The source of the event.</param>
        /// <param name="E">The <see cref="EventArgs"/> instance containing the event data.</param>
        void RewriteModule_BeginRequest(object Sender, EventArgs E)
        {
            HttpContext context = HttpContext.Current;
            context.Response.ContentType = "application/json";

            if (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath == null)
                return;

            string appRelative = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.ToLower();
            string url_authority = HttpContext.Current.Request.Url.Authority;

            // Favicon.ico is a common request.. abort right here
            if (appRelative.IndexOf("favicon.ico", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return;

            // Remove any double slashes
            appRelative = appRelative.Replace("//", "/");

            // Get rid of first part 
            if (appRelative.IndexOf("~/") == 0)
                appRelative = appRelative.Substring(2);

            // Get rid of leading '/'
            if ((appRelative.Length > 0) && (appRelative[0] == '/'))
            {
                appRelative = appRelative.Length > 1 ? appRelative.Substring(1) : String.Empty;
            }

            // Save the original URL 
            HttpContext.Current.Items.Add("Original_URL", HttpContext.Current.Request.Url.ToString());

            // Get the current query string
            string current_querystring = HttpContext.Current.Request.QueryString.ToString();

            if (appRelative.IndexOf("engine") == 0)
            {
                if (appRelative.Length > 6)
                {
                    appRelative = appRelative.Substring(6);

                    // Standard rewrite to the sobek engine service
                    if (current_querystring.Length > 0)
                    {
                        HttpContext.Current.RewritePath("~/sobekcm.svc?urlrelative=" + appRelative + "&" + current_querystring + "&portal=" + url_authority, true);
                    }
                    else
                    {
                        HttpContext.Current.RewritePath("~/sobekcm.svc?urlrelative=" + appRelative + "&portal=" + url_authority, true);
                    }
                }
                else
                {
                    // Standard rewrite to the sobek engine service
                    if (current_querystring.Length > 0)
                    {
                        HttpContext.Current.RewritePath("~/sobekcm.svc?" + current_querystring + "&portal=" + url_authority, true);
                    }
                    else
                    {
                        HttpContext.Current.RewritePath("~/sobekcm.svc?portal=" + url_authority, true);
                    }
                }
            }
        }

        /// <summary> Initializes a module and prepares it to handle requests. </summary>
        /// <param name="Context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication Context)
        {
            Context.BeginRequest += RewriteModule_BeginRequest;
        }

        /// <summary> Dispose of this class </summary>
        public void Dispose() { }
    }
}