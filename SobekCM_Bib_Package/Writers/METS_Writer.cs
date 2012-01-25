using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.Writers.SubWriters;

namespace SobekCM.Bib_Package.Writers
{
    /// <summary> Enumeration defines the types of metadata sections to include in this METS wrapper </summary>
    public enum Metadata_Type_Enum
    {
        /// <summary> Dark Archiving in the Sunshine State (DAITSS), Dark Archive metadata section </summary>
        DAITSS,

        /// <summary> Darwin Core-compliant zoological taxonomy information metadata section </summary>
        DarwinCore,

        /// <summary> Dublin Core bibliographic metadata section </summary>
        DublinCore,

        /// <summary> MARC formatted as XML bibliographic section </summary>
        MarcXML,

        /// <summary> MODS ( Metadata Object Description Standard ) bibliographic metadata section </summary>
        MODS,

        /// <summary> SobekkCM Custom Behaviors metadata section </summary>
        SobekCM_Behaviors,

        /// <summary> SobekCM Custom BibDesc bibliographic metadata section </summary>
        SobekCM_BibDesc,

        /// <summary> SobekCM Custom File Section administrative section </summary>
        SobekCM_FileSpecs,

        /// <summary> SobekCM Custom Map bibliographic metadata section </summary>
        SobekCM_Map,

        /// <summary> SobekCM Custom Processing Parameters (bibliographic) metadata section </summary>
        SobekCM_ProcParam
    }

    /// <summary> Class writes a METS file with any of the included metadata schemes in 
    /// the bibliographic and administrative sections </summary>
    public class METS_Writer
    {


        /// <summary> Constructor for a new instance of METS_Writer class </summary>
        public METS_Writer()
        {
            
        }


