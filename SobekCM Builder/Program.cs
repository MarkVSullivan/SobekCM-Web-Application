#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Library.Database;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;
using SobekCM.Builder_Library;
using Microsoft.Win32;
using SobekCM.Builder_Library.Settings;

#endregion

namespace SobekCM.Builder
{
    public class Program
    {
 
        static void Main(string[] args)
        {
            //// Try to read the metadata configuration file
            string app_start_config = Application.StartupPath + "\\config";
            //if ((Directory.Exists(app_start_config)) && (File.Exists(app_start_config + "\\sobekCM_metadata.config")))
            //{
            //    ResourceObjectSettings.MetadataConfig.Read_Metadata_Configuration(app_start_config + "\\sobekCM_metadata.config");
            //}


	        bool complete_static_rebuild = false;
            bool marc_rebuild = false;
            bool run_preloader = true;
            bool show_help = false;
            bool build_production_marcxml_feed = false;
            bool build_test_marcxml_feed = false;
            string invalid_arg = String.Empty;
            bool verbose = false;

	        // Get values from the arguments
            foreach (string thisArgs in args)
            {
                bool arg_handled = false;

                // Check for versioning option
                if (thisArgs == "--version")
                {
                    Console.WriteLine("You are running version " + Engine_ApplicationCache_Gateway.Settings.Static.Current_Builder_Version + " of the SobekCM Builder.");
                    return;
                }

                // Always run in BACKGROUND mode
                if (thisArgs == "--background")
                {
                    arg_handled = true;
                }

                // Check for verbose flag
                if (thisArgs == "--verbose")
                {
                    verbose = true;
                    arg_handled = true;
                }

                // Check for no loader flag
                if (thisArgs == "--refresh_oai")
                {
                    arg_handled = true;
                }


                // Check for no oading flag
                if (thisArgs == "--noload")
                {
                    run_preloader = false;
                    arg_handled = true;
                }

                // Check for static rebuild
                if (thisArgs == "--staticrebuild")
                {
                    complete_static_rebuild = true;
                    arg_handled = true;
                }

                // Check for static rebuild
                if (thisArgs == "--createmarc")
                {
                    marc_rebuild = true;
                    arg_handled = true;
                }

                // Check for marc xml feed creation flags
                if (thisArgs.IndexOf("--marcxml") == 0)
                {
                    build_production_marcxml_feed = true;
                    arg_handled = true;
                }
                if (thisArgs.IndexOf("--testmarcxml") == 0)
                {
                    build_test_marcxml_feed = true;
                    arg_handled = true;
                }

                // Check for help
                if ((thisArgs == "--help") || (thisArgs == "?") || (thisArgs == "-help"))
                {
                    show_help = true;
                    arg_handled = true;
                }

                // If not handled, set as error
                if (!arg_handled)
                {
                    invalid_arg = thisArgs;
                    break;
                }
            }

            // Was there an invalid argument or was help requested
            if ((invalid_arg.Length > 0 ) || (show_help))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("\nThis application is used to bulk load SobekCM items, perform post-processing\n");
                builder.Append("for items loaded through the web, and perform some regular maintenance activities\n");
                builder.Append("in support of a SobekCM web application.\n\n");
                builder.Append("Usage: SobekCM_Builder [options]\n\n");
                builder.Append("Options:\n\n");
                builder.Append("  --version\tDisplays the current version of the SobekCM Builder\n\n");
                builder.Append("  --verbose\tFlag indicates to be verbose in the logs and console\n\n");
                builder.Append("  --help\t\tShows these instructions\n\n");
                builder.Append("  --noload\t\tSupresses the loading portion of the SobekCM Builder\n\n");
                builder.Append("  --staticrebuild\tPerform a complete rebuild on static pages\n\n");
                builder.Append("  --marcxml\tBuild the marcxml production feed\n\n");
                builder.Append("  --testmarcxml\tBuild the marcxml test feed\n\n");
                builder.Append("  --createmarc\tRecreate all of the MARC.xml files\n\n");
                builder.Append("  --refresh_oai\tResave the OAI-PMH DC data for every item in the library\n\n");
                builder.Append("Examples:\n\n");
                builder.Append("  1. To just rebuild all the static pages:\n");
                builder.Append("       SobekCM_Builder --nopreload --noload --staticrebuild");
                builder.Append("  2. To have the SobekCM Builder constantly run in the background");
                builder.Append("       SobekCM_Builder --background");

                // If invalid arg, save to log file
                if (invalid_arg.Length > 0 )
                {
                    // Show INVALID ARGUMENT error in console
                    Console.WriteLine("\nINVALID ARGUMENT PROVIDED ( " + invalid_arg + " )");
                }

                Console.WriteLine(builder.ToString());
                return;
            }

