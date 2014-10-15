#region Using directives

using System;
using System.IO;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class UpdateWebConfigModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Delete any existing web.config file and write is as necessary
            try
            {
                string web_config = Resource.Resource_Folder + "\\web.config";
                if (File.Exists(web_config))
                    File.Delete(web_config);
                if ((Resource.Metadata.Behaviors.Dark_Flag) || (Resource.Metadata.Behaviors.IP_Restriction_Membership > 0))
                {
                    StreamWriter writer = new StreamWriter(web_config, false);
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<configuration>");
                    writer.WriteLine("    <system.webServer>");
                    writer.WriteLine("        <security>");
                    writer.WriteLine("            <ipSecurity allowUnlisted=\"false\">");
                    writer.WriteLine("                 <clear />");
                    writer.WriteLine("                 <add ipAddress=\"127.0.0.1\" allowed=\"true\" />");
                    if (Settings.SobekCM_Web_Server_IP.Length > 0)
                        writer.WriteLine("                 <add ipAddress=\"" + Settings.SobekCM_Web_Server_IP.Trim() + "\" allowed=\"true\" />");
                    writer.WriteLine("            </ipSecurity>");
                    writer.WriteLine("        </security>");
                    writer.WriteLine("        <modules runAllManagedModulesForAllRequests=\"true\" />");
                    writer.WriteLine("    </system.webServer>");

                    // Is there now a main thumbnail?
                    if ((Resource.Metadata.Behaviors.Main_Thumbnail.Length > 0) && (Resource.Metadata.Behaviors.Main_Thumbnail.IndexOf("http:") < 0))
                    {
                        writer.WriteLine("    <location path=\"" + Resource.Metadata.Behaviors.Main_Thumbnail + "\">");
                        writer.WriteLine("        <system.webServer>");
                        writer.WriteLine("            <security>");
                        writer.WriteLine("                <ipSecurity allowUnlisted=\"true\" />");
                        writer.WriteLine("            </security>");
                        writer.WriteLine("        </system.webServer>");
                        writer.WriteLine("    </location>");
                    }

                    writer.WriteLine("</configuration>");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                OnError("Unable to update the resource web.config file", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }
        }
    }
}
