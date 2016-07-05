using System.Collections.Generic;

namespace SobekCM.Builder_Library.Settings
{
    /// <summary> Basic setting and configuration information used by the builder
    /// across all instances that it may be processing </summary>
    public static class MultiInstance_Builder_Settings
    {
        /// <summary> Static constructor for the MultiInstance_Builder_Settings class </summary>
        static MultiInstance_Builder_Settings()
        {
            Instance_Package_Limit = -1;
            Instances = new List<Single_Instance_Configuration>();
        }

        /// <summary> Maximum number of packages to process for each instance, before moving onto the 
        /// instance  </summary>
        /// <remarks> -1 is the default value and indicates no limit </remarks>
        public static int Instance_Package_Limit { get; set; }

        /// <summary> ImageMagick executable file </summary>
        public static string ImageMagick_Executable { get; set; }

        /// <summary> Ghostscript executable file </summary>
        public static string Ghostscript_Executable { get; set; }

        /// <summary> Tesseract executable file </summary>
        public static string Tesseract_Executable { get; set; }

        /// <summary> List of all the SobekCM instances supported by this builder </summary>
        public static List<Single_Instance_Configuration> Instances { get; set; }

        /// <summary> List of any reading errors which may have occurred </summary>
        public static List<string> ReadingError { get; set; }

        /// <summary> Number of seconds between polls, from the configuration file (not the database) </summary>
        /// <remarks> This is used if the SobekCM Builder is working between multiple instances. If the SobekCM
        /// Builder is only servicing a single instance, then the data can be pulled from the database. </remarks>
        public static int? Override_Seconds_Between_Polls { get; set; }

        /// <summary> Add information about a new error encountered while reading the config file </summary>
        /// <param name="Error"> Error to log in this settings objects </param>
        public static void Add_Error(string Error)
        {
            if (ReadingError == null) ReadingError = new List<string>();

            ReadingError.Add(Error);
        }

        /// <summary> Clear the collections </summary>
        public static void Clear()
        {
            if (ReadingError != null) ReadingError.Clear();
            ReadingError = null;

            Instances.Clear();
        }

    }
}