            // Now, veryify the configuration file exists
            string config_file = Application.StartupPath + "\\config\\sobekcm.config";
            if (!File.Exists(config_file))
            {
                Console.WriteLine("The configuration file is missing!!\n");
                Console.WriteLine("Execution aborted due to missing configuration file.");
                return;
            }

            // Should be a config file now, so read it
            if (!MultiInstance_Builder_Settings_Reader.Read_Config(config_file))
            {
                Console.WriteLine("Error encountered reading the configuration file!!\n");
                Console.WriteLine("Execution aborted due to incorrect configuration file.");
                return;
            }

            // If no instances exist, then the builder has nothing to do
            if ((MultiInstance_Builder_Settings.Instances.Count == 0) || (String.IsNullOrEmpty(MultiInstance_Builder_Settings.Instances[0].DatabaseConnection.Connection_String)))
            {
                Console.WriteLine("Missing database connection string!!\n");
                Console.WriteLine("Execution aborted due to configuration file not including any instances to process");
                return;
            }

            // Assign the connection string and test the connection (if only a single connection listed)
	        if ( MultiInstance_Builder_Settings.Instances.Count == 1)
	        {
		        SobekCM_Database.Connection_String = MultiInstance_Builder_Settings.Instances[0].DatabaseConnection.Connection_String;
		        if (!SobekCM_Database.Test_Connection())
		        {
			        Console.WriteLine("Unable to connect to the database using provided connection string:");
			        Console.WriteLine();
			        Console.WriteLine(SobekCM_Database.Connection_String);
			        return;
		        }
	        }

            // Verify connectivity and rights on the logs subfolder
            string uri = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///","");
            string logFileDirectory = Path.Combine(new FileInfo(uri).Directory.FullName, "logs");
            if (!Directory.Exists(logFileDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logFileDirectory);
                }
                catch
                {
                    Console.WriteLine("Error creating necessary logs subfolder under the application folder.\n");
                    Console.WriteLine("Please create manually.\n");
                    Console.WriteLine(logFileDirectory);
                    return;
                }
            }
            try
            {
				StreamWriter testWriter = new StreamWriter(Path.Combine(logFileDirectory, "test.log"), false);
                testWriter.WriteLine("TEST");
                testWriter.Flush();
                testWriter.Close();

                File.Delete(Path.Combine(logFileDirectory, "test.log"));
            }
            catch
            {
                Console.WriteLine("The service account needs modify rights on the logs subfolder.\n");
                Console.WriteLine("Please correct manually.\n");
				Console.WriteLine(logFileDirectory);
                return;
            }

            // Look for Ghostscript from the registry, if not provided in the config file
            if (Engine_ApplicationCache_Gateway.Settings.Builder.Ghostscript_Executable.Length == 0)
            {
                // LOOK FOR THE GHOSTSCRIPT DIRECTORY
                string possible_ghost = Look_For_Variable_Registry_Key("SOFTWARE\\GPL Ghostscript", "GS_DLL");
                if (!String.IsNullOrEmpty(possible_ghost))
                   Engine_ApplicationCache_Gateway.Settings.Builder.Ghostscript_Executable = possible_ghost;
            }

