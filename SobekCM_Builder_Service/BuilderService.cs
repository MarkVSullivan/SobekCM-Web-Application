using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using SobekCM.Builder_Library;
using SobekCM.Builder_Library.Settings;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Library.Database;
using SobekCM.Resource_Object.Configuration;

namespace SobekCM_Builder_Service
{
    public partial class BuilderService : ServiceBase
    {
        public BuilderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Get the application data path 
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SobekCM", "Builder");
            try
            {
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }
            }
            catch (Exception ee)
            {
                EventLog.WriteEntry("SobekCM Builder Service", "Error creating the application data folder, which is missing.\n\n" + appDataPath + "\n\n" + ee.Message, EventLogEntryType.Error );
                return;
            }


            //// Try to read the metadata configuration file
            string app_start_config = appDataPath + "\\config";
            //if ((Directory.Exists(app_start_config)) && (File.Exists(app_start_config + "\\sobekCM_metadata.config")))
            //{
            //    ResourceObjectSettings.MetadataConfig.Read_Metadata_Configuration(app_start_config + "\\sobekCM_metadata.config");
            //}

            // Now, verify the configuration file exists
            string config_file = appDataPath + "\\config\\sobekcm.config";
            if (!File.Exists(config_file))
            {
                EventLog.WriteEntry("SobekCM Builder Service", "The configuration file is missing!!\n\nExecution aborted due to missing configuration file.\n\n" + config_file, EventLogEntryType.Error);
                return;
            }

            // Assign the connection string and test the connection (if only a single connection listed)
            if ( MultiInstance_Builder_Settings.Instances.Count == 1)
            {
                SobekCM_Database.Connection_String = MultiInstance_Builder_Settings.Instances[0].DatabaseConnection.Connection_String;
                if (!SobekCM_Database.Test_Connection())
                {
                    if ( SobekCM_Database.Last_Exception != null )
                        EventLog.WriteEntry("SobekCM Builder Service", "Unable to connect to the database using provided connection string:\n\n"+ SobekCM_Database.Connection_String + "\n\n" + SobekCM_Database.Last_Exception.Message, EventLogEntryType.Error);
                    else
                        EventLog.WriteEntry("SobekCM Builder Service", "Unable to connect to the database using provided connection string:\n\n" + SobekCM_Database.Connection_String, EventLogEntryType.Error);

                    return;
                }
            }

            // Verify connectivity and rights on the logs subfolder
            string logFileDirectory = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().CodeBase, "logs");
            if (!Directory.Exists(logFileDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logFileDirectory);
                }
                catch
                {
                    EventLog.WriteEntry("SobekCM Builder Service", "Error creating necessary logs subfolder under the application folder.\n\nPlease create manually.\n\n" + logFileDirectory, EventLogEntryType.Error);
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
                EventLog.WriteEntry("SobekCM Builder Service", "The service account needs modify rights on the logs subfolder.\n\nPlease create manually.\n\n" + logFileDirectory, EventLogEntryType.Error);
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

            // Two ways to run this... constantly in background or once
            Worker_Controller controller = new Worker_Controller(true, logFileDirectory);
            controller.Execute_In_Background();

            // If this was set to aborting, set to last execution aborted
            Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
            if ((operationFlag == Builder_Operation_Flag_Enum.ABORTING) || (operationFlag == Builder_Operation_Flag_Enum.ABORT_REQUESTED))
                Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
        }

        protected override void OnStop()
        {

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
