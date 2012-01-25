using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package.Writers
{
    /// <summary> Class is used to output the OAI-PMH file for a given SobekCM_Item  </summary>
    public class OAI_Writer : XML_Writing_Base_Type
    {
        /// <summary> Constructor for a new instance of the OAI-Writer class </summary>
        public OAI_Writer()
        {
            // Do nothing
        }

        /// <summary> Saves a SobekCM Item's metdata as an OAI-PMH source XML file in Dublin Core format </summary>
        /// <param name="package"> Item to save as the source XML file for OAI-PMH / dubliin core</param>
        /// <param name="Output_File_Name"> Name and path of the output file </param>
        /// <param name="OAI_Set"> OAI Set(s) to include in the set informations </param>
        /// <param name="oai_date"> Date for the creation/modification of this OAI information </param>
        public void Save_OAI_File(SobekCM_Item package, string Output_File_Name, string OAI_Set, DateTime oai_date )
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(Output_File_Name, false);
            writer.Write(To_OAI_String(package, OAI_Set, oai_date));
            writer.Flush();
            writer.Close();
        }

        private string To_OAI_String(SobekCM_Item package, string OAI_Set, DateTime oai_date )
        {
            StringBuilder returnValue = new StringBuilder();

            // Add the header for this OAI
            returnValue.Append("<xml>");

            returnValue.Append("<record><header><identifier>oai:www.uflib.ufl.edu.ufdc:" + package.BibID + "</identifier>");
            returnValue.Append("<datestamp>" + oai_date.Year + "-" + oai_date.Month.ToString().PadLeft(2, '0') + "-" + oai_date.Day.ToString().PadLeft(2, '0') + "</datestamp>");
            returnValue.Append("<setSpec>" + OAI_Set + "</setSpec></header>");

            // Start the metadata section with the namespace references
            returnValue.Append("<metadata>");
            returnValue.Append("<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" ");
            returnValue.Append("xmlns:dc=\"http://purl.org/dc/elements/1.1/\" ");
            returnValue.Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            returnValue.Append("xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ ");
            returnValue.Append("http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");

            // Add the URL as the identifier
            Identifier_Info urlIdentifier = new Identifier_Info("<dc:identifier>http://www.uflib.ufl.edu/ufdc/?b=" + package.BibID + "</dc:identifier>");
            if ((package.SobekCM_Web.Web_Skins.Count > 0) && (package.SobekCM_Web.Web_Skins[0].ToUpper() == "DLOC"))
            {
                urlIdentifier.Identifier = "<dc:identifier>http://www.dloc.com/?b=" + package.BibID + "</dc:identifier>";
            }
            package.Bib_Info.Add_Identifier(urlIdentifier);

            // Add the dublin core
            System.IO.StringWriter returnStream = new System.IO.StringWriter(returnValue);
            SubWriters.Dublin_Core_SubWriter.Add_Simple_Dublin_Core(returnStream, package);

            // Now, remove that URL identifier
            package.Bib_Info.Remove_Identifier(urlIdentifier);

            // Finish this OAI
            returnValue.Append("</oai_dc:dc>");
            returnValue.Append("</metadata>");
            returnValue.Append("</record>");

            returnValue.Append("</xml>");

            // Return the built OAI string
            return returnValue.ToString();
        }


    }
}