            // Look for Imagemagick from the registry, if not provided in the config file
            string possible_imagemagick = Look_For_Variable_Registry_Key("SOFTWARE\\ImageMagick", "BinPath");
            if (!String.IsNullOrEmpty(possible_imagemagick))
                Engine_ApplicationCache_Gateway.Settings.Builder.ImageMagick_Executable = possible_imagemagick;

            //// If this is to refresh the OAI, don't use the worker controller
            //if (( refresh_oai ) &&  (Engine_ApplicationCache_Gateway.Settings.Database_Connections.Count == 1))
            //{
            //    // Set the item for the current mode
            //    int successes = 0;

            //    DataTable item_list_table = SobekCM_Database.Get_All_Groups_First_VID( );
            //    foreach (DataRow thisRow in item_list_table.Rows)
            //    {
            //        string bibid = thisRow["BibID"].ToString();
            //        string vid = thisRow["VID"].ToString();
            //        int groupid = Convert.ToInt32(thisRow["groupid"]);

            //        string directory = Engine_ApplicationCache_Gateway.Settings.Image_Server_Network + bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8, 2) + "\\" + vid;
            //        string mets = directory + "\\" + bibid + "_" + vid + ".mets.xml";

            //        try
            //        {
            //            SobekCM_Item thisItem = SobekCM_Item.Read_METS(mets);
            //            if (thisItem != null)
            //            {
            //                // Get the OAI-PMH dublin core information
            //                StringBuilder oaiDataBuilder = new StringBuilder(1000);
            //                StringWriter writer = new StringWriter(oaiDataBuilder);
            //                DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(writer, thisItem.Bib_Info);
            //                // Also add the URL as identifier
            //                oaiDataBuilder.AppendLine("<dc:identifier>" + Engine_ApplicationCache_Gateway.Settings.System_Base_URL + bibid + "</dc:identifier>");
            //                Resource_Object.Database.SobekCM_Database.Save_Item_Group_OAI(groupid, oaiDataBuilder.ToString(), "oai_dc", true);
            //                writer.Flush();
            //                writer.Close();

            //                successes++;
            //                if (successes%1000 == 0)
            //                    Console.WriteLine(@"{0} complete", successes);
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }
            //    return;
            //}

            // Controller always runs in background mode
            Worker_Controller controller = new Worker_Controller(verbose, Application.StartupPath );
            controller.Execute_In_Background();

            // If this was set to aborting, set to last execution aborted
            Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
            if (( operationFlag == Builder_Operation_Flag_Enum.ABORTING ) || ( operationFlag == Builder_Operation_Flag_Enum.ABORT_REQUESTED ))
                Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
        }

        private static string Look_For_Variable_Registry_Key(string Manufacturer, string KeyName)
        {
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            localKey = localKey.OpenSubKey(Manufacturer);
            if (localKey != null)
            {
                string[] subkeys = localKey.GetSubKeyNames();
                foreach (string thisSubKey in subkeys)
                {
                    RegistryKey subKey = localKey.OpenSubKey(thisSubKey);
                    string value64 = subKey.GetValue(KeyName) as string;
                    if (!String.IsNullOrEmpty(value64))
                        return value64;
                }
            }
            RegistryKey localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            localKey32 = localKey32.OpenSubKey(Manufacturer);
            if (localKey32 != null)
            {
                string[] subkeys = localKey32.GetSubKeyNames();
                foreach (string thisSubKey in subkeys)
                {
                    RegistryKey subKey = localKey32.OpenSubKey(thisSubKey);
                    string value32 = subKey.GetValue(KeyName) as string;
                    if (!String.IsNullOrEmpty(value32))
                        return value32;
                }
            }
            return null;
        }

    }
}
