using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SobekCM.Bib_Package.Writers.SubWriters;

namespace SobekCM.Bib_Package.Writers
{
    /// <summary> Class to write the bibliographic information for a digital resource as a stand-alone MODS file </summary>
    /// <remarks> This class adds a wrapper around the output from the <see cref="MODS_SubWriter"/> class. </remarks>
    public class MODS_Writer
    {
        /// <summary> Write the digital resource information as a stand-alone MODS file </summary>
        /// <param name="fileName"> Complete name for the file </param>
        /// <param name="package"> Digital resource object </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_MODS(string fileName, SobekCM_Item package)
        {
            try
            {
                // Start to build the XML result
                StringBuilder results = new StringBuilder();
                results.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                string mods_start = "<mods xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"3.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://www.loc.gov/mods/v3\" xsi:schemaLocation=\"http://www.loc.gov/mods/v3 http://www.loc.gov/mods/v3/mods-3-4.xsd\">\r\n";



                System.IO.StringWriter string_writer = new System.IO.StringWriter(results);
                SubWriters.MODS_SubWriter.Add_MODS(string_writer, package);

                 System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, false);
                writer.Write(results.ToString().Replace("<mods:","<").Replace("</mods:","</").Replace("<mods>", mods_start ));
                writer.Flush();
                writer.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
