#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    public class ETD_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            Thesis_Dissertation_Info thesisInfo = METS_Item.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
            if ((thesisInfo == null) || (!thesisInfo.hasData))
                return false;
            return true;
        }

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            Thesis_Dissertation_Info thesisInfo = METS_Item.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
            if ((thesisInfo == null) || (!thesisInfo.hasData))
                return true;

            Output_Stream.WriteLine("<palmm:thesis>");
            if (!String.IsNullOrEmpty(thesisInfo.Committee_Chair))
                Output_Stream.WriteLine("<palmm:committeeChair>" + base.Convert_String_To_XML_Safe(thesisInfo.Committee_Chair) + "</palmm:committeeChair>");
            if (!String.IsNullOrEmpty(thesisInfo.Committee_Co_Chair))
                Output_Stream.WriteLine("<palmm:committeeCoChair>" + base.Convert_String_To_XML_Safe(thesisInfo.Committee_Co_Chair) + "</palmm:committeeCoChair>");
            if (thesisInfo.Committee_Members_Count > 0)
            {
                foreach (string thisCommitteeMember in thesisInfo.Committee_Members)
                {
                    Output_Stream.WriteLine("<palmm:committeeMember>" + base.Convert_String_To_XML_Safe(thisCommitteeMember) + "</palmm:committeeMember>");
                }
            }
            if (thesisInfo.Graduation_Date.HasValue)
            {
                string encoded_date = thesisInfo.Graduation_Date.Value.Year + "-" + thesisInfo.Graduation_Date.Value.Month.ToString().PadLeft(2, '0') + "-" + thesisInfo.Graduation_Date.Value.Day.ToString().PadLeft(2, '0');
                Output_Stream.WriteLine("<palmm:graduationDate>" + encoded_date + "</palmm:graduationDate>");
            }
            if (!String.IsNullOrEmpty(thesisInfo.Degree))
                Output_Stream.WriteLine("<palmm:degree>" + base.Convert_String_To_XML_Safe(thesisInfo.Degree) + "</palmm:degree>");
            if (!String.IsNullOrEmpty(thesisInfo.Degree_Discipline))
                Output_Stream.WriteLine("<palmm:degreeDiscipline>" + base.Convert_String_To_XML_Safe(thesisInfo.Degree_Discipline) + "</palmm:degreeDiscipline>");
            if (!String.IsNullOrEmpty(thesisInfo.Degree_Grantor))
                Output_Stream.WriteLine("<palmm:degreeGrantor>" + base.Convert_String_To_XML_Safe(thesisInfo.Degree_Grantor) + "</palmm:degreeGrantor>");
            if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters)
                Output_Stream.WriteLine("<palmm:degreeLevel>Masters</palmm:degreeLevel>");
            if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate)
                Output_Stream.WriteLine("<palmm:degreeLevel>Doctorate</palmm:degreeLevel>");
            Output_Stream.WriteLine("</palmm:thesis>");
            return true;
        }

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists
            Thesis_Dissertation_Info thesisInfo = Return_Package.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
            if (thesisInfo == null)
            {
                thesisInfo = new Thesis_Dissertation_Info();
                Return_Package.Add_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY, thesisInfo);
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
                    if (name.IndexOf("palmm:") == 0)
                        name = name.Substring(6);

                    switch (name)
                    {
                        case "committeechair":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                thesisInfo.Committee_Chair = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "committeecochair":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                thesisInfo.Committee_Co_Chair = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "committeemember":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                thesisInfo.Add_Committee_Member(Input_XmlReader.Value);
                            }
                            break;

                        case "graduationdate":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                DateTime convertedDate;
                                if (DateTime.TryParse(Input_XmlReader.Value, out convertedDate))
                                {
                                    thesisInfo.Graduation_Date = convertedDate;
                                }
                            }
                            break;

                        case "degree":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                thesisInfo.Degree = Input_XmlReader.Value;
                            }
                            break;

                        case "degreediscipline":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                thesisInfo.Degree_Discipline = Input_XmlReader.Value;
                            }
                            break;

                        case "degreegrantor":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                thesisInfo.Degree_Grantor = Input_XmlReader.Value;
                            }
                            break;

                        case "degreelevel":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string temp = Input_XmlReader.Value.ToLower();
                                if (temp == "doctorate")
                                    thesisInfo.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate;
                                if (temp == "masters")
                                    thesisInfo.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters;
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
            Thesis_Dissertation_Info thesisInfo = METS_Item.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
            if (thesisInfo == null)
                return false;

            return thesisInfo.hasData;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] {"palmm=\"http://www.fcla.edu/dls/md/palmm/\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[] {"    http://www.fcla.edu/dls/md/palmm/\r\n    http://www.fcla.edu/dls/md/palmm.xsd"};
        }

        #endregion
    }
}