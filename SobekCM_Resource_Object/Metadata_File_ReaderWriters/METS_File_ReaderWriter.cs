#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary> Reader/Writer for reading metadata stored in METS files </summary>
    /// <remarks> This relies heavily upon the METS Sec ReaderWriters to read the dmdSec and amdSec
    /// portions of the METS file. </remarks>
    public class METS_File_ReaderWriter : XML_Writing_Base_Type, iMetadata_File_ReaderWriter
    {

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns TRUE </value>
        public bool canRead
        {
            get { return true; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return TRUE </value>
        public bool canWrite
        {
            get { return true; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'Metadata Encoding and Transmission Standard'</value>
        public string Metadata_Type_Name
        {
            get { return "Metadata Encoding and Transmission Standard"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'METS'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "METS"; }
        }

        #region Methods to write the metadata as METS to a file or stream


        /// <summary> Writes the formatted metadata from the provided item to a file </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to write</param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(string MetadataFilePathName, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            StreamWriter results = null;
            bool returnValue;
            try
            {
                results = new StreamWriter(MetadataFilePathName, false, Encoding.UTF8);
                returnValue = Write_Metadata(results, Item_To_Save, Options, out Error_Message);
                
            }
            catch (Exception ee)
            {
                Error_Message = "Error writing METS metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                returnValue = false;
            }
            finally
            {
                if (results != null)
                {
                    results.Flush();
                    results.Close();
                }
            }

            return returnValue;
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks> OPTIONS: Accepts 'METS_File_ReaderWriter:METS_Writing_Profile', otherwise the default METS
        /// writing profile is utilized. </remarks>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // PERHAPS MAKE THESE OPTIONS?
            List<string> mimes_to_exclude = new List<string>();
            const bool MINIMIZE_FILE_SIZE = false;

            // Get the METS writing profile
            METS_Writing_Profile profile = ResourceObjectSettings.MetadataConfig.Default_METS_Writing_Profile;


            // Set default error outpt message
            Error_Message = String.Empty;

            // Ensure the METS ID is set from BibID and VID
            if (Item_To_Save.METS_Header.ObjectID.Length == 0)
            {
                Item_To_Save.METS_Header.ObjectID = Item_To_Save.BibID + "_" + Item_To_Save.VID;
                if (Item_To_Save.VID.Length == 0)
                    Item_To_Save.METS_Header.ObjectID = Item_To_Save.BibID;
            }

            // If this is bib level, it is different
            if ((Item_To_Save.VID == "*****") || (Item_To_Save.METS_Header.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL))
                Item_To_Save.METS_Header.ObjectID = Item_To_Save.BibID;

            // If this is juat a project XML, do this differently
            if (Item_To_Save.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project)
            {
                Item_To_Save.Bib_Info.Main_Title.Title = "Project level metadata for '" + Item_To_Save.BibID + "'";
                Item_To_Save.METS_Header.ObjectID = Item_To_Save.BibID;
            }

            // Ensure the source code appears in the METS header correctly
            Item_To_Save.METS_Header.Creator_Organization = Item_To_Save.Bib_Info.Source.Code;
            if ((Item_To_Save.Bib_Info.Source.Statement.Length > 0) && (String.Compare(Item_To_Save.Bib_Info.Source.Statement, Item_To_Save.Bib_Info.Source.Code, StringComparison.OrdinalIgnoreCase) != 0))
            {
                Item_To_Save.METS_Header.Creator_Organization = Item_To_Save.Bib_Info.Source.Code + "," + Item_To_Save.Bib_Info.Source.XML_Safe_Statement;
            }

            // Get the list of divisions from the physical tree and the download/other file tre
            List<abstract_TreeNode> physicalDivisions = Item_To_Save.Divisions.Physical_Tree.Divisions_PreOrder;
            List<abstract_TreeNode> downloadDivisions = Item_To_Save.Divisions.Download_Tree.Divisions_PreOrder;
            List<abstract_TreeNode> allDivisions = new List<abstract_TreeNode>();
            List<SobekCM_File_Info> allFiles = new List<SobekCM_File_Info>();

            #region Prepare the list of divisions and files to be written by assigning proper ID's and groupid's to all

            // Create some private variables for this
            int page_and_group_number = 1;
            int division_number = 1;
            bool hasPageFiles = false;
            bool hasDownloadFiles = false;
            Dictionary<string, List<SobekCM_File_Info>> mimeHash = new Dictionary<string, List<SobekCM_File_Info>>();
            List<string> fileids_used = new List<string>();

            // Clear any existing ID's here since the ID's are only used for writing METS files
            foreach (abstract_TreeNode thisNode in physicalDivisions)
            {
                thisNode.ID = String.Empty;
                thisNode.DMDID = String.Empty;
                thisNode.ADMID = String.Empty;
            }

            // First, assign group numbers for all the files on each page (physical or other)
            // Group numbers in the METS file correspond to files on the same page (physical or other)
            // At the same time, we will build the list of all files and files by mime type
            foreach (abstract_TreeNode thisNode in physicalDivisions)
            {
                // If this node was already hit (perhaps if a div has two parent divs), 
                // then skip it
                if (thisNode.ID.Length > 0)
                    continue;

                // Add this to the list of all nodes
                allDivisions.Add(thisNode);

                // Was this a PAGE or a DIVISION node?
                if (thisNode.Page)
                {
                    // Get this page node
                    Page_TreeNode pageNode = (Page_TreeNode) thisNode;

                    // Only do anything if there are actually any files here
                    if ((pageNode.Files != null) && (pageNode.Files.Count > 0))
                    {

                        // Set the page ID here
                        pageNode.ID = "PAGE" + page_and_group_number;

                        // Step through any files under this page
                        foreach (SobekCM_File_Info thisFile in pageNode.Files)
                        {
                            // Set the ADMID and DMDID to empty in preparation for METS writing
                            thisFile.ADMID = String.Empty;
                            thisFile.DMDID = String.Empty;

                            // If no file name, skip this
                            if (String.IsNullOrEmpty(thisFile.System_Name))
                                continue;

                            // Get this file extension and MIME type
                            string fileExtension = thisFile.File_Extension;
                            string mimetype = thisFile.MIME_Type(thisFile.File_Extension);

                            // If this is going to be excluded from appearing in the METS file, just skip 
                            // it here as well.
                            if (!mimes_to_exclude.Contains(mimetype))
                            {
                                // Set the group number on this file
                                thisFile.Group_Number = "G" + page_and_group_number;

                                // Set the ID for this file as well
                                switch (mimetype)
                                {
                                    case "image/tiff":
                                        thisFile.ID = "TIF" + page_and_group_number;
                                        break;
                                    case "text/plain":
                                        thisFile.ID = "TXT" + page_and_group_number;
                                        break;
                                    case "image/jpeg":
                                        if (thisFile.System_Name.ToLower().IndexOf("thm.jp") > 0)
                                        {
                                            thisFile.ID = "THUMB" + page_and_group_number;
                                            mimetype = mimetype + "-thumbnails";
                                        }
                                        else
                                            thisFile.ID = "JPEG" + page_and_group_number;
                                        break;
                                    case "image/gif":
                                        if (thisFile.System_Name.ToLower().IndexOf("thm.gif") > 0)
                                        {
                                            thisFile.ID = "THUMB" + page_and_group_number;
                                            mimetype = mimetype + "-thumbnails";
                                        }
                                        else
                                            thisFile.ID = "GIF" + page_and_group_number;
                                        break;
                                    case "image/jp2":
                                        thisFile.ID = "JP2" + page_and_group_number;
                                        break;
                                    default:
                                        if (fileExtension.Length > 0)
                                        {
                                            thisFile.ID = fileExtension + page_and_group_number;
                                        }
                                        else
                                        {
                                            thisFile.ID = "NOEXT" + page_and_group_number;
                                        }
                                        break;
                                }

                                // Ensure this fileid is really unique.  It may not be if there are multiple
                                // files of the same mime-type in the same page.  (such as 0001.jpg and 0001.qc.jpg)
                                if (fileids_used.Contains(thisFile.ID))
                                {
                                    int count = 2;
                                    while (fileids_used.Contains(thisFile.ID + "." + count))
                                        count++;
                                    thisFile.ID = thisFile.ID + "." + count;
                                }

                                // Save this file id
                                fileids_used.Add(thisFile.ID);

                                // Also add to the list of files
                                allFiles.Add(thisFile);

                                // Also ensure we know there are page image files
                                hasPageFiles = true;


                                // If this is a new MIME type, add it, else just save this file in the MIME hash
                                if (!mimeHash.ContainsKey(mimetype))
                                {
                                    List<SobekCM_File_Info> newList = new List<SobekCM_File_Info> {thisFile};
                                    mimeHash[mimetype] = newList;
                                }
                                else
                                {
                                    mimeHash[mimetype].Add(thisFile);
                                }
                            }
                        }

                        // Prepare for the next page
                        page_and_group_number++;
                    }
                    else
                    {
                        // Page has no files, so it should be skipped when written
                        pageNode.ID = "SKIP";
                    }
                }
                else
                {
                    // This node is a DIVISION (non-page)
                    thisNode.ID = "PDIV" + division_number;
                    division_number++;
                }
            }


            // Clear any existing ID's here since the ID's are only used for writing METS files
            foreach (abstract_TreeNode thisNode in downloadDivisions)
            {
                thisNode.ID = String.Empty;
                thisNode.DMDID = String.Empty;
                thisNode.ADMID = String.Empty;
            }

            // Now, do the same thing for the download/other files division tree
            // Group numbers in the METS file correspond to files on the same page (physical or other)
            // At the same time, we will build the list of all files and files by mime type
            page_and_group_number = 1;
            division_number = 1;
            foreach (abstract_TreeNode thisNode in downloadDivisions)
            {
                // If this node was already hit (perhaps if a div has two parent divs), 
                // then skip it
                if (thisNode.ID.Length > 0)
                    continue;

                // Add this to the list of all nodes
                allDivisions.Add(thisNode);

                // Was this a PAGE or a DIVISION node?
                if (thisNode.Page)
                {
                    // Get this page node
                    Page_TreeNode pageNode = (Page_TreeNode) thisNode;

                    // Only do anything if there are actually any files here
                    if ((pageNode.Files != null) && (pageNode.Files.Count > 0))
                    {

                        // Set the page ID here
                        pageNode.ID = "FILES" + page_and_group_number;

                        // Step through any files under this page
                        foreach (SobekCM_File_Info thisFile in pageNode.Files)
                        {
                            // Set the ADMID and DMDID to empty in preparation for METS writing
                            thisFile.ADMID = String.Empty;
                            thisFile.DMDID = String.Empty;

                            // If no file name, skip this
                            if (String.IsNullOrEmpty(thisFile.System_Name))
                                continue;

                            // Get this file extension and MIME type
                            string fileExtension = thisFile.File_Extension;
                            string mimetype = thisFile.MIME_Type(thisFile.File_Extension);

                            // If this is going to be excluded from appearing in the METS file, just skip 
                            // it here as well.
                            if (!mimes_to_exclude.Contains(mimetype))
                            {
                                // Set the group number on this file
                                thisFile.Group_Number = "G" + page_and_group_number;

                                // Set the ID for this file as well
                                switch (mimetype)
                                {
                                    case "image/tiff":
                                        thisFile.ID = "TIF" + page_and_group_number;
                                        break;
                                    case "text/plain":
                                        thisFile.ID = "TXT" + page_and_group_number;
                                        break;
                                    case "image/jpeg":
                                        if (thisFile.System_Name.ToLower().IndexOf("thm.jp") > 0)
                                        {
                                            thisFile.ID = "THUMB" + page_and_group_number;
                                            mimetype = mimetype + "-thumbnails";
                                        }
                                        else
                                            thisFile.ID = "JPEG" + page_and_group_number;
                                        break;
                                    case "image/gif":
                                        if (thisFile.System_Name.ToLower().IndexOf("thm.gif") > 0)
                                        {
                                            thisFile.ID = "THUMB" + page_and_group_number;
                                            mimetype = mimetype + "-thumbnails";
                                        }
                                        else
                                            thisFile.ID = "GIF" + page_and_group_number;
                                        break;
                                    case "image/jp2":
                                        thisFile.ID = "JP2" + page_and_group_number;
                                        break;
                                    default:
                                        if (fileExtension.Length > 0)
                                        {
                                            thisFile.ID = fileExtension + page_and_group_number;
                                        }
                                        else
                                        {
                                            thisFile.ID = "NOEXT" + page_and_group_number;
                                        }
                                        break;
                                }

                                // Ensure this fileid is really unique.  It may not be if there are multiple
                                // files of the same mime-type in the same page.  (such as 0001.jpg and 0001.qc.jpg)
                                if (fileids_used.Contains(thisFile.ID))
                                {
                                    int count = 2;
                                    while (fileids_used.Contains(thisFile.ID + "." + count))
                                        count++;
                                    thisFile.ID = thisFile.ID + "." + count;
                                }

                                // Save this file id
                                fileids_used.Add(thisFile.ID);

                                // Also add to the list of files
                                allFiles.Add(thisFile);

                                // Also ensure we know there are page image files
                                hasDownloadFiles = true;


                                // If this is a new MIME type, add it, else just save this file in the MIME hash
                                if (!mimeHash.ContainsKey(mimetype))
                                {
                                    List<SobekCM_File_Info> newList = new List<SobekCM_File_Info> {thisFile};
                                    mimeHash[mimetype] = newList;
                                }
                                else
                                {
                                    mimeHash[mimetype].Add(thisFile);
                                }
                            }
                        }

                        // Prepare for the next page
                        page_and_group_number++;
                    }
                    else
                    {
                        // Page has no files, so it should be skipped when written
                        pageNode.ID = "SKIP";
                    }
                }
                else
                {
                    // This node is a DIVISION (non-page)
                    thisNode.ID = "ODIV" + division_number;
                    division_number++;
                }
            }

            #endregion

            // Add the XML declaration
            Output_Stream.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");

            #region Write some processing instructions requested by Florida SUS's / FLVC (hope to deprecate)

            // Some special code for setting processing parameters
            bool daitssWriterIncluded = false;
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.Package_Level_AmdSec_Writer_Configs)
            {
                if (thisConfig.Code_Class == "DAITSS_METS_amdSec_ReaderWriter")
                {
                    daitssWriterIncluded = true;
                    break;
                }
            }
            if (daitssWriterIncluded)
            {
                DAITSS_Info daitssInfo = Item_To_Save.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
                if (daitssInfo != null)
                {
                    if ((daitssInfo.hasData) && (daitssInfo.toArchive))
                    {
                        Output_Stream.WriteLine("<?fcla fda=\"yes\"?>");
                    }
                    else
                    {
                        Output_Stream.WriteLine("<?fcla fda=\"no\"?>");
                    }
                }
            }

            #endregion

            // Add a remark here with the title and type
            Output_Stream.WriteLine("<!--  " + Item_To_Save.Bib_Info.Main_Title.Title_XML.Replace("-", " ") + " ( " + Item_To_Save.Bib_Info.SobekCM_Type_String + " ) -->");

            // Add the METS declaration information
            Output_Stream.WriteLine("<METS:mets OBJID=\"" + Item_To_Save.METS_Header.ObjectID + "\"");
            Output_Stream.WriteLine("  xmlns:METS=\"http://www.loc.gov/METS/\"");
            Output_Stream.WriteLine("  xmlns:xlink=\"http://www.w3.org/1999/xlink\"");
            Output_Stream.WriteLine("  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");

            #region Add XMLNS and schema locations for active METS section reader/writers with data to write

            // Collect all the xmlns and schema locations
            List<string> xmlnsList = new List<string>();
            List<string> schemaLocList = new List<string>();
            foreach (METS_Section_ReaderWriter_Config thisRWconfig in profile.Package_Level_AmdSec_Writer_Configs)
            {
                iPackage_amdSec_ReaderWriter thisRw = (iPackage_amdSec_ReaderWriter) thisRWconfig.ReaderWriterObject;
                if (thisRw.Schema_Reference_Required_Package(Item_To_Save))
                {
                    string[] rwXmlns = thisRw.Schema_Namespace(Item_To_Save);
                    string[] rwSchema = thisRw.Schema_Location(Item_To_Save);
                    foreach (string thisValue in rwXmlns)
                    {
                        if (!xmlnsList.Contains(thisValue))
                            xmlnsList.Add(thisValue);
                    }
                    foreach (string thisValue in rwSchema)
                    {
                        if (!schemaLocList.Contains(thisValue))
                            schemaLocList.Add(thisValue);
                    }
                }
            }
            foreach (METS_Section_ReaderWriter_Config thisRWconfig in profile.Package_Level_DmdSec_Writer_Configs)
            {
                iPackage_dmdSec_ReaderWriter thisRw = (iPackage_dmdSec_ReaderWriter) thisRWconfig.ReaderWriterObject;
                if (thisRw.Schema_Reference_Required_Package(Item_To_Save))
                {
                    string[] rwXmlns = thisRw.Schema_Namespace(Item_To_Save);
                    string[] rwSchema = thisRw.Schema_Location(Item_To_Save);
                    foreach (string thisValue in rwXmlns)
                    {
                        if (!xmlnsList.Contains(thisValue))
                            xmlnsList.Add(thisValue);
                    }
                    foreach (string thisValue in rwSchema)
                    {
                        if (!schemaLocList.Contains(thisValue))
                            schemaLocList.Add(thisValue);
                    }
                }
            }
            foreach (METS_Section_ReaderWriter_Config thisRWconfig in profile.Division_Level_AmdSec_Writer_Configs)
            {
                iDivision_amdSec_ReaderWriter thisRw = (iDivision_amdSec_ReaderWriter)thisRWconfig.ReaderWriterObject;
                foreach (abstract_TreeNode thisNode in physicalDivisions)
                {
                    if (thisRw.Schema_Reference_Required_Division(thisNode))
                    {
                        string[] rwXmlns = thisRw.Schema_Namespace(Item_To_Save);
                        string[] rwSchema = thisRw.Schema_Location(Item_To_Save);
                        foreach (string thisValue in rwXmlns)
                        {
                            if (!xmlnsList.Contains(thisValue))
                                xmlnsList.Add(thisValue);
                        }
                        foreach (string thisValue in rwSchema)
                        {
                            if (!schemaLocList.Contains(thisValue))
                                schemaLocList.Add(thisValue);
                        }

                        break;
                    }
                }
            }
            foreach (METS_Section_ReaderWriter_Config thisRWconfig in profile.Division_Level_DmdSec_Writer_Configs)
            {
                iDivision_dmdSec_ReaderWriter thisRw = (iDivision_dmdSec_ReaderWriter)thisRWconfig.ReaderWriterObject;
                foreach (abstract_TreeNode thisNode in physicalDivisions)
                {
                    if (thisRw.Schema_Reference_Required_Division(thisNode))
                    {
                        string[] rwXmlns = thisRw.Schema_Namespace(Item_To_Save);
                        string[] rwSchema = thisRw.Schema_Location(Item_To_Save);
                        foreach (string thisValue in rwXmlns)
                        {
                            if (!xmlnsList.Contains(thisValue))
                                xmlnsList.Add(thisValue);
                        }
                        foreach (string thisValue in rwSchema)
                        {
                            if (!schemaLocList.Contains(thisValue))
                                schemaLocList.Add(thisValue);
                        }

                        break;
                    }
                }
            }

            // Add the namespaces
            foreach (string namespaces in xmlnsList)
            {
                Output_Stream.WriteLine("  xmlns:" + namespaces);
            }

            Output_Stream.WriteLine("  xsi:schemaLocation=\"http://www.loc.gov/METS/");
            Output_Stream.Write("    http://www.loc.gov/standards/mets/mets.xsd");

            // Add the schema locations
            foreach (string location in schemaLocList)
            {
                Output_Stream.WriteLine();
                Output_Stream.Write(location);
            }

            #endregion

            Output_Stream.WriteLine("\">");

            // Add the METS declaration information and METS header
            Item_To_Save.METS_Header.Add_METS(Item_To_Save, Output_Stream);

            // Create the options dictionary for these writers
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["SobekCM_FileInfo_METS_amdSec_ReaderWriter:All_Files"] = allFiles;

            // Counters to keep track of the number of each section added
            int dmdSec_counter = 1;
            int digiProvMd = 1;
            int rightsMd = 1;
            int sourceMd = 1;
            int techMd = 1;

            #region Add all the package-level DMDSECs 

            // Prepare to add all the bibliographic section
            StringBuilder dmd_secid_builder = new StringBuilder();

            // Step through all the package level dmdSecs to be added
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.Package_Level_DmdSec_Writer_Configs)
            {
                iPackage_dmdSec_ReaderWriter thisWriter = (iPackage_dmdSec_ReaderWriter) thisConfig.ReaderWriterObject;
                if (thisWriter.Include_dmdSec(Item_To_Save, options))
                {
                    // Save this DMD ID
                    dmd_secid_builder.Append("DMD" + dmdSec_counter + " ");

                    // Start this METS section
                    Output_Stream.WriteLine("<METS:dmdSec ID=\"DMD" + dmdSec_counter + "\">");
                    if (thisConfig.Default_Mapping.Other_MD_Type.Length > 0)
                        Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  OTHERMDTYPE=\"" + thisConfig.Default_Mapping.Other_MD_Type + "\" MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");
                    else
                        Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");

                    Output_Stream.WriteLine("<METS:xmlData>");

                    // Add the amd section to the stream
                    thisWriter.Write_dmdSec(Output_Stream, Item_To_Save, options);

                    // Close this METS section
                    Output_Stream.WriteLine("</METS:xmlData>");
                    Output_Stream.WriteLine("</METS:mdWrap>");
                    Output_Stream.WriteLine("</METS:dmdSec>");

                    dmdSec_counter++;
                }
            }

            // Now, add any unanalyzed DMD sections
            if (( Item_To_Save.Unanalyzed_DMDSECs != null ) && ( Item_To_Save.Unanalyzed_DMDSECs.Count > 0 ))
            {
                foreach (Unanalyzed_METS_Section thisSection in Item_To_Save.Unanalyzed_DMDSECs)
                {
                    // This wll be linked to the top-level
                    dmd_secid_builder.Append("DMD" + dmdSec_counter + " ");

                    // Add this to the output stream
                    Output_Stream.Write("<METS:dmdSec ID=\"DMD" + dmdSec_counter + "\"");

                    foreach (KeyValuePair<string, string> attribute in thisSection.Section_Attributes)
                    {
                        if (attribute.Key != "ID")
                            Output_Stream.Write(" " + attribute.Key + "=\"" + attribute.Value + "\"");
                    }
                    Output_Stream.WriteLine(">");
                    Output_Stream.WriteLine(thisSection.Inner_XML);
                    Output_Stream.WriteLine("</METS:dmdSec>");

                    dmdSec_counter++;
                }
            }

            #endregion

            #region Add all the division-level DMDSECs

            // Should we add DMDSEC metadata sections for the divisions?
            // Step through all the possible division level dmdSecs to be added
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.Division_Level_DmdSec_Writer_Configs)
            {
                // Step through each division
                foreach (abstract_TreeNode thisDivision in allDivisions)
                {
                    iDivision_dmdSec_ReaderWriter thisWriter = (iDivision_dmdSec_ReaderWriter) thisConfig.ReaderWriterObject;

                    // Include the DMD Sec for this division?
                    if (thisWriter.Include_dmdSec(thisDivision, options))
                    {
                        // Save this DMD ID
                        thisDivision.DMDID = thisDivision.DMDID + "DMD" + dmdSec_counter + " ";

                        // Start this METS section
                        Output_Stream.WriteLine("<METS:dmdSec ID=\"DMD" + dmdSec_counter + "\">");
                        if (thisConfig.Default_Mapping.Other_MD_Type.Length > 0)
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  OTHERMDTYPE=\"" + thisConfig.Default_Mapping.Other_MD_Type + "\" MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");
                        else
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");

                        Output_Stream.WriteLine("<METS:xmlData>");

                        // Add the amd section to the stream
                        thisWriter.Write_dmdSec(Output_Stream, thisDivision, options);

                        // Close this METS section
                        Output_Stream.WriteLine("</METS:xmlData>");
                        Output_Stream.WriteLine("</METS:mdWrap>");
                        Output_Stream.WriteLine("</METS:dmdSec>");

                        dmdSec_counter++;
                    }
                }
            }


            #endregion

            #region Add all the file-level DMDSECs

            // Should we add DMDSEC metadata sections for the files?
            // Step through all the possible file level dmdSecs to be added
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.File_Level_DmdSec_Writer_Configs)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in allFiles)
                {
                    iFile_dmdSec_ReaderWriter thisWriter = (iFile_dmdSec_ReaderWriter)thisConfig.ReaderWriterObject;

                    // Include the DMD Sec for this file?
                    if (thisWriter.Include_dmdSec(thisFile, options))
                    {
                        // Save this DMD ID
                        thisFile.DMDID = thisFile.DMDID + "DMD" + dmdSec_counter + " ";

                        // Start this METS section
                        Output_Stream.WriteLine("<METS:dmdSec ID=\"DMD" + dmdSec_counter + "\">");
                        if (thisConfig.Default_Mapping.Other_MD_Type.Length > 0)
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  OTHERMDTYPE=\"" + thisConfig.Default_Mapping.Other_MD_Type + "\" MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");
                        else
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");

                        Output_Stream.WriteLine("<METS:xmlData>");

                        // Add the amd section to the stream
                        thisWriter.Write_dmdSec(Output_Stream, thisFile, options);

                        // Close this METS section
                        Output_Stream.WriteLine("</METS:xmlData>");
                        Output_Stream.WriteLine("</METS:mdWrap>");
                        Output_Stream.WriteLine("</METS:dmdSec>");

                        dmdSec_counter++;
                    }
                }
            }


            #endregion

            #region Add all the package-level AMDSECs

            // Prepare to add all the bibliographic section
            StringBuilder amd_secid_builder = new StringBuilder();

            // Step through all the package level amdSecs to be added
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.Package_Level_AmdSec_Writer_Configs)
            {
                iPackage_amdSec_ReaderWriter thisWriter = (iPackage_amdSec_ReaderWriter)thisConfig.ReaderWriterObject;
                if (thisWriter.Include_amdSec(Item_To_Save, options))
                {
                    // Start this METS section
                    Output_Stream.WriteLine("<METS:amdSec>");
                    switch (thisConfig.AmdSecType)
                    {
                        case METS_amdSec_Type_Enum.DigiProvMD:
                            Output_Stream.WriteLine("<METS:digiprovMD ID=\"DIGIPROV" + digiProvMd + "\">");
                            amd_secid_builder.Append("DIGIPROV" + digiProvMd + " ");
                            digiProvMd++;
                            break;

                        case METS_amdSec_Type_Enum.RightsMD:
                            Output_Stream.WriteLine("<METS:rightsMD ID=\"RIGHTS" + rightsMd + "\">");
                            amd_secid_builder.Append("RIGHTS" + rightsMd + " ");
                            rightsMd++;
                            break;

                        case METS_amdSec_Type_Enum.SourceMD:
                            Output_Stream.WriteLine("<METS:sourceMD ID=\"SOURCE" + sourceMd + "\">");
                            amd_secid_builder.Append("SOURCE" + sourceMd + " ");
                            sourceMd++;
                            break;

                        case METS_amdSec_Type_Enum.TechMD:
                            Output_Stream.WriteLine("<METS:techMD ID=\"TECH" + techMd + "\">");
                            amd_secid_builder.Append("TECH" + techMd + " ");
                            techMd++;
                            break;

                        default:
                            Output_Stream.WriteLine("<METS:rightsMD ID=\"RIGHTS" + rightsMd + "\">");
                            amd_secid_builder.Append("RIGHTS" + rightsMd + " ");
                            rightsMd++;
                            break;
                    }


                    if (thisConfig.Default_Mapping.Other_MD_Type.Length > 0)
                        Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  OTHERMDTYPE=\"" + thisConfig.Default_Mapping.Other_MD_Type + "\" MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");
                    else
                        Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");

                    Output_Stream.WriteLine("<METS:xmlData>");

                    // Add the amd section to the stream
                    thisWriter.Write_amdSec(Output_Stream, Item_To_Save, options);

                    // Close this METS section
                    Output_Stream.WriteLine("</METS:xmlData>");
                    Output_Stream.WriteLine("</METS:mdWrap>");

                    switch (thisConfig.AmdSecType)
                    {
                        case METS_amdSec_Type_Enum.DigiProvMD:
                            Output_Stream.WriteLine("</METS:digiprovMD>");
                            break;

                        case METS_amdSec_Type_Enum.RightsMD:
                            Output_Stream.WriteLine("</METS:rightsMD>");
                            break;

                        case METS_amdSec_Type_Enum.SourceMD:
                            Output_Stream.WriteLine("</METS:sourceMD>");
                            break;

                        case METS_amdSec_Type_Enum.TechMD:
                            Output_Stream.WriteLine("</METS:techMD>");
                            break;

                        default:
                            Output_Stream.WriteLine("</METS:rightsMD>");
                            break;
                    }

                    Output_Stream.WriteLine("</METS:amdSec>");
                }
            }

            // Now, add any unanalyzed AMD sections
            if (Item_To_Save.Unanalyzed_AMDSECs != null)
            {
                foreach (Unanalyzed_METS_Section thisSection in Item_To_Save.Unanalyzed_AMDSECs)
                {
                    // Add this to the output stream
                    Output_Stream.Write("<METS:amdSec");
                    foreach (KeyValuePair<string, string> attribute in thisSection.Section_Attributes)
                    {
                        if (attribute.Key != "ID")
                            Output_Stream.Write(" " + attribute.Key + "=\"" + attribute.Value + "\"");
                    }
                    Output_Stream.WriteLine(">");
                    Output_Stream.WriteLine(thisSection.Inner_XML);
                    Output_Stream.WriteLine("</METS:amdSec>");
                }
            }

            #endregion

            #region Add all the division-level AMDSECs

            // Should we add AMDSEC metadata sections for the divisions?
            // Step through all the possible division level amdSecs to be added
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.Division_Level_AmdSec_Writer_Configs)
            {
                // Step through each division
                foreach (abstract_TreeNode thisDivision in allDivisions)
                {

                    iDivision_amdSec_ReaderWriter thisWriter = (iDivision_amdSec_ReaderWriter) thisConfig.ReaderWriterObject;

                    // Include the DMD Sec for this division?
                    if (thisWriter.Include_amdSec(thisDivision, options))
                    {
                        // Start this METS section
                        Output_Stream.WriteLine("<METS:amdSec>");
                        switch (thisConfig.AmdSecType)
                        {
                            case METS_amdSec_Type_Enum.DigiProvMD:
                                Output_Stream.WriteLine("<METS:digiProvMD ID=\"DIGIPROV" + digiProvMd + "\">");
                                thisDivision.ADMID = thisDivision.ADMID + "DIGIPROV" + digiProvMd + " ";
                                digiProvMd++;
                                break;

                            case METS_amdSec_Type_Enum.RightsMD:
                                Output_Stream.WriteLine("<METS:rightsMD ID=\"RIGHTS" + rightsMd + "\">");
                                thisDivision.ADMID = thisDivision.ADMID + "RIGHTS" + rightsMd + " ";
                                rightsMd++;
                                break;

                            case METS_amdSec_Type_Enum.SourceMD:
                                Output_Stream.WriteLine("<METS:sourceMD ID=\"SOURCE" + sourceMd + "\">");
                                thisDivision.ADMID = thisDivision.ADMID + "SOURCE" + sourceMd + " ";
                                sourceMd++;
                                break;

                            case METS_amdSec_Type_Enum.TechMD:
                                Output_Stream.WriteLine("<METS:techMD ID=\"TECH" + techMd + "\">");
                                thisDivision.ADMID = thisDivision.ADMID + "TECH" + techMd + " ";
                                techMd++;
                                break;

                            default:
                                Output_Stream.WriteLine("<METS:rightsMD ID=\"RIGHTS" + rightsMd + "\">");
                                thisDivision.ADMID = thisDivision.ADMID + "RIGHTS" + rightsMd + " ";
                                rightsMd++;
                                break;
                        }


                        if (thisConfig.Default_Mapping.Other_MD_Type.Length > 0)
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  OTHERMDTYPE=\"" + thisConfig.Default_Mapping.Other_MD_Type + "\" MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");
                        else
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");

                        Output_Stream.WriteLine("<METS:xmlData>");
                        Output_Stream.WriteLine("<METS:xmlData>");

                        // Add the amd section to the stream
                        thisWriter.Write_amdSec(Output_Stream, thisDivision, options);

                        // Close this METS section
                        Output_Stream.WriteLine("</METS:xmlData>");
                        Output_Stream.WriteLine("</METS:mdWrap>");

                        switch (thisConfig.AmdSecType)
                        {
                            case METS_amdSec_Type_Enum.DigiProvMD:
                                Output_Stream.WriteLine("</METS:digiProvMD>");
                                break;

                            case METS_amdSec_Type_Enum.RightsMD:
                                Output_Stream.WriteLine("</METS:rightsMD>");
                                break;

                            case METS_amdSec_Type_Enum.SourceMD:
                                Output_Stream.WriteLine("</METS:sourceMD>");
                                break;

                            case METS_amdSec_Type_Enum.TechMD:
                                Output_Stream.WriteLine("</METS:techMD>");
                                break;

                            default:
                                Output_Stream.WriteLine("</METS:rightsMD>");
                                break;
                        }

                        Output_Stream.WriteLine("</METS:amdSec>");

                        dmdSec_counter++;
                    }
                }
            }



            #endregion

            #region Add all the file-level AMDSECs

            // Should we add AMDSEC metadata sections for the files?
            // Step through all the possible file level amdSecs to be added
            foreach (METS_Section_ReaderWriter_Config thisConfig in profile.File_Level_AmdSec_Writer_Configs)
            {
                // Step through each file
                foreach (SobekCM_File_Info thisFile in allFiles)
                {

                    iFile_amdSec_ReaderWriter thisWriter = (iFile_amdSec_ReaderWriter)thisConfig.ReaderWriterObject;

                    // Include the DMD Sec for this file?
                    if (thisWriter.Include_amdSec(thisFile, options))
                    {
                        // Start this METS section
                        Output_Stream.WriteLine("<METS:amdSec>");
                        switch (thisConfig.AmdSecType)
                        {
                            case METS_amdSec_Type_Enum.DigiProvMD:
                                Output_Stream.WriteLine("<METS:digiProvMD ID=\"DIGIPROV" + digiProvMd + "\">");
                                thisFile.ADMID = thisFile.ADMID + "DIGIPROV" + digiProvMd + " ";
                                digiProvMd++;
                                break;

                            case METS_amdSec_Type_Enum.RightsMD:
                                Output_Stream.WriteLine("<METS:rightsMD ID=\"RIGHTS" + rightsMd + "\">");
                                thisFile.ADMID = thisFile.ADMID + "RIGHTS" + rightsMd + " ";
                                rightsMd++;
                                break;

                            case METS_amdSec_Type_Enum.SourceMD:
                                Output_Stream.WriteLine("<METS:sourceMD ID=\"SOURCE" + sourceMd + "\">");
                                thisFile.ADMID = thisFile.ADMID + "SOURCE" + sourceMd + " ";
                                sourceMd++;
                                break;

                            case METS_amdSec_Type_Enum.TechMD:
                                Output_Stream.WriteLine("<METS:techMD ID=\"TECH" + techMd + "\">");
                                thisFile.ADMID = thisFile.ADMID + "TECH" + techMd + " ";
                                techMd++;
                                break;

                            default:
                                Output_Stream.WriteLine("<METS:rightsMD ID=\"RIGHTS" + rightsMd + "\">");
                                thisFile.ADMID = thisFile.ADMID + "RIGHTS" + rightsMd + " ";
                                rightsMd++;
                                break;
                        }


                        if (thisConfig.Default_Mapping.Other_MD_Type.Length > 0)
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  OTHERMDTYPE=\"" + thisConfig.Default_Mapping.Other_MD_Type + "\" MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");
                        else
                            Output_Stream.WriteLine("<METS:mdWrap MDTYPE=\"" + thisConfig.Default_Mapping.MD_Type + "\"  MIMETYPE=\"text/xml\" LABEL=\"" + thisConfig.Default_Mapping.Label + "\">");

                        Output_Stream.WriteLine("<METS:xmlData>");
                        Output_Stream.WriteLine("<METS:xmlData>");

                        // Add the amd section to the stream
                        thisWriter.Write_amdSec(Output_Stream, thisFile, options);

                        // Close this METS section
                        Output_Stream.WriteLine("</METS:xmlData>");
                        Output_Stream.WriteLine("</METS:mdWrap>");

                        switch (thisConfig.AmdSecType)
                        {
                            case METS_amdSec_Type_Enum.DigiProvMD:
                                Output_Stream.WriteLine("</METS:digiProvMD>");
                                break;

                            case METS_amdSec_Type_Enum.RightsMD:
                                Output_Stream.WriteLine("</METS:rightsMD>");
                                break;

                            case METS_amdSec_Type_Enum.SourceMD:
                                Output_Stream.WriteLine("</METS:sourceMD>");
                                break;

                            case METS_amdSec_Type_Enum.TechMD:
                                Output_Stream.WriteLine("</METS:techMD>");
                                break;

                            default:
                                Output_Stream.WriteLine("</METS:rightsMD>");
                                break;
                        }

                        Output_Stream.WriteLine("</METS:amdSec>");

                        dmdSec_counter++;
                    }
                }
            }



            #endregion

            // Add file and structure map sections
            if ( allFiles.Count > 0 )
            {
                #region Add the files section

                // Start the files section
                Output_Stream.WriteLine("<METS:fileSec>");

                // Step through each mime type
                foreach (string thisMimeType in mimeHash.Keys)
                {
                    List<SobekCM_File_Info> mimeCollection = mimeHash[thisMimeType];

                    // Start this file group section
                    Output_Stream.WriteLine(thisMimeType == "image/tiff" ? "<METS:fileGrp USE=\"archive\" >" : "<METS:fileGrp USE=\"reference\" >");

                    // Step through each file of this mime type and append the METS
                    foreach (SobekCM_File_Info thisFile in mimeCollection)
                    {
                        // Is the size and checksum information here?
                        if ((!MINIMIZE_FILE_SIZE) && ( !Item_To_Save.Divisions.Suppress_Checksum )) 
                        {
                            if ((((String.IsNullOrEmpty(thisFile.Checksum)) || (thisFile.Size <= 0))) && (thisFile.METS_LocType == SobekCM_File_Info_Type_Enum.SYSTEM))
                            {
                                // Perform in a try catch
                                try
                                {
                                    if (File.Exists(Item_To_Save.Source_Directory + "/" + thisFile.System_Name))
                                    {
                                        // Get the size first
                                        if ( thisFile.Size < 0 )
                                        {
                                            FileInfo thisFileInfo = new FileInfo(Item_To_Save.Source_Directory + "/" + thisFile.System_Name);
                                            thisFile.Size = thisFileInfo.Length;
                                        }

                                        // Get the checksum, if it doesn't exist
                                        if (String.IsNullOrEmpty(thisFile.Checksum))
                                        {
                                            FileMD5 checksummer = new FileMD5(Item_To_Save.Source_Directory + "/" + thisFile.System_Name);
                                            thisFile.Checksum = checksummer.Checksum;

                                            // Set the checksum type
                                            thisFile.Checksum_Type = "MD5";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }

                        // Start out this file 
                        Output_Stream.Write("<METS:file GROUPID=\"" + thisFile.Group_Number + "\" ID=\"" + thisFile.ID + "\" MIMETYPE=\"" + thisMimeType + "\"");

                        // Add links to dmd secs and amd secs
                        if (!String.IsNullOrEmpty(thisFile.DMDID))
                        {
                            Output_Stream.Write(" DMDID=\"" + thisFile.DMDID + "\"");
                        }
                        if (!String.IsNullOrEmpty(thisFile.ADMID))
                        {
                            Output_Stream.Write(" ADMID=\"" + thisFile.ADMID + "\"");
                        }

                        // Add the checksum it it exists
                        if ((!MINIMIZE_FILE_SIZE) && (!String.IsNullOrEmpty(thisFile.Checksum)))
                        {
                            Output_Stream.Write(" CHECKSUM=\"" + thisFile.Checksum + "\"");

                            // Add the checksum type if there was one (SHOULD be one here)
                            if (!String.IsNullOrEmpty(thisFile.Checksum_Type))
                            {
                                Output_Stream.Write(" CHECKSUMTYPE=\"" + thisFile.Checksum_Type + "\"");
                            }
                        }

                        // Add the size of the file, if it exists
                        if ((!MINIMIZE_FILE_SIZE) && (thisFile.Size > 0))
                        {
                            Output_Stream.Write(" SIZE=\"" + thisFile.Size + "\"");
                        }

                        // Close out this beginning file tag
                        Output_Stream.WriteLine(">");

                        // Include the system name or URL
                        if (thisFile.METS_LocType == SobekCM_File_Info_Type_Enum.URL)
                        {
                            Output_Stream.WriteLine("<METS:FLocat LOCTYPE=\"URL\" xlink:href=\"" + thisFile.System_Name.Replace(" ", "%20").Replace("&", "&amp;") + "\" />");
                        }
                        else
                        {
                            Output_Stream.WriteLine("<METS:FLocat LOCTYPE=\"OTHER\" OTHERLOCTYPE=\"SYSTEM\" xlink:href=\"" + thisFile.System_Name.Replace(" ", "%20").Replace("&", "&amp;").Replace("\\", "/") + "\" />");
                        }

                        // Add the closing file tag
                        Output_Stream.WriteLine("</METS:file>");
                    }

                    // Close out this file group section
                    Output_Stream.WriteLine("</METS:fileGrp>");
                }

                // Finish out the file section
                Output_Stream.WriteLine("</METS:fileSec>");

                #endregion

                #region Add the structure map section

                Dictionary<abstract_TreeNode, int> pages_to_appearances = new Dictionary<abstract_TreeNode, int>();

                // May or may not be AMDSecs and DMDsec
                string dmdSecIdString = dmd_secid_builder.ToString().Trim();
                string amdSecIdString = amd_secid_builder.ToString().Trim();

                // Add the physical page structure map first
                if (hasPageFiles)
                {
                    Output_Stream.WriteLine("<METS:structMap ID=\"STRUCT1\" TYPE=\"physical\">");

                    // Add any outer divisions here
                    if ( Item_To_Save.Divisions.Outer_Division_Count > 0)
                    {
                        foreach (Outer_Division_Info outerDiv in Item_To_Save.Divisions.Outer_Divisions)
                        {
                            Output_Stream.Write("<METS:div");
                            if (dmdSecIdString.Length > 0)
                            {
                                Output_Stream.Write(" DMDID=\"" + dmdSecIdString + "\"");
                            }
                            if (outerDiv.Label.Length > 0)
                            {
                                Output_Stream.Write(" LABEL=\"" + Convert_String_To_XML_Safe_Static(outerDiv.Label) + "\"");
                            }
                            if (outerDiv.OrderLabel > 0)
                            {
                                Output_Stream.Write(" ORDERLABEL=\"" + outerDiv.OrderLabel + "\"");
                            }
                            if (outerDiv.Type.Length > 0)
                            {
                                Output_Stream.Write(" TYPE=\"" + Convert_String_To_XML_Safe_Static(outerDiv.Type) + "\"");
                            }
                            Output_Stream.WriteLine(">");
                        }
                    }
                    else
                    {
                        // Start the main division information
                        Output_Stream.Write("<METS:div");
                        if (dmdSecIdString.Length > 0)
                        {
                            Output_Stream.Write(" DMDID=\"" + dmdSecIdString + "\"");
                        }
                        if (amdSecIdString.Length > 0)
                        {
                            Output_Stream.Write(" ADMID=\"" + amdSecIdString + "\"");
                        }


                        // Add the title, if one was provided
                        string title = Item_To_Save.Bib_Info.Main_Title.ToString();
                        if (title.Length > 0)
                        {
                            Output_Stream.Write(" LABEL=\"" + title + "\"");
                        }

                        // Finish out this first, main division tag
                        Output_Stream.WriteLine(" ORDER=\"0\" TYPE=\"main\">");
                    }

                    // Add all the divisions recursively
                    int order = 1;
                    foreach (abstract_TreeNode thisRoot in Item_To_Save.Divisions.Physical_Tree.Roots)
                    {
                        recursively_add_div_info(thisRoot, Output_Stream, pages_to_appearances, order++);
                    }

                    // Close any outer divisions here
                    if (Item_To_Save.Divisions.Outer_Division_Count > 0)
                    {
                        for (int index = 0; index < Item_To_Save.Divisions.Outer_Division_Count; index++)
                        {
                            Output_Stream.WriteLine("</METS:div>");
                        }
                    }
                    else
                    {
                        // Close out the main division tag
                        Output_Stream.WriteLine("</METS:div>");
                    }

                    // Close out this structure map portion
                    Output_Stream.WriteLine("</METS:structMap>");
                }

                if (hasDownloadFiles)
                {
                    Output_Stream.WriteLine("<METS:structMap ID=\"STRUCT2\" TYPE=\"other\">");

                   // Add any outer divisions here
                    if ( Item_To_Save.Divisions.Outer_Division_Count > 0)
                    {
                        foreach (Outer_Division_Info outerDiv in Item_To_Save.Divisions.Outer_Divisions)
                        {
                            Output_Stream.Write("<METS:div");
                            if (dmdSecIdString.Length > 0)
                            {
                                Output_Stream.Write(" DMDID=\"" + dmdSecIdString + "\"");
                            }
                            if (outerDiv.Label.Length > 0)
                            {
                                Output_Stream.Write(" LABEL=\"" + Convert_String_To_XML_Safe_Static(outerDiv.Label) + "\"");
                            }
                            if (outerDiv.OrderLabel > 0)
                            {
                                Output_Stream.Write(" ORDERLABEL=\"" + outerDiv.OrderLabel + "\"");
                            }
                            if (outerDiv.Type.Length > 0)
                            {
                                Output_Stream.Write(" TYPE=\"" + Convert_String_To_XML_Safe_Static(outerDiv.Type) + "\"");
                            }
                            Output_Stream.WriteLine(">");
                        }
                    }
                    else
                    {
                        // Start the main division information
                        Output_Stream.Write("<METS:div");
                        if (dmdSecIdString.Length > 0)
                        {
                            Output_Stream.Write(" DMDID=\"" + dmdSecIdString + "\"");
                        }
                        if (amdSecIdString.Length > 0)
                        {
                            Output_Stream.Write(" ADMID=\"" + amdSecIdString + "\"");
                        }

                        // Add the title, if one was provided
                        string title = Item_To_Save.Bib_Info.Main_Title.ToString();
                        if (title.Length > 0)
                        {
                            Output_Stream.Write(" LABEL=\"" + title + "\"");
                        }

                        // Finish out this first, main division tag
                        Output_Stream.WriteLine(" ORDER=\"0\" TYPE=\"main\">");
                    }

                    // Add all the divisions recursively
                    int order = 1;
                    foreach (abstract_TreeNode thisRoot in Item_To_Save.Divisions.Download_Tree.Roots)
                    {
                        recursively_add_div_info(thisRoot, Output_Stream, pages_to_appearances, order++);
                    }

                    // Close any outer divisions here
                    if (Item_To_Save.Divisions.Outer_Division_Count > 0)
                    {
                        for (int index = 0; index < Item_To_Save.Divisions.Outer_Division_Count; index++)
                        {
                            Output_Stream.WriteLine("</METS:div>");
                        }
                    }
                    else
                    {
                        // Close out the main division tag
                        Output_Stream.WriteLine("</METS:div>");
                    }

                    // Close out this structure map portion
                    Output_Stream.WriteLine("</METS:structMap>");
                }

                #endregion
            }
            else
            {
                // Structure map is a required element for METS
                Output_Stream.Write("<METS:structMap ID=\"STRUCT1\" > <METS:div /> </METS:structMap>\r\n");
            }

            // Add the behavior section for SobekCM views and interfaces
            //if (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_Behaviors))
            //{
            //    Item_To_Save.Behaviors.Add_BehaviorSec_METS(Output, Item_To_Save.Divisions.Physical_Tree.Has_Files);
            //}

            // Close out the METS file
            Output_Stream.Write("</METS:mets>\r\n");

            return true;
        }

        #endregion

        #region Code to create the METS structure map

 
        private void recursively_add_div_info(abstract_TreeNode ThisNode, TextWriter Results, Dictionary<abstract_TreeNode, int> PagesToAppearances, int Order)
        {
            // Add the div information for this node first
            if (ThisNode.Page)
            {
                // If the ID of this page is SKIP, then just return and do nothing here
                if (ThisNode.ID == "SKIP")
                    return;

                if (PagesToAppearances.ContainsKey(ThisNode))
                {
                    PagesToAppearances[ThisNode] = PagesToAppearances[ThisNode] + 1;
                    Results.Write("<METS:div ID=\"" + ThisNode.ID + "_repeat" + PagesToAppearances[ThisNode] + "\"");
                }
                else
                {
                    PagesToAppearances[ThisNode] = 1;
                    Results.Write("<METS:div ID=\"" + ThisNode.ID + "\"");
                }
            }
            else
            {
                Results.Write("<METS:div ID=\"" + ThisNode.ID + "\"");
            }

            // Add links to dmd secs and amd secs
            if ( !String.IsNullOrEmpty(ThisNode.DMDID))
            {
                Results.Write(" DMDID=\"" + ThisNode.DMDID + "\"");
            }
            if (!String.IsNullOrEmpty(ThisNode.ADMID))
            {
                Results.Write(" ADMID=\"" + ThisNode.ADMID + "\"");
            }

            // Add the label, if there is one
            if ((ThisNode.Label.Length > 0) && (ThisNode.Label != ThisNode.Type))
                Results.Write(" LABEL=\"" + Convert_String_To_XML_Safe(ThisNode.Label) + "\"");

            // Finish the start div label for this division
            Results.WriteLine(" ORDER=\"" + Order + "\" TYPE=\"" + ThisNode.Type + "\">");

            // If this is a page, add all the files, otherwise call this method recursively
            if (ThisNode.Page)
            {
                // Add each file
                Page_TreeNode thisPage = (Page_TreeNode)ThisNode;
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    // Add the file pointer informatino
                    if ( thisFile.ID.Length > 0 )
                        Results.WriteLine("<METS:fptr FILEID=\"" + thisFile.ID + "\" />");
                }
            }
            else
            {
                // Call this method for each subdivision
                int inner_order = 1;
                Division_TreeNode thisDivision = (Division_TreeNode)ThisNode;
                foreach (abstract_TreeNode thisSubDivision in thisDivision.Nodes)
                {
                    recursively_add_div_info(thisSubDivision, Results, PagesToAppearances, inner_order++ );
                }
            }

            // Close out this division
            Results.WriteLine("</METS:div>");
        }

        #endregion

        #region Method to read the metadata from a METS file or stream

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set return value
            bool returnValue;

            // Check that the file exists
            if (!File.Exists(MetadataFilePathName))
            {
                Error_Message = "File does not exist";
                return false;
            }

            // Create a stream and XML reader and read the metadata
            using (Stream reader = new FileStream(MetadataFilePathName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    returnValue = Read_Metadata(reader, Return_Package, Options, out Error_Message);

                    if (Return_Package != null)
                        Return_Package.Source_Directory = (new FileInfo(MetadataFilePathName)).Directory.ToString();
                }
                catch (Exception ee)
                {
                    Error_Message = "Error while reading METS file '" + MetadataFilePathName + "': " + ee.Message;
                    returnValue = false;
                }
                reader.Close();
            }


            return returnValue;
        }

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks> Accepts two options: (1) 'METS_File_ReaderWriter:Minimize_File_Info' which tells whether the reader 
        /// should just skip the file reading portion completely, and just read the bibliographic data ( Default is FALSE).
        /// (2) 'METS_File_ReaderWriter:Support_Divisional_dmdSec_amdSec' </remarks>
        public bool Read_Metadata(Stream Input_Stream, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            Error_Message = String.Empty;

            // Read the options from the dictionary of options
            bool minimizeFileInfo = false;
            if (Options != null)
            {
                if (Options.ContainsKey("METS_File_ReaderWriter:Minimize_File_Info"))
                    bool.TryParse(Options["METS_File_ReaderWriter:Minimize_File_Info"].ToString(), out minimizeFileInfo);

                if (Options.ContainsKey("METS_File_ReaderWriter:Support_Divisional_dmdSec_amdSec"))
                {
                    bool supportDivisionalDmdSecAmdSec;
                    bool.TryParse(Options["METS_File_ReaderWriter:Support_Divisional_dmdSec_amdSec"].ToString(), out supportDivisionalDmdSecAmdSec);
                }
            }

            // Keep a list of all the files created, by file id, as additional data is gathered
            // from the different locations ( amdSec, fileSec, structmap )
            Dictionary<string, SobekCM_File_Info> files_by_fileid = new Dictionary<string, SobekCM_File_Info>();

            // For now, to do support for old way of doing downloads, build a list to hold
            // the deprecated download files
            List<Download_Info_DEPRECATED> deprecatedDownloads = new List<Download_Info_DEPRECATED>();

            // Need to store the unanalyzed sections of dmdSec and amdSec until we determine if 
            // the scope is the whole package, or the top-level div.  We use lists as the value since
            // several sections may have NO id and the METS may even (incorrectly) have multiple sections
            // with the same ID
            Dictionary<string, List<Unanalyzed_METS_Section>> dmdSec = new Dictionary<string, List<Unanalyzed_METS_Section>>();
            Dictionary<string, List<Unanalyzed_METS_Section>> amdSec = new Dictionary<string, List<Unanalyzed_METS_Section>>();

            // Dictionaries store the link between dmdSec and amdSec id's to single divisions
            Dictionary<string, abstract_TreeNode> division_dmdids = new Dictionary<string, abstract_TreeNode>();
            Dictionary<string, abstract_TreeNode> division_amdids = new Dictionary<string, abstract_TreeNode>();


            try
            {
                // Try to read the XML
                XmlReader r = new XmlTextReader(Input_Stream);

                // Begin stepping through each of the XML nodes
                while (r.Read())
                {
                    #region Handle some processing instructions requested by Florida SUS's / FLVC (hope to deprecate)

                    // Handle some processing instructions requested by Florida SUS's / FLVC
                    if (r.NodeType == XmlNodeType.ProcessingInstruction)
                    {
                        if (r.Name.ToLower() == "fcla")
                        {
                            string value = r.Value.ToLower();
                            if (value.IndexOf("fda=\"yes\"") >= 0)
                            {
                                DAITSS_Info daitssInfo = Return_Package.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
                                if (daitssInfo == null)
                                {
                                    daitssInfo = new DAITSS_Info();
                                    Return_Package.Add_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY, daitssInfo);
                                }
                                daitssInfo.toArchive = true;
                            }
                            if (value.IndexOf("fda=\"no\"") >= 0)
                            {
                                DAITSS_Info daitssInfo2 = Return_Package.Get_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY) as DAITSS_Info;
                                if (daitssInfo2 == null)
                                {
                                    daitssInfo2 = new DAITSS_Info();
                                    Return_Package.Add_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY, daitssInfo2);
                                }
                                daitssInfo2.toArchive = false;
                            }
                        }
                    }

                    #endregion

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name.Replace("METS:", ""))
                        {
                            case "mets":
                                if (r.MoveToAttribute("OBJID"))
                                    Return_Package.METS_Header.ObjectID = r.Value;
                                break;

                            case "metsHdr":
                                read_mets_header(r.ReadSubtree(), Return_Package);
                                break;

                            case "dmdSec":
                            case "dmdSecFedora":
                                Unanalyzed_METS_Section thisDmdSec = store_dmd_sec(r.ReadSubtree());
                                if ( dmdSec.ContainsKey(thisDmdSec.ID))
                                    dmdSec[thisDmdSec.ID].Add(thisDmdSec);
                                else
                                {
                                    List<Unanalyzed_METS_Section> newDmdSecList = new List<Unanalyzed_METS_Section>();
                                    newDmdSecList.Add(thisDmdSec);
                                    dmdSec[thisDmdSec.ID] = newDmdSecList;
                                }
                                
                                break;

                            case "amdSec":
                                Unanalyzed_METS_Section thisAmdSec = store_amd_sec(r.ReadSubtree());
                                if (amdSec.ContainsKey(thisAmdSec.ID))
                                    amdSec[thisAmdSec.ID].Add(thisAmdSec);
                                else
                                {
                                    List<Unanalyzed_METS_Section> newAmdSecList = new List<Unanalyzed_METS_Section> {thisAmdSec};
                                    amdSec[thisAmdSec.ID] = newAmdSecList;
                                }
                                break;

                            case "fileSec":
                                read_file_sec(r.ReadSubtree(), minimizeFileInfo, files_by_fileid);
                                break;

                            case "structMap":
                                if (!r.IsEmptyElement)
                                {
                                    read_struct_map(r.ReadSubtree(), Return_Package, files_by_fileid, division_dmdids, division_amdids);
                                }
                                break;

                            case "behaviorSec":
                                read_behavior_sec(r.ReadSubtree(), Return_Package);
                                break;
                        }
                    }
                }

                // writer.Close();
                r.Close();

            }
            catch 
            {
                // Do nothinh
            }

            Input_Stream.Close();

            // Load some options for interoperability
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("SobekCM_FileInfo_METS_amdSec_ReaderWriter:Files_By_FileID", files_by_fileid);

            #region Process the previously stored dmd sections

            // Now, process the previously stored dmd sections
            foreach (string thisDmdSecId in dmdSec.Keys)
            {
                // Could be multiple stored sections with the same (or no) ID
                foreach (Unanalyzed_METS_Section metsSection in dmdSec[thisDmdSecId])
                {
                    XmlReader reader = XmlReader.Create(new StringReader(metsSection.Inner_XML));
                    string mdtype = String.Empty;
                    string othermdtype = String.Empty;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name.ToLower().Replace("mets:", "") == "mdwrap")
                            {

                                if (reader.MoveToAttribute("MDTYPE"))
                                    mdtype = reader.Value;
                                if (reader.MoveToAttribute("OTHERMDTYPE"))
                                    othermdtype = reader.Value;

                                // NOt crazy about this part, but sometimes people do not use the OTHERMDTYPE
                                // tag correctly, and just use the LABEL to differentiate the types
                                if ((mdtype == "OTHER") && (othermdtype.Length == 0) && (reader.MoveToAttribute("LABEL")))
                                    othermdtype = reader.Value;

                                // Now, determine if this was a division-level read, or a package-wide
                                if (division_dmdids.ContainsKey(thisDmdSecId))
                                {
                                    // Division level dmdSec
                                    // Get the division
                                    abstract_TreeNode node = division_dmdids[thisDmdSecId];

                                    // Get an appropriate reader from the metadata configuration
                                    iDivision_dmdSec_ReaderWriter rw = ResourceObjectSettings.MetadataConfig.Get_Division_DmdSec_ReaderWriter(mdtype, othermdtype);

                                    // Is this dmdSec analyzable? (i.e., did we find an appropriate reader/writer?)
                                    if (rw == null)
                                    {
                                        node.Add_Unanalyzed_DMDSEC(metsSection);
                                    }
                                    else
                                    {
                                        rw.Read_dmdSec(reader, node, options);
                                    }
                                }
                                else
                                {
                                    // Package-level dmdSec 
                                    // Get an appropriate reader from the metadata configuration
                                    iPackage_dmdSec_ReaderWriter rw = ResourceObjectSettings.MetadataConfig.Get_Package_DmdSec_ReaderWriter(mdtype, othermdtype);

                                    // Is this dmdSec analyzable? (i.e., did we find an appropriate reader/writer?)
                                    if (rw == null)
                                    {
                                        Return_Package.Add_Unanalyzed_DMDSEC(metsSection);
                                    }
                                    else
                                    {
                                        rw.Read_dmdSec(reader, Return_Package, options);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Process the previously stored amd sections

            // Now, process the previously stored amd sections
            foreach (string thisAmdSecId in amdSec.Keys)
            {
                // Could be multiple stored sections with the same (or no) ID
                foreach (Unanalyzed_METS_Section metsSection in amdSec[thisAmdSecId])
                {
                    XmlReader reader = XmlReader.Create(new StringReader(metsSection.Inner_XML));
                    string mdtype = String.Empty;
                    string othermdtype = String.Empty;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name.ToLower().Replace("mets:", "") == "mdwrap")
                            {

                                if (reader.MoveToAttribute("MDTYPE"))
                                    mdtype = reader.Value;
                                if (reader.MoveToAttribute("OTHERMDTYPE"))
                                    othermdtype = reader.Value;

                                // Package-level amdSec 
                                // Get an appropriate reader from the metadata configuration
                                iPackage_amdSec_ReaderWriter rw = ResourceObjectSettings.MetadataConfig.Get_Package_AmdSec_ReaderWriter(mdtype, othermdtype);

                                // Is this amdSec analyzable? (i.e., did we find an appropriate reader/writer?)
                                if (rw == null)
                                {
                                    Return_Package.Add_Unanalyzed_AMDSEC(metsSection);
                                }
                                else
                                {
                                    rw.Read_amdSec(reader, Return_Package, options);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Special code used for moving downloads into the structure map system, and out of the old SobekCM METS section 

            // For backward compatability, move from the old download system to the
            // new structure.  This has to happen here at the end so that we have access

            // Were there some downloads added here?
            if (deprecatedDownloads.Count > 0)
            {
                // Get the list of downloads from the download tree
                List<SobekCM_File_Info> newStructureDownloads = Return_Package.Divisions.Download_Tree.All_Files;

                // Step through each download in the old system
                foreach (Download_Info_DEPRECATED thisDownload in deprecatedDownloads)
                {
                    // Get the label (if there is one)
                    string label = thisDownload.Label;
                    string filename = thisDownload.FileName;
                    bool found = false;
                    if ((filename.Length == 0) && (thisDownload.File_ID.Length > 0))
                    {
                        if (files_by_fileid.ContainsKey(thisDownload.File_ID))
                        {
                            SobekCM_File_Info thisDownloadFile = files_by_fileid[thisDownload.File_ID];
                            filename = thisDownloadFile.System_Name;

                            // Ensure a file of this name doesn't already exist
                            foreach (SobekCM_File_Info existingFile in newStructureDownloads)
                            {
                                if (existingFile.System_Name.ToUpper().Trim() == filename.ToUpper().Trim())
                                {
                                    found = true;
                                    break;
                                }
                            }

                            // Not found, so add it
                            if (!found)
                            {
                                // Determine the label if it was missing or identical to file name
                                if ((label.Length == 0) || (label == filename))
                                {
                                    label = filename;
                                    int first_period_index = label.IndexOf('.');
                                    if (first_period_index > 0)
                                    {
                                        label = label.Substring(0, first_period_index);
                                    }
                                }

                                // Add the root to the download tree, if not existing
                                Division_TreeNode newRoot;
                                if (Return_Package.Divisions.Download_Tree.Roots.Count == 0)
                                {
                                    newRoot = new Division_TreeNode("Main", String.Empty);
                                    Return_Package.Divisions.Download_Tree.Roots.Add(newRoot);
                                }
                                else
                                {
                                    newRoot = (Division_TreeNode) Return_Package.Divisions.Download_Tree.Roots[0];
                                }

                                // Add a page for this, with the provided label if there was one
                                Page_TreeNode newPage = new Page_TreeNode(label);
                                newRoot.Nodes.Add(newPage);

                                // Now, add this existing file
                                newPage.Files.Add(thisDownloadFile);

                                // Add to the list of files added (in case it appears twice)
                                newStructureDownloads.Add(thisDownloadFile);
                            }
                        }
                    }
                    else
                    {
                        // Ensure a file of this name doesn't already exist
                        foreach (SobekCM_File_Info existingFile in newStructureDownloads)
                        {
                            if (existingFile.System_Name.ToUpper().Trim() == filename.ToUpper().Trim())
                            {
                                found = true;
                                break;
                            }
                        }

                        // Not found, so add it
                        if (!found)
                        {
                            // Determine the label if it was missing or identical to file name
                            if ((label.Length == 0) || (label == filename))
                            {
                                label = filename;
                                int first_period_index = label.IndexOf('.');
                                if (first_period_index > 0)
                                {
                                    label = label.Substring(0, first_period_index);
                                }
                            }

                            // Add the root to the download tree, if not existing
                            Division_TreeNode newRoot;
                            if (Return_Package.Divisions.Download_Tree.Roots.Count == 0)
                            {
                                newRoot = new Division_TreeNode("Main", String.Empty);
                                Return_Package.Divisions.Download_Tree.Roots.Add(newRoot);
                            }
                            else
                            {
                                newRoot = (Division_TreeNode) Return_Package.Divisions.Download_Tree.Roots[0];
                            }

                            // Add a page for this, with the provided label if there was one
                            Page_TreeNode newPage = new Page_TreeNode(label);
                            newRoot.Nodes.Add(newPage);

                            // Now, add this existing file
                            SobekCM_File_Info thisDownloadFile = new SobekCM_File_Info(filename);
                            newPage.Files.Add(thisDownloadFile);

                            // Add to the list of files added (in case it appears twice)
                            newStructureDownloads.Add(thisDownloadFile);
                        }
                    }
                }
            }

            #endregion

            #region Special code for distributing any page-level coordinate information read from the old SobekCM coordinate metadata

            // Get the geospatial data
            GeoSpatial_Information geoSpatial = Return_Package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if ((geoSpatial != null) && ( geoSpatial.Polygon_Count > 0 ))
            {
                // See if any has the page sequence filled out, which means it came from the old metadata system
                bool redistribute = false;
                foreach (Coordinate_Polygon thisPolygon in geoSpatial.Polygons)
                {
                    if (thisPolygon.Page_Sequence > 0)
                    {
                        redistribute = true;
                        break;
                    }
                }

                // If we need to redistribute, get started!
                if (redistribute)
                {
                    // Get the pages, by sequence
                    List<abstract_TreeNode> pagesBySequence = Return_Package.Divisions.Physical_Tree.Pages_PreOrder;
                    List<Coordinate_Polygon> polygonsToRemove = new List<Coordinate_Polygon>();

                    // Step through each polygon
                    foreach (Coordinate_Polygon thisPolygon in geoSpatial.Polygons)
                    {
                        if ((thisPolygon.Page_Sequence > 0) && ( thisPolygon.Page_Sequence <= pagesBySequence.Count ))
                        {
                            // Get the page
                            abstract_TreeNode thisPageFromSequence = pagesBySequence[thisPolygon.Page_Sequence - 1];

                            // We can assume this page does not already have the coordiantes
                            GeoSpatial_Information thisPageCoord = new GeoSpatial_Information();
                            thisPageFromSequence.Add_Metadata_Module( GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, thisPageCoord );
                            thisPageCoord.Add_Polygon( thisPolygon);

                            // Remove this from the package-level coordinates
                            polygonsToRemove.Add(thisPolygon);
                        }
                    }

                    // Now, remove all polygons flagged to be removed
                    foreach (Coordinate_Polygon thisPolygon in polygonsToRemove)
                    {
                        geoSpatial.Remove_Polygon(thisPolygon);
                    }
                }
            }

            #endregion

            #region Copy any serial hierarchy in the Behaviors.Serial_Info part into the bib portion, if not there

            // Do some final cleanup on the SERIAL HIERARCHY
            if ((Return_Package.Behaviors.hasSerialInformation) && (Return_Package.Behaviors.Serial_Info.Count > 0))
            {
                if ((Return_Package.Bib_Info.Series_Part_Info.Enum1.Length == 0) && (Return_Package.Bib_Info.Series_Part_Info.Year.Length == 0))
                {
                    if (Return_Package.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Newspaper)
                    {
                        Return_Package.Bib_Info.Series_Part_Info.Year = Return_Package.Behaviors.Serial_Info[0].Display;
                        Return_Package.Bib_Info.Series_Part_Info.Year_Index = Return_Package.Behaviors.Serial_Info[0].Order;

                        if (Return_Package.Behaviors.Serial_Info.Count > 1)
                        {
                            Return_Package.Bib_Info.Series_Part_Info.Month = Return_Package.Behaviors.Serial_Info[1].Display;
                            Return_Package.Bib_Info.Series_Part_Info.Month_Index = Return_Package.Behaviors.Serial_Info[1].Order;
                        }
                    }

                    if (Return_Package.Behaviors.Serial_Info.Count > 2)
                    {
                        Return_Package.Bib_Info.Series_Part_Info.Day = Return_Package.Behaviors.Serial_Info[2].Display;
                        Return_Package.Bib_Info.Series_Part_Info.Day_Index = Return_Package.Behaviors.Serial_Info[2].Order;
                    }
                }
                else
                {
                    Return_Package.Bib_Info.Series_Part_Info.Enum1 = Return_Package.Behaviors.Serial_Info[0].Display;
                    Return_Package.Bib_Info.Series_Part_Info.Enum1_Index = Return_Package.Behaviors.Serial_Info[0].Order;

                    if (Return_Package.Behaviors.Serial_Info.Count > 1)
                    {
                        Return_Package.Bib_Info.Series_Part_Info.Enum2 = Return_Package.Behaviors.Serial_Info[1].Display;
                        Return_Package.Bib_Info.Series_Part_Info.Enum2_Index = Return_Package.Behaviors.Serial_Info[1].Order;
                    }

                    if (Return_Package.Behaviors.Serial_Info.Count > 2)
                    {
                        Return_Package.Bib_Info.Series_Part_Info.Enum3 = Return_Package.Behaviors.Serial_Info[2].Display;
                        Return_Package.Bib_Info.Series_Part_Info.Enum3_Index = Return_Package.Behaviors.Serial_Info[2].Order;
                    }
                }
            }

            #endregion

            return true;
        }



        #endregion

        #region Read the METS Header

        private void read_mets_header(XmlReader R, SobekCM_Item Package)
        {
            // Since we are now using child trees, read here to get to the first (top-level) element
            R.Read();

            // Is this an empty element?
            bool isEmptyMetsHeader = R.IsEmptyElement;

            // Read the attributes on the METS header first
            if (R.MoveToAttribute("CREATEDATE"))
            {
                DateTime outDate1;
                if (DateTime.TryParse(R.Value.Replace("T", " ").Replace("Z", ""), out outDate1))
                    Package.METS_Header.Create_Date = outDate1;
            }
            if (R.MoveToAttribute("LASTMODDATE"))
            {
                DateTime outDate2;
                if (DateTime.TryParse(R.Value.Replace("T", " ").Replace("Z", ""), out outDate2))
                    Package.METS_Header.Modify_Date = outDate2;
            }
            if (R.MoveToAttribute("RECORDSTATUS"))
                Package.METS_Header.RecordStatus = R.Value;
            if (R.MoveToAttribute("ID"))
                Package.METS_Header.ObjectID = R.Value;

            // If this appears to be BibID_VID format, then assign those as well
            Package.BibID = Package.METS_Header.ObjectID;
            if ((Package.METS_Header.ObjectID.Length == 16) && (Package.METS_Header.ObjectID[10] == '_'))
            {
                bool char_found = false;
                foreach (char thisChar in Package.METS_Header.ObjectID.Substring(11))
                {
                    if (!char.IsNumber(thisChar))
                    {
                        char_found = true;
                    }
                }
                if (!char_found)
                {
                    string objectid = Package.METS_Header.ObjectID;
                    Package.BibID = objectid.Substring(0, 10);
                    Package.VID = objectid.Substring(11);
                }
            }

            // If this is an empty METS header, skip the rest
            if (isEmptyMetsHeader)
                return;

            // Loop through reading each XML node
            int agent_type = -1;
            while (R.Read())
            {
                // If this is the end of this section, return
                if (R.NodeType == XmlNodeType.EndElement)
                {
                    if ((R.Name == "METS:metsHdr") || (R.Name == "metsHdr"))
                        return;
                    if ((R.Name == "METS:agent") || (R.Name == "agent"))
                        agent_type = -1;
                }

                // Essentially a small enumeration here
                const int UNKNOWN_TYPE = -1;
                const int ORGANIZATION = 1;
                const int OTHER = 2;
                const int INDIVIDUAL = 3;

                if (R.NodeType == XmlNodeType.Element)
                {
                    switch (R.Name.Replace("METS:", ""))
                    {
                        case "agent":
                            if ((R.MoveToAttribute("ROLE")) && (R.GetAttribute("ROLE") == "CREATOR") && (R.MoveToAttribute("TYPE")))
                            {
                                switch (R.Value)
                                {
                                    case "ORGANIZATION":
                                        agent_type = ORGANIZATION;
                                        break;

                                    case "OTHER":
                                        agent_type = OTHER;
                                        break;

                                    case "INDIVIDUAL":
                                        agent_type = INDIVIDUAL;
                                        break;

                                    default:
                                        agent_type = UNKNOWN_TYPE;
                                        break;
                                }
                            }
                            break;

                        case "name":
                            switch (agent_type)
                            {
                                case ORGANIZATION:
                                    R.Read();
                                    Package.METS_Header.Creator_Organization = R.Value;
                                    Package.Bib_Info.Source.Code = R.Value;
                                    Package.Bib_Info.Source.Statement = R.Value;
                                    if (R.Value.IndexOf(",") < 0)
                                    {
                                        // Some presets for source codes in Florida
                                        switch (R.Value.ToUpper())
                                        {
                                            case "UF":
                                                Package.Bib_Info.Source.Statement = "University of Florida";
                                                break;

                                            case "FS":
                                            case "FSU":
                                                Package.Bib_Info.Source.Statement = "Florida State University";
                                                break;

                                            case "UCF":
                                            case "CF":
                                                Package.Bib_Info.Source.Statement = "University of Central Florida";
                                                break;

                                            case "USF":
                                            case "SF":
                                                Package.Bib_Info.Source.Statement = "University of South Florida";
                                                break;

                                            case "UNF":
                                            case "NF":
                                                Package.Bib_Info.Source.Statement = "University of North Florida";
                                                break;

                                            case "UWF":
                                            case "WF":
                                                Package.Bib_Info.Source.Statement = "University of West Florida";
                                                break;

                                            case "FIU":
                                            case "FI":
                                                Package.Bib_Info.Source.Statement = "Florida International University";
                                                break;

                                            case "FGCU":
                                            case "FG":
                                            case "GC":
                                                Package.Bib_Info.Source.Statement = "Florida Gulf Coast University";
                                                break;

                                            case "FAMU":
                                            case "AM":
                                                Package.Bib_Info.Source.Statement = "Florida Agricultural and Mechanical University";
                                                break;

                                            case "FAU":
                                                Package.Bib_Info.Source.Statement = "Florida Atlantic University";
                                                break;

                                            case "FCLA":
                                                Package.Bib_Info.Source.Statement = "Florida Center for Library Automation";
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        string code = R.Value.Substring(0, R.Value.IndexOf(","));
                                        string name = R.Value.Substring(R.Value.IndexOf(",") + 1);
                                        Package.Bib_Info.Source.Statement = name;
                                        Package.Bib_Info.Source.Code = code;
                                    }
                                    break;

                                case OTHER:
                                    R.Read();
                                    Package.METS_Header.Creator_Software = R.Value;
                                    break;

                                case INDIVIDUAL:
                                    R.Read();
                                    Package.METS_Header.Creator_Individual = R.Value;
                                    break;
                            }
                            break;

                        case "note":
                            switch (agent_type)
                            {
                                case ORGANIZATION:
                                    R.Read();
                                    Package.METS_Header.Add_Creator_Org_Notes(R.Value);
                                    break;

                                case INDIVIDUAL:
                                    R.Read();
                                    Package.METS_Header.Add_Creator_Individual_Notes(R.Value);
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Collect and store the DMD (bibliographic description) sections

        /// <summary> Reads the descriptive metadata section from a valid METS file </summary>
        /// <param name="R"> XmlTextReader for the given METS file </param>
        /// <returns> The unanalyzed dmd section </returns>    
        private Unanalyzed_METS_Section store_dmd_sec(XmlReader R)
        {
            // Save all the inner XML and attributed for later analysis
            string dmdSecId = String.Empty;
            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();

            // Get the attributes for this DMD section
            R.Read();
            while (R.MoveToNextAttribute())
            {
                attributes.Add(new KeyValuePair<string, string>(R.Name, R.Value));
                if (R.Name == "ID")
                    dmdSecId = R.Value;
            }

            // The next new element should be mdWrap, but read through white spaces
            // and get to the mdwrap element
            do
            {
                R.Read();
            } while (R.Name.ToLower().Replace("mets:","") != "mdwrap");

            // Now, should be at the mdWrap element
            string outerXML = R.ReadOuterXml();
            return new Unanalyzed_METS_Section(attributes, dmdSecId, outerXML);
        }

        #endregion

        #region Collect and store the AMD sections 

        private Unanalyzed_METS_Section store_amd_sec( XmlReader R )
        {
            // Save all the inner XML and attributed for later analysis
            string amdSecId = String.Empty;
            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();

            // Get the attributes for this AMD section
            R.Read();
            while (R.MoveToNextAttribute())
            {
                attributes.Add(new KeyValuePair<string, string>(R.Name, R.Value));
                if (R.Name == "ID")
                    amdSecId = R.Value;
            }

            // The next new element should be mdWrap, but read through white spaces
            // and get to the mdwrap element
            do
            {
                R.Read();
            } while (R.Name.ToLower().Replace("mets:", "") != "mdwrap");

            // Now, should be at the mdWrap element
            string outerXML = R.ReadOuterXml();
            return new Unanalyzed_METS_Section(attributes, amdSecId, outerXML);
        }

        #endregion

        #region Read the File section

        private void read_file_sec(XmlReader R, bool Minimize_File_Info, Dictionary<string, SobekCM_File_Info> FilesByFileid)
        {
            string checkSum = String.Empty;
            string checkSumType = String.Empty;
            string fileID = String.Empty;
            string size = String.Empty;

			// Only allow ONE instance of each file in the METS
	        Dictionary<string, SobekCM_File_Info> filename_to_object = new Dictionary<string, SobekCM_File_Info>();

            // begin to loop through the XML DOM tree

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                switch (R.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if ((R.Name == "METS:fileSec") || (R.Name == "fileSec"))
                        {
                            return;
                        }
                        break;

                    case XmlNodeType.Element:
                        if (((R.Name == "METS:file") || (R.Name == "file")) && (R.HasAttributes) && (R.MoveToAttribute("ID")))
                        {
                            fileID = R.Value;

                            if (R.MoveToAttribute("CHECKSUM"))
                            {
                                checkSum = R.Value;
                                checkSumType = R.MoveToAttribute("CHECKSUMTYPE") ? R.Value : String.Empty;
                            }
                            else
                            {
                                checkSum = String.Empty;
                                checkSumType = String.Empty;
                            }

                            size = R.MoveToAttribute("SIZE") ? R.Value : String.Empty;
                        }

                        if (((R.Name == "METS:FLocat") || (R.Name == "FLocat")) && (R.HasAttributes))
                        {
                            // Determine the location type ( System or URL )
                            SobekCM_File_Info_Type_Enum locType = SobekCM_File_Info_Type_Enum.SYSTEM;
                            if (R.MoveToAttribute("LOCTYPE"))
                            {
                                if ( R.Value == "URL")
                                    locType = SobekCM_File_Info_Type_Enum.URL;
                            }

                            if (R.MoveToAttribute("xlink:href"))
                            {
                                // Get and clean up the system name
                                string systemName;
                                if ((locType == SobekCM_File_Info_Type_Enum.SYSTEM) && ( R.Value.IndexOf("http:") < 0 ))
                                    systemName = R.Value.Replace("%20", " ").Replace("/", "\\");
                                else
                                    systemName = R.Value.Replace("%20", " ");

	                            if (systemName.ToLower() != "web.config")
	                            {
	                                // Is this a new FILEID?
	                                SobekCM_File_Info newFile;
	                                if (!FilesByFileid.ContainsKey(fileID))
		                            {
										// In addition, is this a new FILENAME?
										if (filename_to_object.ContainsKey(systemName.ToUpper()))
										{
											newFile = filename_to_object[systemName.ToUpper()];
											FilesByFileid[fileID] = newFile;
										}
										else
										{
											newFile = new SobekCM_File_Info(systemName);
											FilesByFileid[fileID] = newFile;
											filename_to_object[systemName.ToUpper()] = newFile;
										}
		                            }
		                            else
		                            {
			                            newFile = FilesByFileid[fileID];
			                            // newFile.System_Name = systemName;  (SHOULD BE REDUNDANT - removed 5/2014)
		                            }

		                            if ((!Minimize_File_Info) && (!String.IsNullOrEmpty(checkSum)) && (!String.IsNullOrEmpty(checkSumType)))
		                            {
			                            newFile.Checksum = checkSum;
			                            newFile.Checksum_Type = checkSumType;
		                            }

		                            if (size.Length > 0)
		                            {
			                            try
			                            {
				                            newFile.Size = Convert.ToInt64(size);
			                            }
			                            catch
			                            {
			                            }
		                            }
	                            }
                            }
                        }
                        break;
                }
            } while (R.Read());
        }

        #endregion

        #region Read the Structure Map

        private static void read_struct_map(XmlReader R, SobekCM_Item Package, Dictionary<string, SobekCM_File_Info> FilesByFileid, Dictionary<string, abstract_TreeNode> DivisionDmdids, Dictionary<string, abstract_TreeNode> DivisionAmdids )
        {
            Stack<abstract_TreeNode> parentNodes = new Stack<abstract_TreeNode>();
            Dictionary<string, abstract_TreeNode> divisions_by_id = new Dictionary<string, abstract_TreeNode>();

            bool mainDivisionFound = false;
            Division_Tree thisDivTree = null;

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                switch (R.NodeType)
                {
                        // if EndElement, move up tree
                    case XmlNodeType.EndElement:
                        if (R.Name == "METS:structMap")
                        {
                            return;
                        }

                        if (R.Name == "METS:div")
                        {
                            // If there are more than one parent on the "parent stack" pop one off
                            if (parentNodes.Count > 0)
                                parentNodes.Pop();
                        }
                        break;

                        // if new element, add name and traverse tree
                    case XmlNodeType.Element:

                        // Is this the beginning of a structure map
                        if (R.Name == "METS:structMap")
                        {
                            thisDivTree = Package.Divisions.Physical_Tree;
                            if (R.MoveToAttribute("TYPE"))
                            {
                                if (R.Value.ToUpper() == "OTHER")
                                    thisDivTree = Package.Divisions.Download_Tree;
                            }
                        }

                        // Is this a new division?
                        if ((R.Name == "METS:div") && (R.HasAttributes))
                        {
                            // Since this is a new division, get all the possible attribute values or set to empty string
                            string dmdid = (R.MoveToAttribute("DMDID") ? R.Value : String.Empty);
                            string amdid = (R.MoveToAttribute("AMDID") ? R.Value : String.Empty);
                            string divID = (R.MoveToAttribute("DMDID") ? R.Value : String.Empty);
                            string divType = (R.MoveToAttribute("TYPE") ? R.Value : String.Empty);
                            string divLabel = (R.MoveToAttribute("LABEL") ? R.Value : String.Empty);

                            // Get the order
                            ushort divOrder;
                            if (R.MoveToAttribute("ORDER"))
                            {
                                if (!UInt16.TryParse(R.Value, out divOrder)) divOrder = 0;
                            }
                            else if (R.MoveToAttribute("ORDERLABEL"))
                            {
                                if (!UInt16.TryParse(R.Value, out divOrder)) divOrder = 0;
                            }
                            else
                            {
                                divOrder = 0;
                            }

                            // Was this an outer division, or the main division?
                            if (!mainDivisionFound) 
                            {
                                // This is an outer wrapper and NOT the MAIN division, so save this as an
                                // outer division (division greater than current digital resources), such as 
                                // used sometimes for serials or journals.
                                if (divType.ToUpper() != "MAIN")
                                {
                                    if (!Package.Divisions.Contains_Outer_Division(divLabel, divType))
                                    {
                                        Package.Divisions.Add_Outer_Division(divLabel, divOrder, divType);
                                    }
                                }
                                else
                                {
                                    mainDivisionFound = true;
                                }
                            }
                            else
                            {
                                // Get the parent node, if there is one
                                abstract_TreeNode parentNode = parentNodes.Count > 0 ? parentNodes.Peek() : null;

                                // Create this division
                                abstract_TreeNode bibNode;
                                if (divType.ToUpper() == "PAGE")
                                    bibNode = new Page_TreeNode(divLabel);
                                else
                                    bibNode = new Division_TreeNode(divType, divLabel);

                                // Check to make sure no repeat here                               
                                if (divID.IndexOf("_repeat") > 0)
                                {
                                    divID = divID.Substring(0, divID.IndexOf("_repeat"));
                                    if (divisions_by_id.ContainsKey(divID))
                                    {
                                        bibNode = divisions_by_id[divID];
                                    }
                                }
                                
                                // Get the DMD sec or AMD sec's 
                                if (dmdid.Length > 0)
                                {
                                    string[] divDmdSecIds = dmdid.Split(" ".ToCharArray());
                                    foreach( string thisId in divDmdSecIds )
                                    {
                                        DivisionDmdids[thisId] = bibNode;
                                    }                                        
                                }
                                if (amdid.Length > 0)
                                {
                                    string[] divAmdSecIds = amdid.Split(" ".ToCharArray());
                                    foreach (string thisId in divAmdSecIds)
                                    {
                                        DivisionAmdids[thisId] = bibNode;
                                    }
                                }

                                // If there is a parent, add to it
                                if (parentNode != null)
                                {
                                    ((Division_TreeNode) parentNode).Nodes.Add(bibNode);
                                }
                                else
                                {
                                    // No parent, so add this to the root
                                    thisDivTree.Roots.Add(bibNode);
                                }

                                // Now, add this to the end of the parent list, in case it has children
                                if (!R.IsEmptyElement)
                                {
                                    parentNodes.Push(bibNode);
                                }
                            }

                            R.MoveToElement();
                        }

                        // Is this a new file pointer applying to the last division?
                        if ((R.Name == "METS:fptr") && (R.MoveToAttribute("FILEID")))
                        {
                            // Get this file id
                            string fileID = R.Value;

                            // Get the file from the files by id dictionary
                            if (FilesByFileid.ContainsKey(fileID))
                            {
                                SobekCM_File_Info thisFile = FilesByFileid[fileID];

                                abstract_TreeNode pageParentNode = null;
                                if (parentNodes.Count > 0)
                                    pageParentNode = parentNodes.Peek();


                                if ((pageParentNode != null) && (pageParentNode.Page))
                                {
                                    Page_TreeNode asPageNode = (Page_TreeNode) pageParentNode;
                                    if (!asPageNode.Files.Contains(thisFile))
                                        asPageNode.Files.Add(thisFile);
                                }
                                else
                                {
                                    if (pageParentNode == null)
                                    {
                                        thisDivTree.Add_File(thisFile);
                                    }
                                    else
                                    {
                                        Division_TreeNode asDivNode = (Division_TreeNode) pageParentNode;

                                        Page_TreeNode newPage = new Page_TreeNode();
                                        asDivNode.Add_Child(newPage);

                                        //parentNodes.Push(newPage);

                                        newPage.Files.Add(thisFile);
                                    }
                                }
                            }
                        }
                        break;
                } // end switch
            } while (R.Read());
        }

        #endregion

        #region Read the Behavior section

        private void read_behavior_sec(XmlReader R, SobekCM_Item Package)
        {
            // Create the flags
            bool views_flag = false;
            bool interfaces_flag = false;

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((R.NodeType == XmlNodeType.EndElement) && (R.Name == "METS:behaviorSec"))
                    return;

                // Make sure this is the behaviorSec node and it has attributes?
                if ((R.Name == "METS:behaviorSec") && (R.HasAttributes))
                {
                    // Move to the ID node, if it exists.
                    if (R.MoveToAttribute("ID"))
                    {
                        // Is this the VIEWS behavior sec?
                        if (R.Value == "VIEWS")
                        {
                            views_flag = true;
                            interfaces_flag = false;
                        }

                        // Is this the INTERFACES behavior sec?
                        if (R.Value == "INTERFACES")
                        {
                            interfaces_flag = true;
                            views_flag = false;
                        }
                    }
                }

                // Process the views
                if (views_flag)
                {
                    // Create the sorted list
                    SortedList views_sorted = new SortedList();

                    string view_id = String.Empty;
                    string view_procedure = String.Empty;
                    string view_procedure_upper = String.Empty;
                    string view_label = String.Empty;
                    string view_attributes = String.Empty;

                    // begin to loop through the XML DOM tree
                    while (R.Read())
                    {
                        // Is this the end of this behavior sec?
                        if ((R.NodeType == XmlNodeType.EndElement) && (R.Name == "METS:behaviorSec"))
                        {
                            views_flag = false;
                            break;
                        }

                        // Is this an element node?  If so collect either the behavior id or the title
                        if (R.NodeType == XmlNodeType.Element)
                        {
                            // Is this a new behavior?
                            if ((R.Name == "METS:behavior") && (R.HasAttributes) && (R.MoveToAttribute("ID")))
                            {
                                // Get the view id
                                view_id = R.Value.ToUpper();
                            }

                            // Is this the new mechanism?
                            if ((R.Name == "METS:mechanism") && (R.HasAttributes))
                            {
                                if (R.MoveToAttribute("xlink:title"))
                                {
                                    // Get the title of this behavior mechanism?
                                    view_procedure = R.Value;
                                    view_procedure_upper = view_procedure.ToUpper();
                                }
                                if (R.MoveToAttribute("LABEL"))
                                {
                                    view_label = R.Value;
                                }
                            }
                        }

                        // If we have both an id and title, then add this view
                        if ((view_id.Length > 0) && (view_procedure.Length > 0))
                        {
                            // Get the view enum
                            View_Enum thisViewType = View_Enum.None;

                            if (view_procedure_upper.IndexOf("JP2_VIEWER") == 0)
                            {
                                thisViewType = View_Enum.JPEG2000;
                            }

                            if (view_procedure_upper.IndexOf("JPEG_VIEWER") == 0)
                            {
                                thisViewType = View_Enum.JPEG;
                            }

                            if (view_procedure_upper.IndexOf("TEXT_VIEWER") == 0)
                            {
                                thisViewType = View_Enum.TEXT;
                            }

                            if (view_procedure_upper.IndexOf("SANBORN_VIEWER") == 0)
                            {
                                thisViewType = View_Enum.SANBORN;
                            }

                            if (view_procedure_upper.IndexOf("RELATED_IMAGE_VIEWER") == 0)
                            {
                                thisViewType = View_Enum.RELATED_IMAGES;
                            }

                            if (view_procedure_upper.IndexOf("EPC_VIEWER") == 0)
                            {
                                thisViewType = View_Enum.EPHEMERAL_CITIES;
                            }

                            if (view_procedure_upper.IndexOf("STREETS") == 0)
                            {
                                thisViewType = View_Enum.STREETS;
                            }

                            if (view_procedure_upper.IndexOf("FEATURES") == 0)
                            {
                                thisViewType = View_Enum.FEATURES;
                            }

                            if (view_procedure_upper.IndexOf("HTML") == 0)
                            {
                                thisViewType = View_Enum.HTML;
                            }

                            // Get any attribute
                            int first_parenthesis = view_procedure.IndexOf("(");
                            int second_parenthesis = view_procedure.IndexOf(")");
                            if ((first_parenthesis > 0) && (second_parenthesis > (first_parenthesis + 1)))
                            {
                                view_attributes = view_procedure.Substring(first_parenthesis + 1, second_parenthesis - first_parenthesis - 1);
                                view_attributes = view_attributes.Replace("\"", " ").Replace("'", " ").Trim();
                            }

                            // Add this to the sorted list
                            views_sorted.Add(view_id, new View_Object(thisViewType, view_label, view_attributes));

                            // Clear this data
                            view_id = String.Empty;
                            view_procedure = String.Empty;
                            view_procedure_upper = String.Empty;
                            view_label = String.Empty;
                            view_attributes = String.Empty;
                        }
                    } // end while

                    // Add these views to the bib object
                    Package.Behaviors.Clear_Views();
                    for (int i = 0; i < views_sorted.Count; i++)
                    {
                        View_Object tempViewObject = (View_Object) views_sorted.GetByIndex(i);
                        if ((tempViewObject.View_Type != View_Enum.HTML) || (tempViewObject.Label != "Audio Clips") || (tempViewObject.Attributes != "UF12345678.htm"))
                            Package.Behaviors.Add_View(tempViewObject);
                    }

                    // If there were no views, add JPEG and then JP2 as default
                    if (Package.Behaviors.Views.Count == 0)
                    {
                        Package.Behaviors.Add_View(View_Enum.JPEG);
                        Package.Behaviors.Add_View(View_Enum.JPEG2000);
                    }
                }

                // Process the interfaces
                if (interfaces_flag)
                {
                    // Create the sorted list
                    SortedList interfaces_sorted = new SortedList();

                    string interface_id = String.Empty;
                    string interface_title = String.Empty;

                    // begin to loop through the XML DOM tree
                    while (R.Read())
                    {
                        // Is this the end of this behavior sec?
                        if ((R.NodeType == XmlNodeType.EndElement) && (R.Name == "METS:behaviorSec"))
                        {
                            interfaces_flag = false;
                            break;
                        }

                        // Is this an element node?  If so collect either the behavior id or the title
                        if (R.NodeType == XmlNodeType.Element)
                        {
                            // Is this a new behavior?
                            if ((R.Name.Trim() == "METS:behavior") && (R.HasAttributes) && (R.MoveToAttribute("ID")))
                            {
                                // Get the view id
                                interface_id = R.Value.ToUpper();
                            }

                            // Is this the new mechanism?
                            if ((R.Name.Trim() == "METS:mechanism") && (R.HasAttributes) && (R.MoveToAttribute("xlink:title")))
                            {
                                // Get the title of this behavior mechanism?
                                interface_title = R.Value.ToUpper();
                                interface_title = interface_title.Replace("_INTERFACE_LOADER", "");
                            }
                        }

                        // If we have both an id and title, then add this view
                        if ((interface_id.Length > 0) && (interface_title.Length > 0))
                        {
                            // Add this to the sorted list
                            interfaces_sorted.Add(interface_id, interface_title);

                            // Clear this data
                            interface_id = String.Empty;
                            interface_title = String.Empty;
                        }
                    } // end while

                    // Add these web skin to the bib object
                    Package.Behaviors.Clear_Web_Skins();
                    for (int i = 0; i < interfaces_sorted.Count; i++)
                    {
                        Package.Behaviors.Add_Web_Skin(interfaces_sorted.GetByIndex(i).ToString());
                    }
                }
            } while (R.Read());
        }

        #endregion
    }


}