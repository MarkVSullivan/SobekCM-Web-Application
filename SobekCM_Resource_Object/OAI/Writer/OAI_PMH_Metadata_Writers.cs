using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Resource_Object.Metadata_Modules;

namespace SobekCM.Resource_Object.OAI.Writer
{
    public static class OAI_PMH_Metadata_Writers
    {
        private static readonly List<Tuple<string, iOAI_PMH_Metadata_Type_Writer>> writers;

        static OAI_PMH_Metadata_Writers()
        {
            writers = new List<Tuple<string, iOAI_PMH_Metadata_Type_Writer>>();

            // Set default reader/writer values to have a baseline in case there is no file to be read 
            Set_Default_Values();
        }

        /// <summary> Sets the defaut OAI-PMH settings, which is only included OAI_DC </summary>
        public static void Set_Default_Values()
        {
            writers.Clear();

            // Add OAI DC as the default writer
            DC_OAI_Metadata_Type_Writer oai_dc_writer = new DC_OAI_Metadata_Type_Writer();
            writers.Add(new Tuple<string, iOAI_PMH_Metadata_Type_Writer>("oai_dc", oai_dc_writer));

            // Add MarcXML as another default writer

        }

        /// <summary> Clear the list of OAI-PMH writers </summary>
        public static void Clear()
        {
            writers.Clear();
        }

        /// <summary> Add an OAI-PMH writer to this collection of writers for this instance </summary>
        /// <param name="Prefix"> Metadata prefix dispayed publicly and used by harvesters to indicate this type of metadata is requested </param>
        /// <param name="Assembly"> Assembly in which this class resdes, if not a standard, included metadata format </param>
        /// <param name="Namespace"> Namspace in which the class used to create the metadata for this format is stored </param>
        /// <param name="Class"> Class which does the actual metadata format writing </param>
        /// <returns> TRUE if this writer is added succesfully, otherwise FALSE </returns>
        public static bool Add_Writer( string Prefix, string Assembly, string Namespace, string Class )
        {
            try
            {
                // Using reflection, create an object from the class namespace/name
                //     System.Reflection.Assembly dllAssembly = System.Reflection.Assembly.LoadFrom("SobekCM_Bib_Package_3_0_5.dll");
                Assembly dllAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                //  Assembly dllAssembly = Assembly..LoadFrom( Code_Assembly );
                Type readerWriterType = dllAssembly.GetType(Namespace + "." + Class);
                object possibleModule = Activator.CreateInstance(readerWriterType);
                iOAI_PMH_Metadata_Type_Writer module = possibleModule as iOAI_PMH_Metadata_Type_Writer;

                if (module == null)
                {
                    return false;
                }

                writers.Add(new Tuple<string, iOAI_PMH_Metadata_Type_Writer>(Prefix, module));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the list of metadata records associated with a single ite </summary>
        /// <param name="ThisItem"> Item to create the metadata records for </param>
        /// <param name="Options"> Options used possibly during the saving process </param>
        /// <returns> List of the OAI metadata prefixes and the associated metadata records </returns>
        public static List<Tuple<string, string>> Get_OAI_PMH_Metadata_Records(SobekCM_Item ThisItem, Dictionary<string, object> Options)
        {
            List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();

            foreach (Tuple<string, iOAI_PMH_Metadata_Type_Writer> thisWriter in writers)
            {
                try
                {
                    string error_message;
                    string record = thisWriter.Item2.Create_OAI_PMH_Metadata(ThisItem, Options, out error_message);
                    returnValue.Add(new Tuple<string, string>(thisWriter.Item1, record));
                }
                catch {  }
            }


            return returnValue;
        }
    }
}