        /// <summary> Helper method to create a simple METS line </summary>
        /// <param name="indent">Indent for this line</param>
        /// <param name="mets_tag">Tag for this XML value</param>
        /// <param name="mets_value">Inner text for this XML value</param>
        /// <returns>Built simple METS string </returns>
        protected static string toMETS(string indent, string mets_tag, string mets_value)
        {
            if (mets_value.Length > 0)
            {
                return indent + "<" + mets_tag + ">" + mets_value + "</" + mets_tag + ">\r\n";
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        /// <param name="embedded_metadata_types"> Embedded metadata types to include in this METS wrapper </param>
        /// <param name="exclude_files">Flag indicates whether to completely exclude files from the resulting metadata file</param>
        public static void Save( SobekCM_Item thisBib, List<Metadata_Type_Enum> embedded_metadata_types, bool exclude_files)
        {
            string file_name = thisBib.Source_Directory + "\\" + thisBib.BibID + "_" + thisBib.VID + ".mets";
            if ((thisBib.VID == "*****") || (thisBib.METS.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL))
                file_name = thisBib.Source_Directory + "\\" + thisBib.BibID + ".mets";
            if (thisBib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project )
                file_name = thisBib.Source_Directory + "\\" + thisBib.BibID + ".pmets";

            StreamWriter results = new StreamWriter(file_name, false);
            Save(results, thisBib, embedded_metadata_types, exclude_files);
            results.Flush();
            results.Close();
        }

        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        /// <param name="File_Name">Name for this mets file</param>
        /// <param name="embedded_metadata_types"> Embedded metadata types to include in this METS wrapper </param>
        /// <param name="exclude_files">Flag indicates whether to completely exclude files from the resulting metadata file</param>
        public static void Save(SobekCM_Item thisBib, string File_Name, List<Metadata_Type_Enum> embedded_metadata_types, bool exclude_files)
        {
            StreamWriter results = new StreamWriter(File_Name, false);
            Save(results, thisBib, embedded_metadata_types, exclude_files, new List<string>());
            results.Flush();
            results.Close();
        }

        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        /// <param name="embedded_metadata_types"> Embedded metadata types to include in this METS wrapper </param>
        /// <param name="exclude_files">Flag indicates whether to completely exclude files from the resulting metadata file</param>
        /// <param name="Output">Output stream to which to write the METS for this item </param>
        public static void Save(System.IO.TextWriter Output, SobekCM_Item thisBib, List<Metadata_Type_Enum> embedded_metadata_types, bool exclude_files)
        {
            Save(Output, thisBib, embedded_metadata_types, exclude_files, new List<string>());
        }

        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        /// <param name="embedded_metadata_types"> Embedded metadata types to include in this METS wrapper </param>
        /// <param name="exclude_files">Flag indicates whether to completely exclude files from the resulting metadata file</param>
        /// <param name="Output">Output stream to which to write the METS for this item </param>
        /// <param name="mimes_to_exclude"> Mime types to exclude from the resulting METS file </param>
        public static void Save(System.IO.TextWriter Output, SobekCM_Item thisBib, List<Metadata_Type_Enum> embedded_metadata_types, bool exclude_files, List<string> mimes_to_exclude )
        {
            // Ensure the METS ID is set from BibID and VID
            if (thisBib.METS.ObjectID.Length == 0)
            {
                thisBib.METS.ObjectID = thisBib.BibID + "_" + thisBib.VID;
                if (thisBib.VID.Length == 0)
                    thisBib.METS.ObjectID = thisBib.BibID;
            }

            // If this is bib level, it is different
            if (( thisBib.VID == "*****") || (thisBib.METS.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL))
                thisBib.METS.ObjectID = thisBib.BibID;

            // If this is juat a project XML, do this differently
            if (thisBib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project)
            {
                thisBib.Bib_Info.Main_Title.Title = "Project level metadata for '" + thisBib.BibID + "'";
                thisBib.METS.ObjectID = thisBib.BibID;
            }

            // Ensure the source code appears in the METS header correctly
            thisBib.METS.Creator_Organization = thisBib.Bib_Info.Source.Code;
            if ((thisBib.Bib_Info.Source.Statement.Length > 0) && ( String.Compare(thisBib.Bib_Info.Source.Statement, thisBib.Bib_Info.Source.Code, true ) != 0 ))
            {
                thisBib.METS.Creator_Organization = thisBib.Bib_Info.Source.Code + "," + thisBib.Bib_Info.Source.XML_Safe_Statement;
            }

            // Add the XML declaration
            Output.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>\r\n");
            Output.Flush();

            // If this will go to DAITSS, add the DAITSS line next
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.DAITSS))
            {
                if ((thisBib.hasDaittsInformation) && (thisBib.DAITSS.toArchive) && 
                    (thisBib.DAITSS.Project.Length > 0) && (thisBib.DAITSS.Account.Length > 0))
                {
                    Output.Write("<?fcla fda=\"yes\"?>\r\n");
                }
                else
                {
                    Output.Write("<?fcla fda=\"no\"?>\r\n");
                }
            }

            // Indicate this will not go to PALMM
            if (thisBib.hasPalmmInformation)
            {
                if (thisBib.PALMM.toPALMM)
                    Output.Write("<?fcla dl=\"yes\"?>\r\n");
                else
                    Output.Write("<?fcla dl=\"no\"?>\r\n");
            }

            // Add a remark here with the title and type
            Output.Write("<!--  " + thisBib.Bib_Info.Main_Title.Title_XML.Replace("-", " ") + " ( " + thisBib.Bib_Info.SobekCM_Type_String + " ) -->\r\n");

            // Add the METS declaration information and METS header
            thisBib.METS.Add_METS( thisBib, Output, embedded_metadata_types );

            // Prepare to add all the bibliographic section
            int bib_section_id_counter = 0;

            // Prepare list of any unanalyzed dmdSec ID's which happen to start with DMD as well
            List<int> unanalyzed_dmdsec_ids = new List<int>();
            if (thisBib.Unanalyzed_DMDSEC_Count > 0)
            {
                foreach (Unanalyzed_METS_Section thisSection in thisBib.Unanalyzed_DMDSECs)
                {
                    if ((thisSection.ID.Length > 3) && (thisSection.ID.IndexOf("DMD") == 0))
                    {
                        string possible_number = thisSection.ID.Substring(3);
                        bool isNumber = true;
                        foreach (char thisChar in possible_number)
                        {
                            if (!Char.IsNumber(thisChar))
                            {
                                isNumber = false;
                                break;
                            }
                        }
                        if (isNumber)
                        {
                            try
                            {
                                int id_to_avoid = Convert.ToInt32(possible_number);
                                unanalyzed_dmdsec_ids.Add(id_to_avoid);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            // Add the dublin core section?
            if ( embedded_metadata_types.Contains(Metadata_Type_Enum.DublinCore))
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do {  bib_section_id_counter++;   } 
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">"); 
                Output.WriteLine("<METS:mdWrap MDTYPE=\"DC\"  MIMETYPE=\"text/xml\" LABEL=\"Simple Dublin Core\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the simple dublin core 
                Dublin_Core_SubWriter.Add_Simple_Dublin_Core(Output, thisBib);

                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");
            }

            // Add the marc section?
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.MarcXML))
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do { bib_section_id_counter++; }
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">");
                Output.WriteLine("<METS:mdWrap MDTYPE=\"MARC\" MIMETYPE=\"text/xml\" LABEL=\"MarcXML Metadata\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the MarcXML section
                MarcXML_SubWriter.Add_MarcXML(Output, thisBib, true);

                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");

            }

            // Add the mods section?
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.MODS))
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do { bib_section_id_counter++; }
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">");
                Output.WriteLine("<METS:mdWrap MDTYPE=\"MODS\"  MIMETYPE=\"text/xml\" LABEL=\"MODS Metadata\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the MODS section
                MODS_SubWriter.Add_MODS(Output, thisBib);
                
                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");

            }

