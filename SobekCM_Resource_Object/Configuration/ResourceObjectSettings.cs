namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Settings necessary for the digital resource library to operate, such as
    /// all the metadata configuration (or the default) </summary>
    public static class ResourceObjectSettings
    {
        /// <summary> Configuration information regarding how to read and write metadata files in the system </summary>
        public static Metadata_Configuration MetadataConfig { get; set; }

        /// <summary> Constructor for a new instance of the ResourceObjectSettings class </summary>
        static ResourceObjectSettings()
        {
            MetadataConfig = new Metadata_Configuration();
        }
    }
}
