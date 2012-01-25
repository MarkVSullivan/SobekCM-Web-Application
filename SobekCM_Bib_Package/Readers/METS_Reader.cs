using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Bib_Package.Divisions;

namespace SobekCM.Bib_Package.Readers
{
    /// <summary> Enumeration indicates the type (or types) of schema used within the DMD Section
    /// of the METS file to encode basic descriptive information about the material </summary>
    /// <remarks> This is used internally when analyzing a DMDSec before passing it to the appropriate
    /// schema's subreader </remarks>
    public enum METS_DMDSEC_Type_Enum : byte
    {
        /// <summary> No DMDSec schema identified </summary>
        None,

        /// <summary> DAITSS (Digital Archiving in The Sunshine State) schema used by the Florida Digital Archive </summary>
        DAITSS,

        /// <summary> DarwinCore schema used for storing zoological taxonomic information ( kingdom, phylum, etc.. ) </summary>
        DarwinCore,

        /// <summary> Standard dublin core which stores all the bibliographic information in roughly thirteen main (flattened) fields </summary>
        DublinCore,

        /// <summary> MarcXML format for encoding the MARC21 data in XML </summary>
        MarcXML,

        /// <summary> Metadata Object Description Standard stores complex bibliographic information about an item </summary>
        MODS,

        /// <summary> Custom SobekCM bibliographic section stores some library-specific fields or additional information that does not fit into any other schemas </summary>
        SobekCM,

        /// <summary> Custom SobekCM technical data which includes the width and height of all image files used for display </summary>
        SobekCM_File,

        /// <summary> Custom SobekCM map section holds coordinate (point, line, polygon) information for the coverage of the digital object </summary>
        SobekCM_Map,

        /// <summary> PALMM/FCLA standard for encoding thesis and dissertation specific information within a METS file </summary>
        Thesis_Dissertation

    }

    /// <summary> Enumeration indicates the type (or types) of schema used within the AMD Section
    /// of the METS file to encode administrative information about the material </summary>
    /// <remarks> This is used internally when analyzing an AMDSec before passing it to the appropriate
    /// schema's subreader </remarks>
    public enum METS_AMDSEC_Type_Enum : byte
    {
        /// <summary> No AMDSec schema identified </summary>
        None,

        /// <summary> Digital provenance administrative metadata standard employed by PALMM/FCLA </summary>
        digiprovMD,

        /// <summary> Rights administrative metadata standard employed by PALMM/FCLA </summary>
        rightsMD,

        /// <summary> Source administrative information employed by PALMM/FCLA </summary>
        sourceMD,

        /// <summary> Technical administrative information employed by PALMM/FCLA </summary>
        techMD
    }


    /// <summary> Class is used to read the UFDC METS files and METS-formatted streams into
    /// the <see cref="SobekCM_Item"/> objects </summary>
    public class METS_Reader
    {
        /// <summary> Constructor for a new instance of the METS_Reader class </summary>
        public METS_Reader()
        {
            // Do nothing 
        }

        /// <summary> Read the metadata from a METS file, and populates the provided
        /// SobekCM_Item object with the data </summary>
        /// <param name="METS_File"> File to read </param>
        /// <param name="returnPackage"> Object into which to read the data from the METS stream</param>
        public void Read_METS(string METS_File, SobekCM_Item returnPackage)
        {
            Read_METS(METS_File, returnPackage, false);
        }


        /// <summary> Read the metadata from a METS file, and populates the provided
        /// SobekCM_Item object with the data </summary>
        /// <param name="METS_File"> File to read </param>
        /// <param name="returnPackage"> Object into which to read the data from the METS stream</param>
        /// <param name="Minimize_File_Info"> Flag indicates to minimize the file information retained in memory</param>
        public void Read_METS(string METS_File, SobekCM_Item returnPackage, bool Minimize_File_Info)
        {
            Stream reader = new FileStream(METS_File, FileMode.Open, FileAccess.Read );
            returnPackage.Source_Directory = (new FileInfo(METS_File)).DirectoryName;
            Read_METS(reader, returnPackage);
        }


        /// <summary> Read the metadata from a METS formatted stream, and populates the provided
        /// SobekCM_Item object with the data </summary>
        /// <param name="METS_Stream"> Stream of METS-formatted data </param>
        /// <param name="returnPackage"> Object into which to read the data from the METS stream</param>
        public void Read_METS(Stream METS_Stream, SobekCM_Item returnPackage)
        {
            Read_METS(METS_Stream, returnPackage, false);
        }