            // Add the sobekcm custom section ( either bib desc or processing parameters )?
            if (( embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_BibDesc)) || ( embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_ProcParam )))
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do { bib_section_id_counter++; }
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">");
                Output.WriteLine("<METS:mdWrap MDTYPE=\"OTHER\" OTHERMDTYPE=\"SobekCM\" MIMETYPE=\"text/xml\" LABEL=\"SobekCM Custom Metadata\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the custom SobekCM metadata section
                SobekCM_Metadata_SubWriter.Add_SobekCM_Metadata( Output, thisBib, embedded_metadata_types );

                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");
            }

            // Add the sobekcm map section?
            if (( embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_Map )) && ( thisBib.has_Map_Data ))
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do { bib_section_id_counter++; }
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">");
                Output.WriteLine("<METS:mdWrap MDTYPE=\"OTHER\" OTHERMDTYPE=\"SobekCM\" MIMETYPE=\"text/xml\" LABEL=\"SobekCM Custom Metadata\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the SobekCM map section
                SobekCM_Map_SubWriter.Add_SobekCM_Metadata( Output, thisBib );
                
                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");
            }

            // Add any darwin core information?
            if ( thisBib.hasZoologicalTaxonomy )
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do { bib_section_id_counter++; }
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">");
                Output.WriteLine("<METS:mdWrap MDTYPE=\"OTHER\" OTHERMDTYPE=\"DarwinCore\" MIMETYPE=\"text/xml\" LABEL=\"DarwinCore Species Information\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the simple dublin core 
                DarwinCore_SubWriter.Add_DarwinCore(Output, thisBib);

                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");
            }

            // Add any ETD information?
            if (thisBib.hasThesisDisserationInformation )
            {
                // Increment the bib section id counter, while being sure to avoid any that are 
                // assigned to an unanalyzed dmd section
                do { bib_section_id_counter++; }
                while (unanalyzed_dmdsec_ids.Contains(bib_section_id_counter));

                // Start this METS section
                Output.WriteLine("<METS:dmdSec ID=\"DMD" + bib_section_id_counter + "\">");
                Output.WriteLine("<METS:mdWrap MDTYPE=\"OTHER\" OTHERMDTYPE=\"PALMM\" MIMETYPE=\"text/xml\" LABEL=\"PALMM Extensions\">");
                Output.WriteLine("<METS:xmlData>");

                // Add the simple dublin core 
                Thesis_Dissertation_SubWriter.Add_Thesis_Dissertation_Metadata(Output, thisBib);

                // Close this METS section
                Output.WriteLine("</METS:xmlData>");
                Output.WriteLine("</METS:mdWrap>");
                Output.WriteLine("</METS:dmdSec>");
            }

            // Now, add any unanalyzed DMD sections
            if (thisBib.Unanalyzed_DMDSEC_Count > 0)
            {
                foreach (Unanalyzed_METS_Section thisSection in thisBib.Unanalyzed_DMDSECs)
                {
                    Output.Write("<METS:dmdSec");
                    foreach (KeyValuePair<string, string> attribute in thisSection.Section_Attributes)
                    {
                        Output.Write(" " + attribute.Key + "=\"" + attribute.Value + "\"");
                    }
                    Output.WriteLine(">");
                    Output.WriteLine(thisSection.Inner_XML);
                    Output.WriteLine("</METS:dmdSec>");
                }
            }

            //// Now, add any additional DMD Sections
            //// Get all the divisions
            //Package.Divisions.TreeNode_Collection allDivisions = thisBib.Divisions.Tree.Divisions_PreOrder;
            //foreach (Package.Divisions.abstract_TreeNode thisDivNode in allDivisions)
            //{
            //    if ( thisDivNode.Bibliographic_Data != null )
            //    {
            //        if ( thisDivNode.Bibliographic_Data.hasData )
            //        {
            //            thisDivNode.Bibliographic_Data.METS_MODS_ID = "DMD" + DMD;
            //            DMD++;
            //            thisDivNode.Bibliographic_Data.Add_MODS_in_METS("\t", false, results);
            //        }
            //        else
            //        {
            //            thisDivNode.Bibliographic_Data = null;
            //        }
            //    }
            //}

            // Add the DAITSS information if this is destined for daitts
            if (( embedded_metadata_types.Contains(Metadata_Type_Enum.DAITSS)) && ( thisBib.hasDaittsInformation))
            {
                DAITSS_SubWriter.Add_DAITSS( Output, thisBib );
            }

            // Now, add any unanalyzed AMD sections
            if (thisBib.Unanalyzed_AMDSEC_Count > 0)
            {
                foreach (Unanalyzed_METS_Section thisSection in thisBib.Unanalyzed_AMDSECs)
                {
                    Output.Write("<METS:amdSec");
                    foreach (KeyValuePair<string, string> attribute in thisSection.Section_Attributes)
                    {
                        Output.Write(" " + attribute.Key + "=\"" + attribute.Value + "\"");
                    }
                    Output.WriteLine(">");
                    Output.Write(thisSection.Inner_XML);
                    Output.WriteLine("</METS:amdSec>");
                }
            }

            // Add file section
            if ((!exclude_files) && ( thisBib.Divisions.Has_Files ))
            {
                // Build the linking information
                StringBuilder dmd_sec_builder = new StringBuilder();
                for (int i = 1; i <= bib_section_id_counter; i++)
                {
                    dmd_sec_builder.Append("DMD" + i + " ");
                }

                // Determine if the technical section should exist here
                bool include_sobekcm_custom_file_tech_specs = false;
                if (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_FileSpecs))
                    include_sobekcm_custom_file_tech_specs = true;

                // Add all the METS file/division portions
                thisBib.Divisions.Add_METS(Output, false, include_sobekcm_custom_file_tech_specs, thisBib.Bib_Info.Main_Title.Title_XML, dmd_sec_builder.ToString().Trim(), mimes_to_exclude);
            }
            else
            {
                // Structure map is a required element for METS
                Output.Write("<METS:structMap ID=\"STRUCT1\" > <METS:div /> </METS:structMap>\r\n");
            }

            // Add the behavior section for SobekCM views and interfaces
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_Behaviors))
            {
                SobekCM_Behaviors_SubWriter.Add_SobekCM_Behaviors(Output, thisBib);
            }

            // Close out the METS file
            Output.Write("</METS:mets>\r\n");

        }

        #region Specialized METS Types writing routines

        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        public static void Save_FCLA_METS(SobekCM_Item thisBib)
        {
            // Create the filename for this METS
            string FileName = thisBib.Source_Directory + "/" + thisBib.METS.ObjectID + ".xml";

            // Set some of the PALMM values, if not already set
            if (thisBib.hasPalmmInformation)
            {
                thisBib.PALMM.Set_Values(thisBib.Bib_Info.Type.MODS_Type_String);
                thisBib.METS.Clear_Creator_Org_Notes();
                thisBib.METS.Add_Creator_Org_Notes("server=" + thisBib.PALMM.PALMM_Server);
                thisBib.METS.Add_Creator_Org_Notes("projects=" + thisBib.PALMM.PALMM_Project.ToUpper());
            }

            // Write to a string builder first, then we'll dump to the file
            StreamWriter results = new StreamWriter(FileName, false);

            // Create the list of metadata types to write
            List<Metadata_Type_Enum> embedded_metadata_types = new List<Metadata_Type_Enum>();
            embedded_metadata_types.Add(Metadata_Type_Enum.MODS);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_BibDesc);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_FileSpecs);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_Map);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_ProcParam);
            embedded_metadata_types.Add(Metadata_Type_Enum.DAITSS);
            embedded_metadata_types.Add(Metadata_Type_Enum.DarwinCore);
            METS_Writer.Save(results, thisBib, embedded_metadata_types, false);

            // Now flush and close the connection to the file
            results.Flush();
            results.Close();
        }


        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        /// <param name="Destination_File">Filename for the output file</param>
        /// <param name="exclude_files">Flag indicates whether to completely exclude files from the resulting metadata file</param>
        /// <param name="include_daitss_flags">Flag indicates whether to include the DAITSS ( Florida Digital Archive ) information </param>
        public static void Save_SobekCM_MODS_METS(string Destination_File, SobekCM_Item thisBib, bool exclude_files, bool include_daitss_flags)
        {
            // Create the filename to save
            if ((thisBib.BibID.Length == 0) || ((thisBib.VID.Length == 0) && (thisBib.Bib_Info.SobekCM_Type != TypeOfResource_SobekCM_Enum.Project )))
            {
                throw new ApplicationException("Error caught while saving a METS file.\n\nCan not create METS file without BibID or VID.");
            }

            // Remove the first slash
            if (Destination_File[0] == '/')
            {
                Destination_File = Destination_File.Substring(1);
            }

            // Write to a string builder first, then we'll dump to the file
            StreamWriter results = new StreamWriter(Destination_File, false);

            // Create the list of metadata types to write
            List<Metadata_Type_Enum> embedded_metadata_types = new List<Metadata_Type_Enum>();
            embedded_metadata_types.Add(Metadata_Type_Enum.MODS);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_BibDesc);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_FileSpecs);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_Map);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_ProcParam);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_Behaviors);
            embedded_metadata_types.Add(Metadata_Type_Enum.DarwinCore);
            METS_Writer.Save(results, thisBib, embedded_metadata_types, exclude_files);

            // Now flush and close the connection to the file
            results.Flush();
            results.Close();
        }

        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        /// <param name="exclude_files">Flag indicates whether to completely exclude files from the resulting metadata file</param>
        /// <param name="include_daitss_flags">Flag indicates whether to include the DAITSS ( Florida Digital Archive ) information </param>
        /// <param name="indent">Indent to use while formatting the metadata output</param>
        /// <param name="Output">Output stream to which to write the METS for this item </param>
        public static void Save_SobekCM_MODS_METS(System.IO.TextWriter Output, SobekCM_Item thisBib, bool exclude_files, bool include_daitss_flags, string indent)
        {
            // Create the list of metadata types to write
            List<Metadata_Type_Enum> embedded_metadata_types = new List<Metadata_Type_Enum>();
            embedded_metadata_types.Add(Metadata_Type_Enum.MODS);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_BibDesc);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_FileSpecs);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_Map);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_ProcParam);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_Behaviors);
            embedded_metadata_types.Add(Metadata_Type_Enum.DarwinCore);
            METS_Writer.Save(Output, thisBib, embedded_metadata_types, exclude_files);
        }


        /// <summary> Saves this bibliographic resource in METS using MODS as the main bibliographic describors </summary>
        /// <param name="thisBib">Bibliographic Package to save in METS/MODS form</param>
        public static void Save_SobekCM_Service_METS(SobekCM_Item thisBib)
        {
            if ((thisBib.METS.ObjectID.Length == 0) || (( thisBib.BibID.Length > 0 ) && ( thisBib.VID.Length > 0 )))
                thisBib.METS.ObjectID = thisBib.BibID + "_" + thisBib.VID;

            // Clear the checksums and remove files not needed
            List<string> mimes_to_exclude = new List<string>();
            mimes_to_exclude.Add("text/plain");
            mimes_to_exclude.Add("image/tiff");
            mimes_to_exclude.Add("text/x-pro");

            // Create the filename to save
            if ((thisBib.BibID.Length == 0) || ((thisBib.VID.Length == 0) && (thisBib.Bib_Info.SobekCM_Type != TypeOfResource_SobekCM_Enum.Project )))
            {
                throw new ApplicationException("Error caught while saving a METS file.\n\nCan not create METS file without BibID or VID.");
            }

            // Create the filename for this METS
            string FileName = thisBib.Source_Directory + "/" + thisBib.METS.ObjectID + ".mets.xml";

            // If this is juat a project XML, do this differently
            if (thisBib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project )
            {
                thisBib.Bib_Info.Main_Title.Title = "Project level metadata for '" + thisBib.BibID + "'";
                FileName = thisBib.Source_Directory + "/" + thisBib.BibID + ".pmets";
                thisBib.METS.ObjectID = thisBib.BibID;
            }

            // Remove the first slash
            if (FileName[0] == '/')
            {
                FileName = FileName.Substring(1);
            }

            // Create a stream to the file
            StreamWriter results = new StreamWriter(FileName, false);

            // Create the list of metadata types to write
            List<Metadata_Type_Enum> embedded_metadata_types = new List<Metadata_Type_Enum>();
            embedded_metadata_types.Add(Metadata_Type_Enum.MODS);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_BibDesc);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_FileSpecs);
            embedded_metadata_types.Add(Metadata_Type_Enum.SobekCM_Map);
            embedded_metadata_types.Add(Metadata_Type_Enum.DarwinCore);
            METS_Writer.Save(results, thisBib, embedded_metadata_types, false, mimes_to_exclude);

            // Now flush and close the connection to the file
            results.Flush();
            results.Close();
        }

        #endregion
    }
}
