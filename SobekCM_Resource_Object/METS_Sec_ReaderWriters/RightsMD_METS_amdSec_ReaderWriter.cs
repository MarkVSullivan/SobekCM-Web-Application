#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> RightsMD reader that operates against a single METS section  </summary>
    public class RightsMD_METS_amdSec_ReaderWriter : iPackage_amdSec_ReaderWriter
    {
        #region iPackage_amdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write an amdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_amdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            RightsMD_Info rightsInfo = METS_Item.Get_Metadata_Module( GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY)  as RightsMD_Info;
            if ((rightsInfo == null) || (!rightsInfo.hasData))
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
            RightsMD_Info rightsInfo = METS_Item.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
            if ((rightsInfo == null) || (!rightsInfo.hasData))
                return true;

            if (!String.IsNullOrEmpty(rightsInfo.Version_Statement))
                Output_Stream.WriteLine("<rightsmd:versionStatement>" + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(rightsInfo.Version_Statement) + "</rightsmd:versionStatement>");
            if (!String.IsNullOrEmpty(rightsInfo.Copyright_Statement))
                Output_Stream.WriteLine("<rightsmd:copyrightStatement>" + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(rightsInfo.Copyright_Statement) + "</rightsmd:copyrightStatement>");
            if (rightsInfo.Access_Code != RightsMD_Info.AccessCode_Enum.NOT_SPECIFIED)
            {
                Output_Stream.Write("<rightsmd:accessCode>");
                switch (rightsInfo.Access_Code)
                {
                    case RightsMD_Info.AccessCode_Enum.Public:
                        Output_Stream.Write("public");
                        break;

                    case RightsMD_Info.AccessCode_Enum.Private:
                        Output_Stream.Write("private");
                        break;

                    case RightsMD_Info.AccessCode_Enum.Campus:
                        Output_Stream.Write("campus");
                        break;
                }
                Output_Stream.WriteLine("</rightsmd:accessCode>");
            }
            if (rightsInfo.Has_Embargo_End)
            {
                string encoded_date = rightsInfo.Embargo_End.Year + "-" + rightsInfo.Embargo_End.Month.ToString().PadLeft(2, '0') + "-" + rightsInfo.Embargo_End.Day.ToString().PadLeft(2, '0');

                Output_Stream.WriteLine("<rightsmd:embargoEnd>" + encoded_date + "</rightsmd:embargoEnd>");
            }

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
            RightsMD_Info rightsInfo = Return_Package.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
            if (rightsInfo == null)
            {
                rightsInfo = new RightsMD_Info();
                Return_Package.Add_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY, rightsInfo);
            }

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && ((Input_XmlReader.Name == "METS:mdWrap") || (Input_XmlReader.Name == "mdWrap")))
                    return true;

                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if (name.IndexOf("rightsmd:") == 0)
                        name = name.Substring(9);

                    switch (name)
                    {
                        case "versionstatement":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                rightsInfo.Version_Statement = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "copyrightstatement":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                rightsInfo.Copyright_Statement = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "accesscode":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                rightsInfo.Access_Code = RightsMD_Info.AccessCode_Enum.NOT_SPECIFIED;
                                switch (Input_XmlReader.Value.Trim().ToLower())
                                {
                                    case "public":
                                        rightsInfo.Access_Code = RightsMD_Info.AccessCode_Enum.Public;
                                        break;

                                    case "private":
                                        rightsInfo.Access_Code = RightsMD_Info.AccessCode_Enum.Private;
                                        break;

                                    case "campus":
                                        rightsInfo.Access_Code = RightsMD_Info.AccessCode_Enum.Campus;
                                        break;
                                }
                            }
                            break;

                        case "embargoend":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                DateTime convertedDate;
                                if (DateTime.TryParse(Input_XmlReader.Value, out convertedDate))
                                {
                                    rightsInfo.Embargo_End = convertedDate;
                                }
                            }
                            break;
                    }
                }
            } while (Input_XmlReader.Read());

            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            RightsMD_Info rightsInfo = METS_Item.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
            if (rightsInfo == null)
                return false;

            return rightsInfo.hasData;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] {"rightsmd=\"http://www.fcla.edu/dls/md/rightsmd/\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[] {"    http://www.fcla.edu/dls/md/rightsmd/\r\n    http://www.fcla.edu/dls/md/rightsmd.xsd"};
        }

        #endregion
    }
}