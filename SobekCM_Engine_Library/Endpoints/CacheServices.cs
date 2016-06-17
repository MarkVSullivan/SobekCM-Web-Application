using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    public class CacheServices : EndpointBase
    {
        /// <summary> Clears information about a single digital resource from the cache, and returns a message </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="Protocol"></param>
        /// <param name="IsDebug"></param>
        public void ClearCachedItem(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, Microservice_Endpoint_Protocol_Enum Protocol, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();
            string returnMessage = "Unknown error encountered";

            try
            {

                if (UrlSegments.Count < 2)
                {
                    tracer.Add_Trace("CacheServices.ClearCachedItem", "ERROR: BibID and VID must be included in the URL for the item to reset.");
                    returnMessage = "ERROR: BibID and VID must be included in the URL for the item to reset.";
                }
                else
                {
                    string BibID = UrlSegments[0];
                    string VID = UrlSegments[1];

                    // If this is 00000, then this clears the entire item group
                    if (VID == "00000")
                    {
                        tracer.Add_Trace("CacheServices.ClearCachedItem", "Will clear the cache of instances of all items/objects related to " + BibID + ".");

                        CachedDataManager.Items.Remove_Digital_Resource_Objects(BibID, tracer);

                        // Also remove the list of volumes, since this may have changed
                        CachedDataManager.Items.Remove_Items_In_Title(BibID, tracer);

                        returnMessage = "Cleared cache for " + BibID + ":" + VID;
                    }
                    else
                    {
                        tracer.Add_Trace("CacheServices.ClearCachedItem", "Will clear the cache of instances related to " + BibID + ":" + VID);

                        CachedDataManager.Items.Remove_Digital_Resource_Object(BibID, VID, tracer);

                        // Also remove the list of volumes, since this may have changed
                        CachedDataManager.Items.Remove_Items_In_Title(BibID, tracer);

                        returnMessage = "Cleared cache for " + BibID + ":" + VID;
                    }
                }

                // If this was debug mode, then just write the tracer
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("DEBUG MODE DETECTED");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine("Response: " + returnMessage);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    return;
                }

                // Get the JSON-P callback function
                string json_callback = "clearCachedItem";
                if ((Protocol == Microservice_Endpoint_Protocol_Enum.JSON_P) && (!String.IsNullOrEmpty(QueryString["callback"])))
                {
                    json_callback = QueryString["callback"];
                }

                // Use the base class to serialize the object according to request protocol
                Serialize(returnMessage, Response, Protocol, json_callback);

            }
            catch (Exception ee)
            {
                if (IsDebug)
                {
                    Response.ContentType = "text/plain";
                    Response.Output.WriteLine("EXCEPTION CAUGHT!");
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.Message);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(ee.StackTrace);
                    Response.Output.WriteLine();
                    Response.Output.WriteLine(tracer.Text_Trace);
                    return;
                }

                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Error completing request");
                Response.StatusCode = 500;
            }
        }
    }
}
