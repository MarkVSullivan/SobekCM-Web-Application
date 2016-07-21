using System;
using System.Collections.Generic;
using System.Reflection;
using SobekCM.Engine_Library.ApplicationState;


namespace SobekCM.Library.Citation.SectionWriter
{
    /// <summary> Class is used to load a section writer for displaying the citation </summary>
    public class SectionWriter_Factory
    {
        private static Dictionary<string, iCitationSectionWriter> writers;
 
        /// <summary> Return a build section writer, used for displaying a portion of the citation </summary>
        /// <param name="Assembly"> Assembly from which to load the section writer, or null/empty</param>
        /// <param name="Class"> Fully qualified class name </param>
        /// <returns> Built citation section writer </returns>
        public static iCitationSectionWriter GetSectionWriter(string AssemblyName, string Class)
        {
            // Was the writers dictionary declared?
            if (writers == null)
                writers = new Dictionary<string, iCitationSectionWriter>();

            // If there was no assembly listed, try to find a match in the existing template elements
            if (String.IsNullOrEmpty(AssemblyName))
            {
                // Look in the dictionary, just by class
                if (writers.ContainsKey(Class))
                    return writers[Class];

                // Was a namespace not included?  All elements in the base assemblies should have one
                string className = (Class.IndexOf(".") < 0) ? "SobekCM.Library.Citation.SectionWriter." + Class : Class;

                // Look for a standard match
                iCitationSectionWriter returnValue = null;
                switch (className)
                {
                    case "SobekCM.Library.Citation.SectionWriter.Coordinates_SectionWriter":
                        returnValue = new Coordinates_SectionWriter();
                        break;
                    case "SobekCM.Library.Citation.SectionWriter.Creator_SectionWriter":
                        returnValue = new Creator_SectionWriter();
                        break;
                    case "SobekCM.Library.Citation.SectionWriter.Rights_SectionWriter":
                        returnValue = new Rights_SectionWriter();
                        break;
                    case "SobekCM.Library.Citation.SectionWriter.SpatialCoverage_SectionWriter":
                        returnValue = new SpatialCoverage_SectionWriter();
                        break;
                    case "SobekCM.Library.Citation.SectionWriter.UserTags_SectionWriter":
                        returnValue = new UserTags_SectionWriter();
                        break;
                }

                // Was it found?
                if (returnValue != null)
                {
                    writers[Class] = returnValue;
                    return returnValue;
                }

                // If it made it here, there is no assembly, but it is an unexpected type.  
                // Just create it from the same assembly then
                try
                {
                    Assembly dllAssembly = System.Reflection.Assembly.GetCallingAssembly();
                    Type elementType = dllAssembly.GetType(Class);
                    returnValue = (iCitationSectionWriter)Activator.CreateInstance(elementType);

                    if (returnValue != null) writers[Class] = returnValue;

                    return returnValue;
                }
                catch (Exception)
                {
                    // Not sure exactly what to do here, honestly
                    return null;
                }
            }

            // An assembly was indicated, look in dicationry with that
            if (writers.ContainsKey(AssemblyName + "|" + Class))
                return writers[AssemblyName + "|" + Class];

            try
            {
                Assembly dllAssembly = null;
                string assemblyFilePath = Engine_ApplicationCache_Gateway.Configuration.Extensions.Get_Assembly(AssemblyName);
                if (assemblyFilePath != null)
                {
                    dllAssembly = Assembly.LoadFrom(assemblyFilePath);
                }
                Type elementType = dllAssembly.GetType(Class);
                iCitationSectionWriter returnObj = (iCitationSectionWriter)Activator.CreateInstance(elementType);

                if (returnObj != null) writers[AssemblyName + "|" + Class] = returnObj;

                return returnObj;
            }
            catch (Exception)
            {
                // Not sure exactly what to do here, honestly
                return null;
            }
        }
    }
}
