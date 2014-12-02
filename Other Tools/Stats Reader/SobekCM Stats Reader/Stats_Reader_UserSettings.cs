#region Using directives

using SobekCM.Tools.Settings;

#endregion

namespace SobekCM_Stats_Reader
{
    /// <summary> Stats_Reader_UserSettings is a static class which holds all of the settings for this 
    /// particular user and assemply in the Isolated Storage. </summary>
    public class Stats_Reader_UserSettings : IS_UserSettings
    {
        /// <summary> Name of the XML file used to store the QC settings </summary>
        private const string FILE_NAME = "SobekCM_Stats_Reader_UserSettings";

        /// <summary> Static constructor for the SMaRT_UserSettings class. </summary>
        static Stats_Reader_UserSettings()
        {
            Load();
        }

        /// <summary> Location where the IIS web logs were last read </summary>
        public static string IIS_Web_Log_Directory
        {
            get
            {
                return Get_String_Setting("IIS_Web_Log_Directory");
            }
            set
            {
                Add_Setting("IIS_Web_Log_Directory", value);
            }
        }

        /// <summary> Location where the sql output files were last written </summary>
        public static string SQL_Output_Directory
        {
            get
            {
                return Get_String_Setting("SQL_Output_Directory");
            }
            set
            {
                Add_Setting("SQL_Output_Directory", value);
            }
        }

        /// <summary> Location where the temporary datasets were last written </summary>
        public static string DataSet_Directory
        {
            get
            {
                return Get_String_Setting("DataSet_Directory");
            }
            set
            {
                Add_Setting("DataSet_Directory", value);
            }
        }

        /// <summary> Database connection string </summary>
        public static string Database_Connection_String
        {
            get
            {
                return Get_String_Setting("Database_Connection_String");
            }
            set
            {
                Add_Setting("Database_Connection_String", value);
            }
        }

        /// <summary> Network location where the web application resides </summary>
        public static string SobekCM_Web_Application_Directory
        {
            get
            {
                return Get_String_Setting("SobekCM_Web_Application_Directory");
            }
            set
            {
                Add_Setting("SobekCM_Web_Application_Directory", value);
            }
        }

        /// <summary> Load the individual user settings </summary>
        public static void Load()
        {
            // Try to read the XML file from isolated storage
            Read_XML_File(FILE_NAME);
        }

        /// <summary> Save the individual user settings </summary>
        public static void Save()
        {
            // Ask the base class to save the data
            Write_XML_File(FILE_NAME);
        }
    }
}