        /// <summary> Read the metadata from a METS formatted stream, and populates the provided
        /// SobekCM_Item object with the data </summary>
        /// <param name="METS_Stream"> Stream of METS-formatted data </param>
        /// <param name="returnPackage"> Object into which to read the data from the METS stream</param>
        /// <param name="Minimize_File_Info"> Flag indicates to minimize the file information retained in memory</param>
        public void Read_METS(Stream METS_Stream, SobekCM_Item returnPackage, bool Minimize_File_Info )
        {
            // Keep a list of all the files created, by file id, as additional data is gathered
            // from the different locations ( amdSec, fileSec, structmap )
            Dictionary<string, SobekCM_File_Info> files_by_fileid = new Dictionary<string, SobekCM_File_Info>();

            // For now, to do support for old way of doing downloads, build a list to hold
            // the deprecated download files
            List<Download_Info_DEPRECATED> deprecatedDownloads = new List<Download_Info_DEPRECATED>();

            try
            {
                // Try to read the XML
                XmlTextReader r = new XmlTextReader(METS_Stream);

                string indent_space = String.Empty;
                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.ProcessingInstruction)
                    {
                        if (r.Name.ToLower() == "fcla")
                        {
                            string value = r.Value.ToLower();
                            if (value.IndexOf("fda=\"yes\"") >= 0)
                            {
                                returnPackage.DAITSS.toArchive = true;
                            }
                            if (value.IndexOf("fda=\"no\"") >= 0)
                            {
                                returnPackage.DAITSS.toArchive = false;
                            }
                            if (value.IndexOf("dl=\"yes\"") >= 0)
                            {
                                returnPackage.PALMM.toPALMM = true;
                            }
                            if (value.IndexOf("dl=\"no\"") >= 0)
                            {
                                returnPackage.PALMM.toPALMM = false;
                            }
                        }
                    }

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name.Replace("METS:", ""))
                        {
                            case "mets":
                                if (r.MoveToAttribute("OBJID"))
                                    returnPackage.METS.ObjectID = r.Value;
                                break;

                            case "metsHdr":
                                read_mets_header(r, returnPackage);
                                break;

                            case "dmdSec":
                            case "dmdSecFedora":
                                read_dmd_sec(r, returnPackage, deprecatedDownloads);
                                break;

                            case "amdSec":
                                read_amd_sec(r, returnPackage, files_by_fileid);
                                break;

                            case "fileSec":
                                read_file_sec(r, returnPackage, Minimize_File_Info, files_by_fileid);
                                break;

                            case "structMap":
                                if (!r.IsEmptyElement)
                                {
                                    read_struct_map(r, returnPackage, files_by_fileid);
                                }
                                break;

                            case "behaviorSec":
                                read_behavior_sec(r, returnPackage);
                                break;
                        }
                    }
                }

                // writer.Close();
                r.Close();

            }
            catch ( Exception ee )
            {
                string error = ee.ToString();
            }

            METS_Stream.Close();

            // For backward compatability, move from the old download system to the
            // new structure.  This has to happen here at the end so that we have access

                // Were there some downloads added here?
                if ( deprecatedDownloads.Count > 0)
                {
                    // Get the list of downloads from the download tree
                    List<SobekCM_File_Info> newStructureDownloads = returnPackage.Divisions.Download_Tree.All_Files;

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
                                    if ((label.Length == 0) || ( label == filename ))
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
                                    if (returnPackage.Divisions.Download_Tree.Roots.Count == 0)
                                    {
                                        newRoot = new Division_TreeNode("Main", String.Empty);
                                        returnPackage.Divisions.Download_Tree.Roots.Add(newRoot);
                                    }
                                    else
                                    {
                                        newRoot = ( Division_TreeNode) returnPackage.Divisions.Download_Tree.Roots[0];
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
                                if (returnPackage.Divisions.Download_Tree.Roots.Count == 0)
                                {
                                    newRoot = new Division_TreeNode("Main", String.Empty);
                                    returnPackage.Divisions.Download_Tree.Roots.Add(newRoot);
                                }
                                else
                                {
                                    newRoot = (Division_TreeNode)returnPackage.Divisions.Download_Tree.Roots[0];
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


            // Do some final cleanup on the SERIAL HIERARCHY
            if (( returnPackage.hasSerialInformation ) && ( returnPackage.Serial_Info.Count > 0))
            {
                if ((returnPackage.Bib_Info.Series_Part_Info.Enum1.Length == 0) && (returnPackage.Bib_Info.Series_Part_Info.Year.Length == 0))
                {
                    if (returnPackage.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Newspaper )
                    {
                        returnPackage.Bib_Info.Series_Part_Info.Year = returnPackage.Serial_Info[0].Display;
                        returnPackage.Bib_Info.Series_Part_Info.Year_Index = returnPackage.Serial_Info[0].Order;

                        if (returnPackage.Serial_Info.Count > 1)
                        {
                            returnPackage.Bib_Info.Series_Part_Info.Month = returnPackage.Serial_Info[1].Display;
                            returnPackage.Bib_Info.Series_Part_Info.Month_Index = returnPackage.Serial_Info[1].Order;
                        }
                    }

                    if (returnPackage.Serial_Info.Count > 2)
                    {
                        returnPackage.Bib_Info.Series_Part_Info.Day = returnPackage.Serial_Info[2].Display;
                        returnPackage.Bib_Info.Series_Part_Info.Day_Index = returnPackage.Serial_Info[2].Order;
                    }
                }
                else
                {
                    returnPackage.Bib_Info.Series_Part_Info.Enum1 = returnPackage.Serial_Info[0].Display;
                    returnPackage.Bib_Info.Series_Part_Info.Enum1_Index = returnPackage.Serial_Info[0].Order;

                    if (returnPackage.Serial_Info.Count > 1)
                    {
                        returnPackage.Bib_Info.Series_Part_Info.Enum2 = returnPackage.Serial_Info[1].Display;
                        returnPackage.Bib_Info.Series_Part_Info.Enum2_Index = returnPackage.Serial_Info[1].Order;
                    }

                    if (returnPackage.Serial_Info.Count > 2)
                    {
                        returnPackage.Bib_Info.Series_Part_Info.Enum3 = returnPackage.Serial_Info[2].Display;
                        returnPackage.Bib_Info.Series_Part_Info.Enum3_Index = returnPackage.Serial_Info[2].Order;
                    }

                }
            }
        }
  

        #region Read the METS Header

        private void read_mets_header(XmlTextReader r, SobekCM_Item package )
        {
            // Is this an empty element?
            bool isEmptyMetsHeader = r.IsEmptyElement;

            // Read the attributes on the METS header first
            try
            {
                if (r.MoveToAttribute("CREATEDATE"))
                    package.METS.Create_Date = Convert.ToDateTime(r.Value.Replace("T", " ").Replace("Z", ""));
            }
            catch
            {

            }

            try
            {
                if (r.MoveToAttribute("LASTMODDATE"))
                {
                    package.METS.Modify_Date = Convert.ToDateTime(r.Value.Replace("T", " ").Replace("Z", ""));
                }
            }
            catch
            {

            }

            if (r.MoveToAttribute("RECORDSTATUS"))
                package.METS.RecordStatus = r.Value;

            if (r.MoveToAttribute("ID"))
                package.METS.ObjectID = r.Value;

            // If this appears to be BibID_VID format, then assign those as well
            package.BibID = package.METS.ObjectID;
            if ((package.METS.ObjectID.Length == 16) && (package.METS.ObjectID[10] == '_'))
            {
                bool char_found = false;
                foreach (char thisChar in package.METS.ObjectID.Substring(11))
                {
                    if (!char.IsNumber(thisChar))
                    {
                        char_found = true;
                    }
                }
                if (!char_found)
                {
                    string objectid = package.METS.ObjectID;
                    package.BibID = objectid.Substring(0, 10);
                    package.VID = objectid.Substring(11);
                }
            }

            // If this is an empty METS header, skip the rest
            if (isEmptyMetsHeader)
                return;

            // Loop through reading each XML node
            int agent_type = -1;
            while (r.Read())
            {
                // If this is the end of this section, return
                if (r.NodeType == XmlNodeType.EndElement)
                {
                    if ((r.Name == "METS:metsHdr") || ( r.Name == "metsHdr" ))
                        return;
                    if (( r.Name == "METS:agent" ) || ( r.Name == "agent" ))
                        agent_type = -1;
                }

                // If this is the beginning of a DMD sec, also done
                if (( r.Name == "METS:dmdSec" ) || ( r.Name == "dmdSec" ))
                    return;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name.Replace("METS:",""))
                    {
                        case "agent":
                            if ((r.MoveToAttribute("ROLE")) && (r.GetAttribute("ROLE") == "CREATOR") && (r.MoveToAttribute("TYPE")))
                            {
                                switch (r.Value)
                                {
                                    case "ORGANIZATION":
                                        agent_type = 1;
                                        break;

                                    case "OTHER":
                                        agent_type = 2;
                                        break;

                                    case "INDIVIDUAL":
                                        agent_type = 3;
                                        break;

                                    default:
                                        agent_type = -1;
                                        break;
                                }
                            }
                            break;

                        case "name":
                            switch( agent_type )
                            {
                                case 1:
                                    r.Read();
                                    package.METS.Creator_Organization = r.Value;
                                    package.Bib_Info.Source.Code = r.Value;
                                    package.Bib_Info.Source.Statement = r.Value;
                                    if (r.Value.IndexOf(",") < 0)
                                    {
                                        // Some presets for source codes in Florida
                                        switch (r.Value.ToUpper())
                                        {
                                            case "UF":
                                                package.Bib_Info.Source.Statement = "University of Florida";
                                                break;

                                            case "FS":
                                            case "FSU":
                                                package.Bib_Info.Source.Statement = "Florida State University";
                                                break;

                                            case "UCF":
                                            case "CF":
                                                package.Bib_Info.Source.Statement = "University of Central Florida";
                                                break;

                                            case "USF":
                                            case "SF":
                                                package.Bib_Info.Source.Statement = "University of South Florida";
                                                break;

                                            case "UNF":
                                            case "NF":
                                                package.Bib_Info.Source.Statement = "University of North Florida";
                                                break;

                                            case "UWF":
                                            case "WF":
                                                package.Bib_Info.Source.Statement = "University of West Florida";
                                                break;

                                            case "FIU":
                                            case "FI":
                                                package.Bib_Info.Source.Statement = "Florida International University";
                                                break;

                                            case "FGCU":
                                            case "FG":
                                            case "GC":
                                                package.Bib_Info.Source.Statement = "Florida Gulf Coast University";
                                                break;

                                            case "FAMU":
                                            case "AM":
                                                package.Bib_Info.Source.Statement = "Florida Agricultural and Mechanical University";
                                                break;

                                            case "FAU":
                                                package.Bib_Info.Source.Statement = "Florida Atlantic University";
                                                break;

                                            case "FCLA":
                                                package.Bib_Info.Source.Statement = "Florida Center for Library Automation";
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        string code = r.Value.Substring(0, r.Value.IndexOf(","));
                                        string name = r.Value.Substring(r.Value.IndexOf(",") + 1);
                                        package.Bib_Info.Source.Statement = name;
                                        package.Bib_Info.Source.Code = code;
                                    }                                    
                                    break;

                                case 2:
                                    r.Read();
                                    package.METS.Creator_Software = r.Value;
                                    break;

                                case 3:
                                    r.Read();
                                    package.METS.Creator_Individual = r.Value;
                                    break;
                            }
                            break;

                        case "note":
                            switch( agent_type )
                            {
                                case 1:
                                    r.Read();
                                    package.METS.Add_Creator_Org_Notes(r.Value);
                                    if (r.Value.Trim().IndexOf("projects=") == 0)
                                        package.PALMM.PALMM_Project = r.Value.Trim().Replace("projects=", "");
                                    if (r.Value.Trim().IndexOf("server=") == 0)
                                        package.PALMM.PALMM_Server = r.Value.Trim().Replace("server=", "");
                                    break;

                                case 3:
                                    r.Read();
                                    package.METS.Add_Creator_Individual_Notes( r.Value );
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Read the DMD (bibliographic description) sections

        /// <summary> Reads the descriptive metadata section from a valid METS file </summary>
        /// <param name="r"> XmlTextReader for the given METS file </param>
        /// <param name="package"> Bibliographic item to load the data from the METS file into </param>
        /// <param name="deprecatedDownloads"> List of downloads read from the deprecated SobekCM donwloads tags </param>
        private void read_dmd_sec(XmlTextReader r, SobekCM_Item package, List<Download_Info_DEPRECATED> deprecatedDownloads)
        {
            // TODO: This no longer supports DMD secs for just a PORTION of the digital resource


            string dmdSecId = String.Empty;
            string sobekCmSchemeName = "sobekcm";
            METS_DMDSEC_Type_Enum readerType = METS_DMDSEC_Type_Enum.None;

            // Before continuing, save all the inner XML and attributes, in case this section cannot be handled
            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();

            // Get the attributes for this DMD section
            while( r.MoveToNextAttribute() )
            {
                attributes.Add(new KeyValuePair<string, string>(r.Name, r.Value));
                if ( r.Name == "ID" )
                    dmdSecId = r.Value;
            }

            if (dmdSecId.IndexOf("pageModsBib") >= 0)
                return;            

            // The next new element should be mdWrap, but read through white spaces
            do
            {
                r.Read();
            } while ((r.NodeType != XmlNodeType.Element) || (r.Name == "METS:descMD"));
            if (r.Name.Replace("METS:", "") == "mdWrap")
            {
                // MDTYPE is actually a required attribute
                if (r.MoveToAttribute("MDTYPE"))
                {
                    string mdtype_adjusted = r.Value;
                    if (mdtype_adjusted == "OTHER")
                    {
                        string temp = String.Empty;
                        if (r.MoveToAttribute("OTHERMDTYPE"))
                            temp = r.Value.ToUpper();
                        if ((temp.Length == 0) && (r.MoveToAttribute("LABEL")))
                            temp = r.Value.ToUpper();

                        if (temp.Length > 0)
                        {
                            if (temp.IndexOf("MODS") >= 0)
                                temp = "MODS";
                            if (temp.IndexOf("MARC") >= 0)
                                temp = "MARC";
                            if (temp.IndexOf("SOBEKCM") >= 0)
                                temp = "SOBEKCM";
                            if (temp.IndexOf("PALMM") >= 0)
                                temp = "PALMM";
                            if (temp.IndexOf("UFDC") >= 0)
                                temp = "UFDC";
                            else if (temp.IndexOf("DC") >= 0)
                                temp = "DC";
                            if (temp.IndexOf("DUBLIN CORE") >= 0)
                                temp = "DC";
                            mdtype_adjusted = temp;
                        }
                    }


                    switch (mdtype_adjusted.ToUpper())
                    {
                        case "MARC":
                            readerType = METS_DMDSEC_Type_Enum.MarcXML;
                            break;

                        case "MODS":
                            readerType = METS_DMDSEC_Type_Enum.MODS;
                            break;

                        case "SOBEKCM":
                            readerType = METS_DMDSEC_Type_Enum.SobekCM;
                            break;

                        case "UFDC":
                            readerType = METS_DMDSEC_Type_Enum.SobekCM;
                            sobekCmSchemeName = "ufdc";
                            break;

                        case "DLOC":
                            readerType = METS_DMDSEC_Type_Enum.SobekCM;
                            sobekCmSchemeName = "dloc";
                            break;

                        case "UFDC_MAP":
                        case "SOBEKCM_MAP":
                            readerType = METS_DMDSEC_Type_Enum.SobekCM_Map;
                            break;

                        case "PALMM":
                            readerType = METS_DMDSEC_Type_Enum.Thesis_Dissertation;
                            break;

                        case "DC":
                            readerType = METS_DMDSEC_Type_Enum.DublinCore;
                            break;

                        case "DARWINCORE":
                            readerType = METS_DMDSEC_Type_Enum.DarwinCore;
                            break;
                    }
                }
            }

            // If no reader type was selected, just save as an unanalyzed METS section
            if (readerType == METS_DMDSEC_Type_Enum.None)
            {
                r.MoveToElement();
                string outerXML = r.ReadOuterXml();
                package.Add_Unanalyzed_DMDSEC(attributes, dmdSecId, outerXML);
            }
            else
            {
                switch (readerType)
                {
                    case METS_DMDSEC_Type_Enum.MarcXML:
                        SubReaders.MarcXML_SubReader.Read_MarcXML_Info(r, package);
                        break;

                    case METS_DMDSEC_Type_Enum.MODS:
                        SubReaders.MODS_SubReader.Read_MODS_Info(r, package);
                        break;

                    case METS_DMDSEC_Type_Enum.SobekCM:
                        SubReaders.SobekCM_SubReader.Read_SobekCM_DMD_Sec(r, package, sobekCmSchemeName, deprecatedDownloads);
                        break;

                    case METS_DMDSEC_Type_Enum.SobekCM_Map:
                        SubReaders.SobekCM_SubReader.Read_SobekCM_Map_DMD_Sec(r, package);
                        break;

                    case METS_DMDSEC_Type_Enum.Thesis_Dissertation:
                        SubReaders.Thesis_Dissertation_Reader.Read_ETD_Sec(r, package);
                        break;

                    case METS_DMDSEC_Type_Enum.DublinCore:
                        SubReaders.Dublin_Core_SubReader.Read_Dublin_Core_Info(r, package);
                        break;

                    case METS_DMDSEC_Type_Enum.DarwinCore:
                        SubReaders.DarwinCore_SubReader.Read_DarwinCore_Sec(r, package);
                        break;
                }
            }
        }

        #endregion

        #region Read the AMD section (technical data about the files)

        private void read_amd_sec(XmlTextReader r, SobekCM_Item package, Dictionary<string, SobekCM_File_Info> files_by_fileid )
        {
            string amdSecId = String.Empty;
            METS_AMDSEC_Type_Enum readerType = METS_AMDSEC_Type_Enum.None;

            // Before continuing, save all the inner XML and attributes, in case this section cannot be handled
            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();

            // Get the attributes for this DMD section
            while (r.MoveToNextAttribute())
            {
                attributes.Add(new KeyValuePair<string, string>(r.Name, r.Value));
                if (r.Name == "ID")
                    amdSecId = r.Value;
            }

            // Read the next item
            r.Read();

            // In order to keep any unhandled XML, build a stack 
            Stack<string> xmlDeclarationNodesStack = new Stack<string>();
            Stack<string> xmlEndNodesStack = new Stack<string>();

            // We'll use a stringbuilder to retain any bits of this amdSec which is not analyzed
            StringBuilder innerXmlBuilder = new StringBuilder();

            // The Read to the next element definition
            do
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    string element_name = r.Name;
                    if (( element_name.IndexOf("METS:") == 0 ) && ( element_name.Length > 5 ))
                        element_name = element_name.Substring(5);
                    if (readerType == METS_AMDSEC_Type_Enum.None)
                    {
                        switch (element_name)
                        {
                            case "digiprovMD":
                                readerType = METS_AMDSEC_Type_Enum.digiprovMD;
                                break;

                            case "rightsMD":
                                readerType = METS_AMDSEC_Type_Enum.rightsMD;
                                break;

                            case "sourceMD":
                                readerType = METS_AMDSEC_Type_Enum.sourceMD;
                                break;

                            case "techMD":
                                readerType = METS_AMDSEC_Type_Enum.techMD;
                                break;
                        }

                        // Add this node onto the two stacks
                        if (r.HasAttributes)
                        {
                            string name = r.Name;
                            StringBuilder nodeNameBuilder = new StringBuilder("<" + r.Name);
                            while (r.MoveToNextAttribute())
                            {
                                nodeNameBuilder.Append(" " + r.Name + "=\"" + r.Value + "\"");
                            }
                            xmlDeclarationNodesStack.Push(nodeNameBuilder.ToString() + ">");
                            xmlEndNodesStack.Push("</" + name + ">");
                        }
                        else
                        {
                            xmlDeclarationNodesStack.Push("<" + r.Name + ">");
                            xmlEndNodesStack.Push("</" + r.Name + ">");
                        }

                    }
                    else
                    {
                        // Are we actually coming to important data in this section now?
                        bool handled = false;
                        if ((element_name != "mdWrap") && (element_name != "xmlData"))
                        {
                            // Look for the source code in the palmm:entityDesc in the sourceMD section
                            if ((readerType == METS_AMDSEC_Type_Enum.sourceMD) && (r.Name == "palmm:entityDesc"))
                            {
                                if ((r.MoveToAttribute("SOURCE")) && (package.Bib_Info.Source.Code.Length == 0))
                                {
                                    if (r.Value.Length > 0)
                                    {
                                        package.Bib_Info.Source.Code = r.Value;
                                    }
                                }
                                handled = true;
                            }

                            // Look for specific rights fields within the rightsMD section
                            if ((readerType == METS_AMDSEC_Type_Enum.rightsMD) && (r.Name.IndexOf("rightsmd:") == 0))
                            {
                                // For now, just read through this
                                do
                                {
                                    r.Read();
                                } while ((r.NodeType != XmlNodeType.EndElement) && (r.Name != "mdWrap"));
                                handled = true;
                            }

                            // Look for the SobekCM file information in the techMD section
                            if ((readerType == METS_AMDSEC_Type_Enum.techMD) && ((r.Name.IndexOf("FileInfo") == 0 ) || ( r.Name.IndexOf(":FileInfo") > 0 )))
                            {
                                SubReaders.SobekCM_SubReader.Read_SobekCM_File_Sec(r, package, "sobekcm", files_by_fileid);
                                handled = true;
                            }

                            // Look for the daitss information in the digiprovMD section
                            if ((readerType == METS_AMDSEC_Type_Enum.digiprovMD) && (r.Name.IndexOf("daitss:") == 0))
                            {
                                SubReaders.DAITSS_SubReader.Read_Daitss_Sec(r, package);
                                handled = true;
                            }

                            // If not handled, and no longer the standard type tags, this will be retained
                            // as an unanalyzed AMDSEC portion, so do that now
                            if (!handled)
                            {
                                innerXmlBuilder.Append(r.ReadOuterXml());
                                while (xmlDeclarationNodesStack.Count > 0)
                                {
                                    innerXmlBuilder.Insert(0, xmlDeclarationNodesStack.Pop() + "\r\n");
                                    innerXmlBuilder.AppendLine(xmlEndNodesStack.Pop());
                                }

                            }
                            else
                            {
                                // Since it was handled, clear the stacks
                                xmlDeclarationNodesStack.Clear();
                                xmlEndNodesStack.Clear();
                            }

                            readerType = METS_AMDSEC_Type_Enum.None;
                        }
                        else
                        {
                            // Since these are just the standard links, push them onto the stacks
                            // Add this node onto the two stacks
                            if (r.HasAttributes)
                            {
                                string name = r.Name;
                                StringBuilder nodeNameBuilder = new StringBuilder("<" + r.Name);
                                while (r.MoveToNextAttribute())
                                {
                                    nodeNameBuilder.Append(" " + r.Name + "=\"" + r.Value + "\"");
                                }
                                xmlDeclarationNodesStack.Push(nodeNameBuilder.ToString() + ">");
                                xmlEndNodesStack.Push("</" + name + ">");
                            }
                            else
                            {
                                xmlDeclarationNodesStack.Push("<" + r.Name + ">");
                                xmlEndNodesStack.Push("</" + r.Name + ">");
                            }

                        }
                    }                    
                }

                
            } while ((r.Read()) && ((r.NodeType != XmlNodeType.EndElement ) || ( r.Name != "METS:amdSec" )));

            // If there is some unanalyzed, add it here
            if (innerXmlBuilder.Length > 0)
            {
                package.Add_Unanalyzed_AMDSEC(attributes, amdSecId, innerXmlBuilder.ToString());
            }
        }

        #endregion

        #region Read the File section 

        private void read_file_sec(XmlTextReader r, SobekCM_Item package, bool Minimize_File_Info, Dictionary<string, SobekCM_File_Info> files_by_fileid )
        {
            string systemName = String.Empty;
            string checkSum = String.Empty;
            string checkSumType = String.Empty;
            string fileID = String.Empty;
            string size = String.Empty;

            // begin to loop through the XML DOM tree
            SobekCM_File_Info newFile;

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                switch (r.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if ((r.Name == "METS:fileSec") || ( r.Name == "fileSec" ))
                        {
                            return;
                        }
                        break;

                    case XmlNodeType.Element:
                        if (((r.Name == "METS:file") || ( r.Name == "file" )) && (r.HasAttributes) && (r.MoveToAttribute("ID")))
                        {
                            fileID = r.Value;

                            if (r.MoveToAttribute("CHECKSUM"))
                            {
                                checkSum = r.Value;
                                if (r.MoveToAttribute("CHECKSUMTYPE"))
                                    checkSumType = r.Value;
                                else
                                    checkSumType = String.Empty;
                            }
                            else
                            {
                                checkSum = String.Empty;
                                checkSumType = String.Empty;
                            }

                            if (r.MoveToAttribute("SIZE"))
                                size = r.Value;
                            else
                                size = String.Empty;
                        }

                        if (((r.Name == "METS:FLocat") || ( r.Name == "FLocat")) && (r.HasAttributes)
                            && (r.MoveToAttribute("OTHERLOCTYPE")) && (r.GetAttribute("OTHERLOCTYPE") == "SYSTEM"))
                        {
                            if (r.MoveToAttribute("xlink:href"))
                            {
                                systemName = r.Value.Replace("%20", " ");
                                newFile = null;
                                if (!files_by_fileid.ContainsKey(fileID))
                                {
                                    newFile = new SobekCM_File_Info(systemName);
                                    files_by_fileid[fileID] = newFile;
                                }
                                else
                                {
                                    newFile = files_by_fileid[fileID];
                                    newFile.System_Name = systemName;
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
                        break;
                }
            } while (r.Read()) ;
        }
        
        #endregion

        #region Read the Structure Map

        private void read_struct_map(XmlTextReader r, SobekCM_Item package, Dictionary<string, SobekCM_File_Info> files_by_fileid)
        {
            Stack<abstract_TreeNode> parentNodes = new Stack<abstract_TreeNode>();
            Dictionary<string, abstract_TreeNode> divisions_by_id = new Dictionary<string,abstract_TreeNode>();

            string parentID = String.Empty;
            string divID = String.Empty;
            string divType = String.Empty;
            string divLabel = String.Empty;
            string fileID = String.Empty;
            ushort divOrder = 0;
            bool topDivision = true;
            abstract_TreeNode parentNode;
            Division_Tree thisDivTree = null;

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                switch (r.NodeType)
                {
                    // if EndElement, move up tree
                    case XmlNodeType.EndElement:
                        if (r.Name == "METS:structMap")
                        {
                            return;
                        }

                        if (r.Name == "METS:div")
                        {
                            // If there are more than one parent on the "parent stack" pop one off
                            if (parentNodes.Count > 0)
                                parentNodes.Pop();
                        }
                        break;

                    // if new element, add name and traverse tree
                    case XmlNodeType.Element:

                        // Is this the beginning of a structure map
                        if (r.Name == "METS:structMap")
                        {
                            thisDivTree = package.Divisions.Physical_Tree;
                            if (r.MoveToAttribute("TYPE"))
                            {
                                if (r.Value.ToUpper() == "OTHER")
                                    thisDivTree = package.Divisions.Download_Tree;
                            }
                        }

                        // Is this a new division?
                        if ((r.Name == "METS:div") && (r.HasAttributes))
                        {
                            // Just skip the top divisoin, which is the division for the entire item
                            if (topDivision)
                            {
                                topDivision = false;
                            }
                            else
                            {
                                // Get the parent node, if there is one
                                if (parentNodes.Count > 0)
                                    parentNode = parentNodes.Peek();
                                else
                                    parentNode = null;

                                // Get the ID
                                if (r.MoveToAttribute("ID"))
                                    divID = r.Value;
                                else
                                    divID = String.Empty;

                                // Get the type
                                if (r.MoveToAttribute("TYPE"))
                                    divType = r.Value;
                                else
                                    divType = String.Empty;

                                // Get the label
                                if (r.MoveToAttribute("LABEL"))
                                    divLabel = r.Value;
                                else
                                    divLabel = String.Empty;

                                // Get the order
                                if (r.MoveToAttribute("ORDER"))
                                {
                                    try
                                    {
                                        divOrder = Convert.ToUInt16(r.Value);
                                    }
                                    catch
                                    {
                                        divOrder = 0;
                                    }
                                }
                                else
                                {
                                    divOrder = 0;
                                }

                                // Create this division
                                abstract_TreeNode bibNode;
                                if ( divType.ToUpper() == "PAGE" )
                                    bibNode = new Page_TreeNode( divLabel );
                                else
                                    bibNode = new Division_TreeNode( divType, divLabel );

                                // Check to make sure no repeat here                               
                                if ( divID.IndexOf("_repeat") > 0 )
                                {
                                    divID = divID.Substring(0, divID.IndexOf("_repeat"));
                                    if ( divisions_by_id.ContainsKey( divID ))
                                    {
                                        bibNode = divisions_by_id[divID];
                                    }
                                }

                                // If there is a parent, add to it
                                if ( parentNode != null )
                                {
                                    (( Division_TreeNode ) parentNode).Nodes.Add( bibNode );
                                }
                                else
                                {
                                    // No parent, so add this to the root
                                    thisDivTree.Roots.Add(bibNode);
                                }

                                // Now, add this to the end of the parent list, in case it has children
                                r.MoveToElement();
                                if (!r.IsEmptyElement)
                                {
                                    parentNodes.Push(bibNode);
                                }
                            }
                        }

                        // Is this a new file pointer applying to the last division?
                        if ((r.Name == "METS:fptr") && (r.MoveToAttribute("FILEID")))
                        {
                            // Get this file id
                            fileID = r.Value;

                            // Get the file from the files by id dictionary
                            if (files_by_fileid.ContainsKey(fileID))
                            {
                                SobekCM_File_Info thisFile = files_by_fileid[fileID];
                                abstract_TreeNode pageParentNode = parentNodes.Peek();
                                if (pageParentNode.Page)
                                {
                                    Page_TreeNode asPageNode = (Page_TreeNode) pageParentNode;
                                    if ( !asPageNode.Files.Contains( thisFile ))
                                        asPageNode.Files.Add( thisFile );
                                }
                            }
                        }
                        break;
                } // end switch
            } while (r.Read());
        }

        #endregion

        #region Read the Behavior section

        private void read_behavior_sec(XmlTextReader r, SobekCM_Item package)
        {
            // Create the flags
            bool views_flag = false;
            bool interfaces_flag = false;

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "METS:behaviorSec"))
                    return;
                
                // Make sure this is the behaviorSec node and it has attributes?
                if ((r.Name == "METS:behaviorSec") && (r.HasAttributes))
                {
                    // Move to the ID node, if it exists.
                    if (r.MoveToAttribute("ID"))
                    {
                        // Is this the VIEWS behavior sec?
                        if (r.Value == "VIEWS")
                        {
                            views_flag = true;
                            interfaces_flag = false;
                        }

                        // Is this the INTERFACES behavior sec?
                        if (r.Value == "INTERFACES")
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
                    while (r.Read())
                    {
                        // Is this the end of this behavior sec?
                        if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "METS:behaviorSec"))
                        {
                            views_flag = false;
                            break;
                        }

                        // Is this an element node?  If so collect either the behavior id or the title
                        if (r.NodeType == XmlNodeType.Element)
                        {
                            // Is this a new behavior?
                            if ((r.Name == "METS:behavior") && (r.HasAttributes) && (r.MoveToAttribute("ID")))
                            {
                                // Get the view id
                                view_id = r.Value.ToUpper();
                            }

                            // Is this the new mechanism?
                            if ((r.Name == "METS:mechanism") && (r.HasAttributes))
                            {
                                if (r.MoveToAttribute("xlink:title"))
                                {
                                    // Get the title of this behavior mechanism?
                                    view_procedure = r.Value;
                                    view_procedure_upper = view_procedure.ToUpper();
                                }
                                if (r.MoveToAttribute("LABEL"))
                                {
                                    view_label = r.Value;
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
                    package.SobekCM_Web.Clear_Views();
                    for (int i = 0; i < views_sorted.Count; i++)
                    {
                        View_Object tempViewObject = (View_Object)views_sorted.GetByIndex(i);
                        if (( tempViewObject.View_Type != View_Enum.HTML ) || ( tempViewObject.Label != "Audio Clips" ) || ( tempViewObject.Attributes != "UF12345678.htm" ))
                            package.SobekCM_Web.Add_View(tempViewObject);
                    }

                    // If there were no views, add JPEG and then JP2 as default
                    if (package.SobekCM_Web.Views.Count == 0)
                    {
                        package.SobekCM_Web.Add_View(View_Enum.JPEG);
                        package.SobekCM_Web.Add_View(View_Enum.JPEG2000);
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
                    while (r.Read())
                    {
                        // Is this the end of this behavior sec?
                        if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "METS:behaviorSec"))
                        {
                            interfaces_flag = false;
                            break;
                        }

                        // Is this an element node?  If so collect either the behavior id or the title
                        if (r.NodeType == XmlNodeType.Element)
                        {
                            // Is this a new behavior?
                            if ((r.Name.Trim() == "METS:behavior") && (r.HasAttributes) && (r.MoveToAttribute("ID")))
                            {
                                // Get the view id
                                interface_id = r.Value.ToUpper();
                            }

                            // Is this the new mechanism?
                            if ((r.Name.Trim() == "METS:mechanism") && (r.HasAttributes) && (r.MoveToAttribute("xlink:title")))
                            {
                                // Get the title of this behavior mechanism?
                                interface_title = r.Value.ToUpper();
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
                    package.SobekCM_Web.Clear_Web_Skins();
                    for (int i = 0; i < interfaces_sorted.Count; i++)
                    {
                        package.SobekCM_Web.Add_Web_Skin(interfaces_sorted.GetByIndex(i).ToString());
                    }
                }
            } while (r.Read());
        }

        #endregion
    }
}
