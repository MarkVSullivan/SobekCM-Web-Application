using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Readers
{
	/// <summary>Reader reads MARC XML metadata files </summary>
	/// <remarks>MARC XML files are used to hold the bibliographic data downloaded from OCLC <br /> <br />
	/// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class MARC_XML_Reader
	{
		/// <summary> Constructor creates a new instance of the MARC_XML_Reader class </summary>
		public MARC_XML_Reader()
		{
            // Do nothing
		}

		/// <summary> Read the metadata file and enriches the existing bibliographic package </summary>
		/// <param name="MARC_XML_File">MARC XML metadata file</param>
		/// <param name="thisPackage">Bibliographic package to enrich</param>
        public void Read_MARC_XML(string MARC_XML_File, SobekCM_Item thisPackage)
        {
            Stream reader = null;
            XmlTextReader nodeReader = null;
            try
            {

                reader = new FileStream(MARC_XML_File, FileMode.Open, FileAccess.Read);

                // create the node reader
                nodeReader = new XmlTextReader(reader);

                SubReaders.MarcXML_SubReader.Read_MarcXML_Info(nodeReader, thisPackage);
            }
            catch ( Exception ee )
            {
                bool error = true;
            }
            finally
            {
                if (nodeReader != null)
                    nodeReader.Close();
                if (reader != null)
                    reader.Close();
            }         
        }
	}
}
