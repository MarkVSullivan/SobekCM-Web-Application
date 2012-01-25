using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package.Writers
{
    /// <summary> Class to write the bibliographic information for a digital resource as a stand-alone Dublin Core file </summary>
    /// <remarks> This class adds a wrapper around the output from the <see cref="SobekCM.Bib_Package.Writers.SubWriters.Dublin_Core_SubWriter"/> class. </remarks>
    public class Dublin_Core_Writer
    {
        /// <summary> Write the digital resource information as a dublin core file </summary>
        /// <param name="fileName"> Complete name for the file </param>
        /// <param name="package"> Digital resource object </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_Dublin_Core(string fileName, SobekCM_Item package )
        {
            try
            {
                // Start to build the XML result
                StringBuilder results = new StringBuilder();
                results.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                results.Append("<records>\r\n");
                results.Append("<record xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\">\r\n");


                System.IO.StringWriter string_writer = new System.IO.StringWriter(results);
                SubWriters.Dublin_Core_SubWriter.Add_Simple_Dublin_Core(string_writer, package);

                results.Append("</record>\r\n");
                results.Append("</records>\r\n");

                System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, false);
                writer.Write(results.ToString());
                writer.Flush();
                writer.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Write the digital resource information as a dublin core file </summary>
        /// <param name="fileName"> Complete name for the file </param>
        /// <param name="package"> Digital resource object </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_RDF_Dublin_Core(string fileName, SobekCM_Item package)
        {
            try
            {
                // Start to build the XML result
                StringBuilder results = new StringBuilder();
                results.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                results.Append("<!DOCTYPE rdf:RDF SYSTEM \"http://purl.org/dc/schemas/dcmes-xml-20000714.dtd\">\r\n");
                results.Append("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"\r\n");
                results.Append("         xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\r\n");
                results.Append("<rdf:Description>\r\n");

                System.IO.StringWriter string_writer = new System.IO.StringWriter(results);
                SubWriters.Dublin_Core_SubWriter.Add_Simple_Dublin_Core(string_writer, package);

                results.Append("</rdf:Description>\r\n");
                results.Append("</rdf:RDF>\r\n");

                System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, false);
                writer.Write(results.ToString());
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
