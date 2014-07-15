#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    public class DAITSS_METS_amdSec_ReaderWriter : iPackage_amdSec_ReaderWriter
    {
        #region iPackage_amdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write an amdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_amdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            DAITSS_Info daitssInfo = METS_Item.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
            if ((daitssInfo == null) || (!daitssInfo.hasData))
                return false;
            return true;
        }

        /// <summary> Writes the amdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_amdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            DAITSS_Info daitssInfo = METS_Item.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
            if ((daitssInfo == null) || (!daitssInfo.hasData))
                return true;

            Output_Stream.WriteLine("<daitss:daitss>");

            if (!String.IsNullOrEmpty(daitssInfo.SubAccount))
            {
                Output_Stream.WriteLine("<daitss:AGREEMENT_INFO ACCOUNT=\"" + daitssInfo.Account + "\" SUB_ACCOUNT=\"" + daitssInfo.SubAccount + "\" PROJECT=\"" + daitssInfo.Project + "\"/>");
            }
            else
            {
                Output_Stream.WriteLine("<daitss:AGREEMENT_INFO ACCOUNT=\"" + daitssInfo.Account + "\" PROJECT=\"" + daitssInfo.Project + "\"/>");
            }

            Output_Stream.WriteLine("</daitss:daitss>");

            return true;
        }

        /// <summary> Reads the amdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        public bool Read_amdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists
            DAITSS_Info daitssInfo = Return_Package.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
            if (daitssInfo == null)
            {
                daitssInfo = new DAITSS_Info();
                Return_Package.Add_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY, daitssInfo);
            }

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && ((Input_XmlReader.Name == "METS:mdWrap") || (Input_XmlReader.Name == "mdWrap")))
                    return true;

                // get the right division information based on node type
                switch (Input_XmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if ((Input_XmlReader.Name.ToLower() == "daitss:agreement_info") && (Input_XmlReader.HasAttributes))
                        {
                            if (Input_XmlReader.MoveToAttribute("ACCOUNT"))
                                daitssInfo.Account = Input_XmlReader.Value;

                            if (Input_XmlReader.MoveToAttribute("SUB_ACCOUNT"))
                                daitssInfo.SubAccount = Input_XmlReader.Value;

                            if (Input_XmlReader.MoveToAttribute("PROJECT"))
                                daitssInfo.Project = Input_XmlReader.Value;
                        }
                        break;
                }
            } while (Input_XmlReader.Read());

            // Return false since this read all the way to the end of the steam
            return false;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            DAITSS_Info daitssInfo = METS_Item.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
            if (daitssInfo == null)
                return false;
            return daitssInfo.hasData;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] {"daitss=\"http://www.fcla.edu/dls/md/daitss/\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[] {"    http://www.fcla.edu/dls/md/daitss/\r\n    http://www.fcla.edu/dls/md/daitss/daitss.xsd"};
        }

        #endregion
    }
}