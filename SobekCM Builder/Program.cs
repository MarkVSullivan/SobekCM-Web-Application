using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Threading;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Library;
using SobekCM.Library.Database;

namespace SobekCM.Builder
{
    public class Program
    {
 
        static void Main(string[] args)
        {
            // Try to read the metadata configuration file
            string app_start_config = System.Windows.Forms.Application.StartupPath + "\\config";
            if ((Directory.Exists(app_start_config)) && (File.Exists(app_start_config + "\\sobekCM_metadata.config")))
            {
                Resource_Object.Configuration.Metadata_Configuration.Read_Metadata_Configuration(app_start_config + "\\sobekCM_metadata.config");
            }


            string collectionName = String.Empty;
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
            bool arg_handled;

    		// Get values from the arguments
            foreach (string thisArgs in args)
            {
                arg_handled = false;

                // Check for the config flag
                if (thisArgs == "--config")
                {
                    if (File.Exists(app_start_config + "\\SobekCM_Builder_Configuration.exe"))
                    {
                        System.Diagnostics.Process.Start(app_start_config + "\\SobekCM_Builder_Configuration.exe");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Unable to find configuration executable file!!");
                    }
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
            string config_file = System.Windows.Forms.Application.StartupPath + "\\config\\sobekcm.config";
            if (!File.Exists(config_file))
            {
                Console.WriteLine("The configuration file is missing!!\n");
                Console.Write("Would you like to run the configuration tool? [Y/N]: ");
                string result = Console.ReadLine().ToUpper();
                if ((result == "Y") || (result == "YES"))
                {
                    // Does the config app exist?
                    if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe"))
                    {
                        // Run the config app
                        System.Diagnostics.Process configProcess = new System.Diagnostics.Process();
                        configProcess.StartInfo.FileName = System.Windows.Forms.Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe";
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
            if (SobekCM_Library_Settings.Database_Connection_String.Length == 0)
            {
                Console.WriteLine("Missing database connection string!!\n");
                Console.Write("Would you like to run the configuration tool? [Y/N]: ");
                string result = Console.ReadLine().ToUpper();
                if ((result == "Y") || (result == "YES"))
                {
                    // Does the config app exist?
                    if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe"))
                    {
                        // Run the config app
                        System.Diagnostics.Process configProcess = new System.Diagnostics.Process();
                        configProcess.StartInfo.FileName = System.Windows.Forms.Application.StartupPath + "\\config\\SobekCM_Builder_Configuration.exe";
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

            // Assign the connection string and test the connection
            SobekCM.Library.Database.SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connection_String;
            if (!SobekCM.Library.Database.SobekCM_Database.Test_Connection())
            {
                Console.WriteLine("Unable to connect to the database using provided connection string:");
                Console.WriteLine();
                Console.WriteLine(SobekCM.Library.Database.SobekCM_Database.Connection_String);
                Console.WriteLine();
                Console.WriteLine("Run this application with an argument of '--config' to launch the configuration tool.");
                return;
            }

            // Load all the settings
            SobekCM_Library_Settings.Refresh(SobekCM.Library.Database.SobekCM_Database.Get_Settings_Complete(null));

            // Verify connectivity and rights on the logs subfolder
            string logfile_dir = System.Windows.Forms.Application.StartupPath + "\\logs";
            if (!Directory.Exists(logfile_dir))
            {
                try
                {
                    Directory.CreateDirectory(logfile_dir);
                }
                catch
                {
                    Console.WriteLine("Error creating necessary logs subfolder under the application folder.\n");
                    Console.WriteLine("Please create manually.\n");
                    Console.WriteLine(logfile_dir);
                    return;
                }
            }
            try
            {
                StreamWriter testWriter = new StreamWriter(logfile_dir + "\\test.log", false);
                testWriter.WriteLine("TEST");
                testWriter.Flush();
                testWriter.Close();

                File.Delete(logfile_dir + "\\test.log");
            }
            catch
            {
                Console.WriteLine("The service account needs modify rights on the logs subfolder.\n");
                Console.WriteLine("Please correct manually.\n");
                Console.WriteLine(logfile_dir);
                return;
            }

            // If this is to refresh the OAI, don't use the worker controller
            if (refresh_oai)
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
                        SobekCM.Resource_Object.SobekCM_Item thisItem = SobekCM.Resource_Object.SobekCM_Item.Read_METS(mets);
                        if (thisItem != null)
                        {
                            // Get the OAI-PMH dublin core information
                            StringBuilder oaiDataBuilder = new StringBuilder(1000);
                            StringWriter writer = new StringWriter(oaiDataBuilder);
                            Resource_Object.METS_Sec_ReaderWriters.DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(writer, thisItem.Bib_Info);
                            // Also add the URL as identifier
                            oaiDataBuilder.AppendLine("<dc:identifier>" + SobekCM_Library_Settings.System_Base_URL + bibid + "</dc:identifier>");
                            SobekCM.Resource_Object.Database.SobekCM_Database.Save_Item_Group_OAI(groupid, oaiDataBuilder.ToString(), "oai_dc", true);
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
    }
}
