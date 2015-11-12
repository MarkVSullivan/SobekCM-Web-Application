#region Using directives

using System;
using System.IO;
using SobekCM.Library.UI;

#endregion

namespace SobekCM.Library
{
    /// <summary> Class writes the web.config file for the resource folders to limit access
    /// in the case the resource is either dark or is ip restricted. </summary>
    class Resource_Web_Config_Writer
    {
        /// <summary> Update the resource folders web.config file based on the resource visibility
        /// and the current web server's IP address  </summary>
        /// <param name="Resource_Folder"> Folder for the digital resource </param>
        /// <param name="Dark_Flag"> Flag indicates if this item is DARK </param>
        /// <param name="IP_Restriction_Membership"> IP restriction membership is used if the resource is IP restricted </param>
        /// <param name="Main_Thumbnail"> Filename for the main thumbnail (which will NOT be restricted) </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Update_Web_Config( string Resource_Folder, bool Dark_Flag, short IP_Restriction_Membership, string Main_Thumbnail )
        {
            try
            {
                string web_config = Resource_Folder + "\\web.config";
                if (File.Exists(web_config))
                    File.Delete(web_config);
                if (( Dark_Flag) || (IP_Restriction_Membership > 0))
                {
                    StreamWriter writer = new StreamWriter(web_config, false);
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<configuration>");
                    writer.WriteLine("    <system.webServer>");
                    writer.WriteLine("        <security>");
                    writer.WriteLine("            <ipSecurity allowUnlisted=\"false\">");
                    writer.WriteLine("                 <clear />");
                    writer.WriteLine("                 <add ipAddress=\"127.0.0.1\" allowed=\"true\" />");
                    if ( !String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Servers.SobekCM_Web_Server_IP))
                        writer.WriteLine("                 <add ipAddress=\"" + UI_ApplicationCache_Gateway.Settings.Servers.SobekCM_Web_Server_IP.Trim() + "\" allowed=\"true\" />");
                    writer.WriteLine("            </ipSecurity>");
                    writer.WriteLine("        </security>");
                    writer.WriteLine("        <modules runAllManagedModulesForAllRequests=\"true\" />");
                    writer.WriteLine("    </system.webServer>");

                    // Is there now a main thumbnail?
                    if (( Main_Thumbnail.Length > 0) && ( Main_Thumbnail.IndexOf("http:") < 0))
                    {
                        writer.WriteLine("    <location path=\"" + Main_Thumbnail + "\">");
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

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
