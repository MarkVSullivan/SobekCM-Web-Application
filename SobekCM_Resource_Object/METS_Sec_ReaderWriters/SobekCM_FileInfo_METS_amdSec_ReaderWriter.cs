#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    public class SobekCM_FileInfo_METS_amdSec_ReaderWriter : iPackage_amdSec_ReaderWriter
    {
        private string sobekcm_namespace;

        /// <summary> Constructor for a new instance of the SobekCM_FileInfo_METS_amdSec_ReaderWriter class </summary>
        /// <param name="SobekCM_Namespace">Namespace utilized for this material in the XML file being read</param>
        public SobekCM_FileInfo_METS_amdSec_ReaderWriter(string SobekCM_Namespace)
        {
            sobekcm_namespace = SobekCM_Namespace;
        }

        /// <summary> Constructor for a new instance of the SobekCM_FileInfo_METS_amdSec_ReaderWriter class </summary>
        public SobekCM_FileInfo_METS_amdSec_ReaderWriter()
        {
            sobekcm_namespace = "sobekcm";
        }

        #region iPackage_amdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write an amdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_amdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            List<SobekCM_File_Info> fileList = Options["SobekCM_FileInfo_METS_amdSec_ReaderWriter:All_Files"] as List<SobekCM_File_Info>;
            if (fileList == null) return false;
            return (fileList.Count > 0);
        }

        /// <summary> Writes the amdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This writer REQUIRES the list of all SobekCM files </remarks>
        public bool Write_amdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            List<SobekCM_File_Info> fileList = Options["SobekCM_FileInfo_METS_amdSec_ReaderWriter:All_Files"] as List<SobekCM_File_Info>;
            if (fileList == null) return false;

            // If there ARE no files, just return
            if (fileList.Count == 0)
                return true;

            Output_Stream.WriteLine("<" + sobekcm_namespace + ":FileInfo>");

            // Step through each file
            foreach (SobekCM_File_Info thisFileInfo in fileList)
            {
                if ((thisFileInfo.Height > 0) && (thisFileInfo.Width > 0))
                {
                    Output_Stream.WriteLine("<" + sobekcm_namespace + ":File fileid=\"" + thisFileInfo.ID + "\" width=\"" + thisFileInfo.Width + "\" height=\"" + thisFileInfo.Height + "\" />");
                }
            }

            Output_Stream.WriteLine("</" + sobekcm_namespace + ":FileInfo>");
            return true;
        }

        /// <summary> Reads the amdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        /// <remarks> One option is REQUIRED for this to work, you must pass in  a Dictionary&lt;string,SobekCM_File_Info&gt;
        /// generic dictionary with all the pre-collection file information stored by fileid.  It should be include in the 
        /// Options dictionary under the key 'SobekCM_FileInfo_METS_amdSec_ReaderWriter:Files_By_FileID'.</remarks>
        public bool Read_amdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            Dictionary<string, SobekCM_File_Info> files_by_fileid = null;
            if ((Options == null) || (!Options.ContainsKey("SobekCM_FileInfo_METS_amdSec_ReaderWriter:Files_By_FileID")))
                return false;

            files_by_fileid = (Dictionary<string, SobekCM_File_Info>) Options["SobekCM_FileInfo_METS_amdSec_ReaderWriter:Files_By_FileID"];


            string fileid = String.Empty;

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":FileInfo"))
                    return true;

                // get the right division information based on node type
                switch (Input_XmlReader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (Input_XmlReader.Name == sobekcm_namespace + ":FileInfo")
                            return true;
                        break;

                    case XmlNodeType.Element:
                        if ((Input_XmlReader.Name == sobekcm_namespace + ":File") && (Input_XmlReader.HasAttributes) && (Input_XmlReader.MoveToAttribute("fileid")))
                        {
                            fileid = Input_XmlReader.Value;

                            // Save this information
                            SobekCM_File_Info existingFile = null;
                            if (!files_by_fileid.ContainsKey(fileid))
                            {
                                existingFile = new SobekCM_File_Info(String.Empty);
                                files_by_fileid[fileid] = existingFile;
                            }
                            else
                            {
                                existingFile = files_by_fileid[fileid];
                            }

                            try
                            {
                                if (Input_XmlReader.MoveToAttribute("width"))
                                    existingFile.Width = Convert.ToUInt16(Input_XmlReader.Value);

                                if (Input_XmlReader.MoveToAttribute("height"))
                                    existingFile.Height = Convert.ToUInt16(Input_XmlReader.Value);
                            }
                            catch
                            {
                            }
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
            return false;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] {};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[] {};
        }

        #endregion
    }
}