using System;
using System.Collections.Generic;

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Settings necessary for the digital resource library to operate, such as
    /// all the metadata configuration (or the default) </summary>
    public static class ResourceObjectSettings
    {
        private static Dictionary<string, string> assemblyDictionary;


        /// <summary> Configuration information regarding how to read and write metadata files in the system </summary>
        public static Metadata_Configuration MetadataConfig { get; set; }

        /// <summary> Constructor for a new instance of the ResourceObjectSettings class </summary>
        static ResourceObjectSettings()
        {
            MetadataConfig = new Metadata_Configuration();
        }

        /// <summary> Add an external assembly, likely from an extension or plug-in </summary>
        /// <param name="AssemblyCode"> Code for this assembly </param>
        /// <param name="AssemblyFilePath"> File path for the DLL assembly to be referenced </param>
        public static void Add_Assembly(string AssemblyCode, string AssemblyFilePath)
        {
            if (assemblyDictionary == null)
                assemblyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            assemblyDictionary[AssemblyCode] = AssemblyFilePath;
        }

        /// <summary> Gets the absolute path and filename for an assembly included in one of the 
        /// extensions, by extension ID </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string Get_Assembly(string ID)
        {
            // Now look and return the assembly if the ID exists
            if ((assemblyDictionary != null) && (assemblyDictionary.ContainsKey(ID)))
                return assemblyDictionary[ID];

            return null;
        }

        /// <summary> Clears the dictionary of assemblies </summary>
        public static void Clear_Assemblies()
        {
            assemblyDictionary.Clear();
        }
    }
}
