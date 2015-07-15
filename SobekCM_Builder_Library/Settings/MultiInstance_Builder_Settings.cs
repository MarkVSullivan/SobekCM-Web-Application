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
        }

        /// <summary> Maximum number of packages to process for each instance, before moving onto the 
        /// instance  </summary>
        /// <remarks> -1 is the default value and indicates no limit </remarks>
        public static int Instance_Package_Limit { get; set; }

        /// <summary> ImageMagick executable file </summary>
        public static string ImageMagick_Executable { get; set; }

        /// <summary> Ghostscript executable file </summary>
        public static string Ghostscript_Executable { get; set; }

    }
}
