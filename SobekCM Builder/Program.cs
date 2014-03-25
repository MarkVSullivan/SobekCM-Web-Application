#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;
using Microsoft.Win32;

#endregion

namespace SobekCM.Builder
{
    public class Program
    {
 
        static void Main(string[] args)
        {
            // Try to read the metadata configuration file
            string app_start_config = Application.StartupPath + "\\config";
            if ((Directory.Exists(app_start_config)) && (File.Exists(app_start_config + "\\sobekCM_metadata.config")))
            {
                Metadata_Configuration.Read_Metadata_Configuration(app_start_config + "\\sobekCM_metadata.config");
            }


	        bool complete_static_rebuild = false;
            bool marc_rebuild = false;
            bool run_preloader = true;
            bool run_background = false;
            bool show_help = false;
            bool build_production_marcxml_feed = false;
            bool build_test_marcxml_feed = false;
            string invalid_arg = String.Empty;
            bool refresh_oai = false;
            bool verbose = false;

	        // Get values from the arguments
            foreach (string thisArgs in args)
            {
                bool arg_handled = false;

                // Check for the config flag
                if (thisArgs == "--config")
                {
                    if (File.Exists(app_start_config + "\\SobekCM_Builder_Configuration.exe"))
                    {
                        Process.Start(app_start_config + "\\SobekCM_Builder_Configuration.exe");
                        return;
                    }
	                Console.WriteLine("ERROR: Unable to find configuration executable file!!");
	                return;
                }

                // Check for versioning option
                if (thisArgs == "--version")
                {
                    Console.WriteLine("You are running version " + SobekCM_Library_Settings.CURRENT_BUILDER_VERSION + " of the SobekCM Builder.");
                    return;
                }

                // Check for no loader flag
                if (thisArgs == "--background")
                {
                    run_background = true;
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
                    refresh_oai = true;
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
                builder.Append("  --config\tRuns the configuration tool\n\n");
                builder.Append("  --version\tDisplays the current version of the SobekCM Builder\n\n");
                builder.Append("  --verbose\tFlag indicates to be verbose in the logs and console\n\n");
                builder.Append("  --background\tContinues to run in the background\n\n");
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
                Console.Write("Would you like to run the configuration tool? [Y/N]: ");
                string result = Console.ReadLine().ToUpper();
                if ((result == "Y") || (result == "YES"))
                {
                    // Does the config app exist?
                    if (File.Exists(Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe"))
                    {
                        // Run the config app
                        Process configProcess = new Process {StartInfo = {FileName = Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe"}};
	                    configProcess.Start();
                        configProcess.WaitForExit();

                        // If still no config file, just abort
                        if (!File.Exists(config_file))
                        {
                            Console.WriteLine("Execution aborted due to missing configuration file.");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Unable to find configuration executable file!!");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Execution aborted due to missing configuration file.");
                    return;
                }
            }

            // Should be a config file now, so read it
            SobekCM_Library_Settings.Read_Configuration_File(config_file);
            if (( SobekCM_Library_Settings.Database_Connections.Count == 0 ) || (SobekCM_Library_Settings.Database_Connections[0].Connection_String.Length == 0))
            {
                Console.WriteLine("Missing database connection string!!\n");
                Console.Write("Would you like to run the configuration tool? [Y/N]: ");
                string result = Console.ReadLine().ToUpper();
                if ((result == "Y") || (result == "YES"))
                {
                    // Does the config app exist?
                    if (File.Exists(Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe"))
                    {
                        // Run the config app
                        Process configProcess = new Process {StartInfo = {FileName = Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe"}};
	                    configProcess.Start();
                        configProcess.WaitForExit();

                        // If still no config file, just abort
                        if (!File.Exists(config_file))
                        {
                            Console.WriteLine("Execution aborted due to missing configuration file.");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Unable to find configuration executable file!!");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Execution aborted due to missing configuration file.");
                    return;
                }
            }

            // Assign the connection string and test the connection (if only a single connection listed)
	        if (SobekCM_Library_Settings.Database_Connections.Count == 1)
	        {
		        SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connections[0].Connection_String;
		        if (!SobekCM_Database.Test_Connection())
		        {
			        Console.WriteLine("Unable to connect to the database using provided connection string:");
			        Console.WriteLine();
			        Console.WriteLine(SobekCM_Database.Connection_String);
			        Console.WriteLine();
			        Console.WriteLine("Run this application with an argument of '--config' to launch the configuration tool.");
			        return;
		        }
	        }

            // Verify connectivity and rights on the logs subfolder
            SobekCM_Library_Settings.Local_Log_Directory = Application.StartupPath + "\\logs";
			if (!Directory.Exists(SobekCM_Library_Settings.Local_Log_Directory))
            {
                try
                {
					Directory.CreateDirectory(SobekCM_Library_Settings.Local_Log_Directory);
                }
                catch
                {
                    Console.WriteLine("Error creating necessary logs subfolder under the application folder.\n");
                    Console.WriteLine("Please create manually.\n");
					Console.WriteLine(SobekCM_Library_Settings.Local_Log_Directory);
                    return;
                }
            }
            try
            {
				StreamWriter testWriter = new StreamWriter(SobekCM_Library_Settings.Local_Log_Directory + "\\test.log", false);
                testWriter.WriteLine("TEST");
                testWriter.Flush();
                testWriter.Close();

				File.Delete(SobekCM_Library_Settings.Local_Log_Directory + "\\test.log");
            }
            catch
            {
                Console.WriteLine("The service account needs modify rights on the logs subfolder.\n");
                Console.WriteLine("Please correct manually.\n");
				Console.WriteLine(SobekCM_Library_Settings.Local_Log_Directory);
                return;
            }

            // Look for Ghostscript from the registry, if not provided in the config file
            if (SobekCM_Library_Settings.Ghostscript_Executable.Length == 0)
            {
                // LOOK FOR THE GHOSTSCRIPT DIRECTORY
                string possible_ghost = Look_For_Variable_Registry_Key("SOFTWARE\\GPL Ghostscript", "GS_DLL");
                if (!String.IsNullOrEmpty(possible_ghost))
                   SobekCM_Library_Settings.Ghostscript_Executable = possible_ghost;
            }

            // Look for Imagemagick from the registry, if not provided in the config file
            string possible_imagemagick = Look_For_Variable_Registry_Key("SOFTWARE\\ImageMagick", "BinPath");
            if (!String.IsNullOrEmpty(possible_imagemagick))
                SobekCM_Library_Settings.ImageMagick_Executable = possible_imagemagick;

            // If this is to refresh the OAI, don't use the worker controller
            if (( refresh_oai ) &&  (SobekCM_Library_Settings.Database_Connections.Count == 1))
            {
                // Set the item for the current mode
                int successes = 0;

                DataTable item_list_table = SobekCM_Database.Get_All_Groups_First_VID( );
                foreach (DataRow thisRow in item_list_table.Rows)
                {
                    string bibid = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();
                    int groupid = Convert.ToInt32(thisRow["groupid"]);

                    string directory = SobekCM_Library_Settings.Image_Server_Network + bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8, 2) + "\\" + vid;
                    string mets = directory + "\\" + bibid + "_" + vid + ".mets.xml";

                    try
                    {
                        SobekCM_Item thisItem = SobekCM_Item.Read_METS(mets);
                        if (thisItem != null)
                        {
                            // Get the OAI-PMH dublin core information
                            StringBuilder oaiDataBuilder = new StringBuilder(1000);
                            StringWriter writer = new StringWriter(oaiDataBuilder);
                            DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(writer, thisItem.Bib_Info);
                            // Also add the URL as identifier
                            oaiDataBuilder.AppendLine("<dc:identifier>" + SobekCM_Library_Settings.System_Base_URL + bibid + "</dc:identifier>");
                            Resource_Object.Database.SobekCM_Database.Save_Item_Group_OAI(groupid, oaiDataBuilder.ToString(), "oai_dc", true);
                            writer.Flush();
                            writer.Close();

                            successes++;
                            if (successes%1000 == 0)
                                Console.WriteLine(@"{0} complete", successes);
                        }
                    }
                    catch
                    {

                    }
                }
                return;
            }

            // Two ways to run this... constantly in background or once
            Worker_Controller controller = new Worker_Controller(verbose);
            if (!run_background)
                controller.Execute_Immediately(build_production_marcxml_feed, build_test_marcxml_feed, run_preloader, complete_static_rebuild, marc_rebuild );
            else
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
