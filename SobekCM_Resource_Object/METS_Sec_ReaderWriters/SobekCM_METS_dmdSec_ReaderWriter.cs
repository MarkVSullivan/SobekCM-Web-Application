#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> Reader that operates against a single custom SobekCM descriptive METS section  </summary>
    public class SobekCM_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        private readonly string sobekcm_namespace;


        /// <summary> Constructor for a new instance of the SobekCM_METS_dmdSec_ReaderWriter class </summary>
        /// <param name="SobekCM_Namespace">Namespace utilized for this material in the XML file being read</param>
        public SobekCM_METS_dmdSec_ReaderWriter(string SobekCM_Namespace)
        {
            sobekcm_namespace = SobekCM_Namespace;
        }

        /// <summary> Constructor for a new instance of the SobekCM_METS_dmdSec_ReaderWriter class </summary>
        public SobekCM_METS_dmdSec_ReaderWriter()
        {
            sobekcm_namespace = "sobekcm";
        }

        #region Methods to write the SobekCM dmdSec

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Add the processing parameters first
            METS_Item.Behaviors.Add_METS_Processing_Metadata(sobekcm_namespace, Output_Stream);

            // Add the bibliographic description portion
            METS_Item.Bib_Info.Add_SobekCM_BibDesc(Output_Stream);

            // If there is an embedded video write it here
            if (!String.IsNullOrEmpty(METS_Item.Behaviors.Embedded_Video))
            {
                Output_Stream.WriteLine("<sobekcm:embeddedVideo>" + Convert_String_To_XML_Safe(METS_Item.Behaviors.Embedded_Video) + "</sobekcm:embeddedVideo>");
            }

            // Add the serial information, if there is some
            if ((METS_Item.Behaviors.hasSerialInformation) && (METS_Item.Behaviors.Serial_Info.Count > 0) && (METS_Item.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL))
            {
                METS_Item.Behaviors.Serial_Info.Add_METS(sobekcm_namespace, Output_Stream);
            }

            // Add the oral history section, if there is any data+
            Oral_Interview_Info oralInfo = METS_Item.Get_Metadata_Module("OralInterview") as Oral_Interview_Info;
            if ((oralInfo != null) && (oralInfo.hasData))
            {
                // Start the Administrative section
                Output_Stream.WriteLine("<oral:interview>");

                // Add all the custom SobekCM specific data
                if (oralInfo.Interviewee.Length > 0)
                    Output_Stream.WriteLine("<oral:Interviewee>" + Convert_String_To_XML_Safe(oralInfo.Interviewee) + "</oral:Interviewee>");
                if (oralInfo.Interviewer.Length > 0)
                    Output_Stream.WriteLine("<oral:Interviewer>" + Convert_String_To_XML_Safe(oralInfo.Interviewer) + "</oral:Interviewer>");
                if (oralInfo.Interview_Date.Length > 0)
                    Output_Stream.WriteLine("<oral:InterviewDate>" + Convert_String_To_XML_Safe(oralInfo.Interview_Date) + "</oral:InterviewDate>");

                // End the Administrative section
                Output_Stream.WriteLine("</oral:interview>");
            }

            // Add the performing arts section, if there is any data
            Performing_Arts_Info partInfo = METS_Item.Get_Metadata_Module("PerformingArts") as Performing_Arts_Info;
            if ((partInfo != null) && (partInfo.hasData))
            {
                // Start this section
                Output_Stream.WriteLine("<part:performingArts>");

                // Add all the performer data
                if (partInfo.Performers_Count > 0)
                {
                    foreach (Performer thisPerformer in partInfo.Performers)
                    {
                        Output_Stream.Write("<part:Performer");
                        if (thisPerformer.LifeSpan.Length > 0)
                            Output_Stream.Write(" lifespan=\"" + Convert_String_To_XML_Safe(thisPerformer.LifeSpan) + "\"");
                        if (thisPerformer.Title.Length > 0)
                            Output_Stream.Write(" title=\"" + Convert_String_To_XML_Safe(thisPerformer.Title) + "\"");
                        if (thisPerformer.Occupation.Length > 0)
                            Output_Stream.Write(" occupation=\"" + Convert_String_To_XML_Safe(thisPerformer.Occupation) + "\"");
                        if (thisPerformer.Sex.Length > 0)
                            Output_Stream.Write(" sex=\"" + thisPerformer.Sex + "\"");
                        Output_Stream.WriteLine(">" + Convert_String_To_XML_Safe(thisPerformer.Name) + "</part:Performer>");
                    }
                }

                // Add the performance information
                if ((!String.IsNullOrEmpty(partInfo.Performance)) || (!String.IsNullOrEmpty(partInfo.Performance_Date)))
                {
                    string performanceName = Convert_String_To_XML_Safe(partInfo.Performance);
                    if (performanceName.Length == 0)
                        performanceName = "Unknown";
                    Output_Stream.Write("<part:Performance");
                    if (!String.IsNullOrEmpty(partInfo.Performance_Date))
                        Output_Stream.Write(" date=\"" + Convert_String_To_XML_Safe(partInfo.Performance_Date) + "\"");
                    Output_Stream.WriteLine(">" + performanceName + "</part:Performance>");
                }

                //// End this section
                Output_Stream.WriteLine("</part:performingArts>");
            }

            return true;
        }

        /// <summary> Helper method is used to create the METS from each individual data element </summary>
        /// <param name="mets_tag">Tag to use in the XML definition for this line</param>
        /// <param name="mets_value">Value to include in the METS tags</param>
        /// <returns>METS-compliant XML for this data</returns>
        internal static string toMETS(string mets_tag, string mets_value)
        {
            if (!String.IsNullOrEmpty(mets_value))
            {
                return "<" + mets_tag + ">" + mets_value + "</" + mets_tag + ">";
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region Methods to read the SobekCM dmdSec

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>  
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            // Get the related metadata modules here
            Performing_Arts_Info partInfo = Return_Package.Get_Metadata_Module("PerformingArts") as Performing_Arts_Info;
            Oral_Interview_Info oralInfo = Return_Package.Get_Metadata_Module("OralInterview") as Oral_Interview_Info;

            // Was a list for the deprecated download portion of the SobekCM section input?
            List<Download_Info_DEPRECATED> deprecatedDownloads = null;
            if (Options != null)
            {
                if (Options.ContainsKey("SobekCM_METS_dmdSec_ReaderWriter:Deprecated_Downloads"))
                {
                    deprecatedDownloads = (List<Download_Info_DEPRECATED>) Options["SobekCM_METS_dmdSec_ReaderWriter:Deprecated_Downloads"];
                }
            }

            while (Input_XmlReader.Read())
            {
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && ((Input_XmlReader.Name == "METS:mdWrap") || (Input_XmlReader.Name == "mdWrap")))
                    return true;

                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (Input_XmlReader.Name.Replace(sobekcm_namespace + ":", ""))
                    {
                        case "embeddedVideo":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                string code = Input_XmlReader.Value;
                                Return_Package.Behaviors.Embedded_Video = Input_XmlReader.Value;
                            }
                            break;

                        case "Collection.Primary":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                string code = Input_XmlReader.Value;
                                Return_Package.Behaviors.Add_Aggregation(code);
                            }
                            break;

                        case "Collection.Alternate":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                string code = Input_XmlReader.Value;
                                Return_Package.Behaviors.Add_Aggregation(code);
                            }
                            break;

                        case "SubCollection":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Behaviors.Add_Aggregation(Input_XmlReader.Value);
                            break;

                        case "Aggregation":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Behaviors.Add_Aggregation(Input_XmlReader.Value);
                            break;

                        case "MainPage":
                            if (Input_XmlReader.MoveToAttribute("pagename"))
                                Return_Package.Behaviors.Main_Page.PageName = Input_XmlReader.Value;
                            if (Input_XmlReader.MoveToAttribute("previous"))
                            {
                                try
                                {
                                    Return_Package.Behaviors.Main_Page.Previous_Page_Exists = Convert.ToBoolean(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }
                            if (Input_XmlReader.MoveToAttribute("next"))
                            {
                                try
                                {
                                    Return_Package.Behaviors.Main_Page.Next_Page_Exists = Convert.ToBoolean(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Behaviors.Main_Page.FileName = Input_XmlReader.Value;
                            break;

                        case "MainThumbnail":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Behaviors.Main_Thumbnail = Input_XmlReader.Value;
                            break;

                        case "EncodingLevel":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Bib_Info.EncodingLevel = Input_XmlReader.Value;
                            break;

                        case "Icon":
                        case "Wordmark":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Behaviors.Add_Wordmark(Input_XmlReader.Value);
                            break;

                        case "Download":
                            Download_Info_DEPRECATED newDownload = new Download_Info_DEPRECATED();
                            if (Input_XmlReader.MoveToAttribute("label"))
                                newDownload.Label = Input_XmlReader.Value;
                            while (Input_XmlReader.Read())
                            {
                                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                                    break;

                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Download"))
                                    break;
                            }
                            switch (Input_XmlReader.Name.Replace(sobekcm_namespace + ":", ""))
                            {
                                case "name":
                                    Input_XmlReader.Read();
                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    {
                                        newDownload.FileName = Input_XmlReader.Value;
                                        if (deprecatedDownloads != null)
                                            deprecatedDownloads.Add(newDownload);
                                    }
                                    break;

                                case "url":
                                    if (Input_XmlReader.MoveToAttribute("type"))
                                        newDownload.FormatCode = Input_XmlReader.Value;
                                    if (Input_XmlReader.MoveToAttribute("size"))
                                    {
                                        try
                                        {
                                            newDownload.Size_MB = (float) Convert.ToDouble(Input_XmlReader.Value);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    Input_XmlReader.Read();
                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    {
                                        newDownload.URL = Input_XmlReader.Value;
                                        if (deprecatedDownloads != null)
                                            deprecatedDownloads.Add(newDownload);
                                    }
                                    break;

                                case "fptr":
                                    if (Input_XmlReader.MoveToAttribute("FILEID"))
                                    {
                                        newDownload.File_ID = Input_XmlReader.Value;
                                        if (deprecatedDownloads != null)
                                            deprecatedDownloads.Add(newDownload);
                                    }
                                    break;
                            }
                            break;

                        case "URL":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Bib_Info.Location.PURL = Input_XmlReader.Value;
                            break;

                        case "GUID":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Web.GUID = Input_XmlReader.Value;
                            break;

                        case "NotifyEmail":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                Return_Package.Behaviors.NotifyEmail = Input_XmlReader.Value;
                            }
                            break;

						case "VisibilityRestrictions":
		                    Input_XmlReader.Read();
							if (Input_XmlReader.NodeType == XmlNodeType.Text)
							{
								string restriction_text = Input_XmlReader.Value;
								short restriction;
								if (Int16.TryParse(restriction_text, out restriction))
								{
									Return_Package.Behaviors.IP_Restriction_Membership = restriction;
								}
							}
		                    break;

                        case "Tickler":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                Return_Package.Behaviors.Add_Tickler(Input_XmlReader.Value);
                            }
                            break;

                        case "BibID":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Bib_Info.BibID = Input_XmlReader.Value;
                            break;

                        case "VID":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                Return_Package.Bib_Info.VID = Input_XmlReader.Value;
                            break;

                        case "Affiliation":
                            Affiliation_Info newAffiliation = new Affiliation_Info();
                            if (Input_XmlReader.MoveToAttribute("nameid"))
                                newAffiliation.Name_Reference = Input_XmlReader.Value;
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Affiliation"))
                                {
                                    if (newAffiliation.hasData)
                                    {
                                        Return_Package.Bib_Info.Add_Affiliation(newAffiliation);
                                        break;
                                    }
                                }

                                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                                {
                                    switch (Input_XmlReader.Name.Replace(sobekcm_namespace + ":", ""))
                                    {
                                        case "AffiliationTerm":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Term = Input_XmlReader.Value;
                                            break;

                                        case "University":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.University = Input_XmlReader.Value;
                                            break;

                                        case "Campus":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Campus = Input_XmlReader.Value;
                                            break;

                                        case "College":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.College = Input_XmlReader.Value;
                                            break;

                                        case "Unit":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Unit = Input_XmlReader.Value;
                                            break;

                                        case "Department":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Department = Input_XmlReader.Value;
                                            break;

                                        case "Institute":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Institute = Input_XmlReader.Value;
                                            break;

                                        case "Center":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Center = Input_XmlReader.Value;
                                            break;

                                        case "Section":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.Section = Input_XmlReader.Value;
                                            break;

                                        case "SubSection":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                newAffiliation.SubSection = Input_XmlReader.Value;
                                            break;
                                    }
                                }
                            }
                            break;

                        case "Coordinates":
                            read_coordinates_info(Input_XmlReader, Return_Package);
                            break;

                        case "Holding":
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Holding"))
                                    break;

                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (Input_XmlReader.Name == sobekcm_namespace + ":statement"))
                                {
                                    if (Input_XmlReader.MoveToAttribute("code"))
                                        Return_Package.Bib_Info.Location.Holding_Code = Input_XmlReader.Value;
                                    Input_XmlReader.Read();
                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    {
                                        Return_Package.Bib_Info.Location.Holding_Name = Input_XmlReader.Value;
                                        break;
                                    }
                                }
                            }
                            break;

                        case "Source":
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Source"))
                                    break;

                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (Input_XmlReader.Name == sobekcm_namespace + ":statement"))
                                {
                                    if (Input_XmlReader.MoveToAttribute("code"))
                                        Return_Package.Bib_Info.Source.Code = Input_XmlReader.Value;
                                    Input_XmlReader.Read();
                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    {
                                        Return_Package.Bib_Info.Source.Statement = Input_XmlReader.Value;
                                        break;
                                    }
                                }
                            }
                            break;

                        case "Temporal":
                            while (Input_XmlReader.Read())
                            {
								if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Temporal"))
                                {
									break;
                                }

                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (Input_XmlReader.Name == sobekcm_namespace + ":period"))
                                {
									Temporal_Info newTemporal = new Temporal_Info();
									if (Input_XmlReader.MoveToAttribute("start"))
                                    {
	                                    int temp_start_year;
	                                    if (Int32.TryParse(Input_XmlReader.Value, out temp_start_year))
		                                    newTemporal.Start_Year = temp_start_year;
                                    }

                                    if (Input_XmlReader.MoveToAttribute("end"))
                                    {
										int temp_end_year;
										if (Int32.TryParse(Input_XmlReader.Value, out temp_end_year))
											newTemporal.End_Year = temp_end_year;
                                    }

                                    Input_XmlReader.Read();
                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    {
                                        newTemporal.TimePeriod = Input_XmlReader.Value;
                                    }

									if (( newTemporal.Start_Year > 0 ) || ( newTemporal.End_Year > 0 ) || ( newTemporal.TimePeriod.Length > 0 ))
										Return_Package.Bib_Info.Add_Temporal_Subject(newTemporal);
                                }
                            }
                            break;

                        case "Type":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                Return_Package.Bib_Info.SobekCM_Type_String = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "SortDate":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                try
                                {
                                    Return_Package.Bib_Info.SortDate = Convert.ToInt32(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }
                            break;

                        case "SortTitle":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                Return_Package.Bib_Info.SortTitle = Input_XmlReader.Value;
                            }
                            break;

                        case "Manufacturer":
                            Publisher_Info thisManufacturer = null;
                            string manufacturer_id = String.Empty;
                            if (Input_XmlReader.MoveToAttribute("ID"))
                                manufacturer_id = Input_XmlReader.Value;
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Manufacturer"))
                                {
                                    if (thisManufacturer != null)
                                    {
                                        thisManufacturer.ID = manufacturer_id;
                                    }
                                    break;
                                }

                                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                                {
                                    switch (Input_XmlReader.Name.Replace(sobekcm_namespace + ":", ""))
                                    {
                                        case "Name":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                            {
                                                thisManufacturer = Return_Package.Bib_Info.Add_Manufacturer(Input_XmlReader.Value);
                                            }
                                            break;

                                        case "PlaceTerm":
                                            if (thisManufacturer != null)
                                            {
                                                if ((Input_XmlReader.MoveToAttribute("type")) && (Input_XmlReader.Value == "code"))
                                                {
                                                    if (Input_XmlReader.MoveToAttribute("authority"))
                                                    {
                                                        switch (Input_XmlReader.Value)
                                                        {
                                                            case "marccountry":
                                                                Input_XmlReader.Read();
                                                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisManufacturer.Add_Place(String.Empty, Input_XmlReader.Value, String.Empty);
                                                                }
                                                                break;

                                                            case "iso3166":
                                                                Input_XmlReader.Read();
                                                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisManufacturer.Add_Place(String.Empty, String.Empty, Input_XmlReader.Value);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Input_XmlReader.Read();
                                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                    {
                                                        thisManufacturer.Add_Place(Input_XmlReader.Value);
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case "Publisher":
                            Publisher_Info thisPublisher = null;
                            string publisher_id = String.Empty;
                            if (Input_XmlReader.MoveToAttribute("ID"))
                                publisher_id = Input_XmlReader.Value;
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Publisher"))
                                {
                                    if (thisPublisher != null)
                                    {
                                        thisPublisher.ID = publisher_id;
                                    }
                                    break;
                                }

                                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                                {
                                    switch (Input_XmlReader.Name.Replace(sobekcm_namespace + ":", ""))
                                    {
                                        case "Name":
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                            {
                                                thisPublisher = Return_Package.Bib_Info.Add_Publisher(Input_XmlReader.Value);
                                            }
                                            break;

                                        case "PlaceTerm":
                                            if (thisPublisher != null)
                                            {
                                                if ((Input_XmlReader.MoveToAttribute("type")) && (Input_XmlReader.Value == "code"))
                                                {
                                                    if (Input_XmlReader.MoveToAttribute("authority"))
                                                    {
                                                        switch (Input_XmlReader.Value)
                                                        {
                                                            case "marccountry":
                                                                Input_XmlReader.Read();
                                                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisPublisher.Add_Place(String.Empty, Input_XmlReader.Value, String.Empty);
                                                                }
                                                                break;

                                                            case "iso3166":
                                                                Input_XmlReader.Read();
                                                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisPublisher.Add_Place(String.Empty, String.Empty, Input_XmlReader.Value);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Input_XmlReader.Read();
                                                    if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                    {
                                                        thisPublisher.Add_Place(Input_XmlReader.Value);
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;


                        case "part:performingArts":
                            if (partInfo == null)
                            {
                                partInfo = new Performing_Arts_Info();
                                Return_Package.Add_Metadata_Module("PerformingArts", partInfo);
                            }
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == "part:performingArts"))
                                    break;

                                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                                {
                                    switch (Input_XmlReader.Name)
                                    {
                                        case "part:Performer":
                                            string occupation = String.Empty;
                                            string lifespan = String.Empty;
                                            string title = String.Empty;
                                            string sex = String.Empty;
                                            if (Input_XmlReader.MoveToAttribute("occupation"))
                                                occupation = Input_XmlReader.Value;
                                            if (Input_XmlReader.MoveToAttribute("lifespan"))
                                                lifespan = Input_XmlReader.Value;
                                            if (Input_XmlReader.MoveToAttribute("title"))
                                                title = Input_XmlReader.Value;
                                            if (Input_XmlReader.MoveToAttribute("sex"))
                                                sex = Input_XmlReader.Value;
                                            Input_XmlReader.Read();
                                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                            {
                                                Performer newPerformer = partInfo.Add_Performer(Input_XmlReader.Value);
                                                newPerformer.Occupation = occupation;
                                                newPerformer.Title = title;
                                                newPerformer.Sex = sex;
                                                newPerformer.LifeSpan = lifespan;
                                            }
                                            break;

                                        case "part:Performance":
                                            if (Input_XmlReader.MoveToAttribute("date"))
                                                partInfo.Performance_Date = Input_XmlReader.Value;
                                            Input_XmlReader.MoveToElement();
                                            if (!Input_XmlReader.IsEmptyElement)
                                            {
                                                Input_XmlReader.Read();
                                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                                {
                                                    partInfo.Performance = Input_XmlReader.Value;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case "SerialHierarchy":
                            int serial_level = -1;
                            int serial_order = -1;
                            string serial_display = String.Empty;
                            if (Input_XmlReader.MoveToAttribute("level"))
                            {
                                try
                                {
                                    serial_level = Convert.ToInt32(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }
                            if (Input_XmlReader.MoveToAttribute("order"))
                            {
                                try
                                {
                                    serial_order = Convert.ToInt32(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }
                            Input_XmlReader.MoveToElement();
                            if (!Input_XmlReader.IsEmptyElement)
                            {
                                Input_XmlReader.Read();
                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    serial_display = Input_XmlReader.Value;
                            }
                            if ((serial_display.Length == 0) && (serial_order > 0))
                                serial_display = serial_order.ToString();
                            if ((serial_display.Length > 0) && (serial_order > 0))
                                Return_Package.Behaviors.Serial_Info.Add_Hierarchy(serial_level, serial_order, serial_display);
                            break;

                        case "Container":
                            int container_level = -1;
                            string container_type = String.Empty;
                            string container_name = String.Empty;
                            if (Input_XmlReader.MoveToAttribute("level"))
                            {
                                try
                                {
                                    container_level = Convert.ToInt32(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }
                            if (Input_XmlReader.MoveToAttribute("type"))
                            {
                                container_type = Input_XmlReader.Value;
                            }
                            Input_XmlReader.MoveToElement();
                            if (!Input_XmlReader.IsEmptyElement)
                            {
                                Input_XmlReader.Read();
                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                {
                                    container_name = Input_XmlReader.Value;

                                    Return_Package.Bib_Info.Add_Container(container_type, container_name, container_level);
                                }
                            }
                            break;

                        case "oral:Interviewee":
                            if (oralInfo == null)
                            {
                                oralInfo = new Oral_Interview_Info();
                                Return_Package.Add_Metadata_Module("OralInterview", oralInfo);
                            }
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                oralInfo.Interviewee = Input_XmlReader.Value;
                            }
                            break;

                        case "oral:Interviewer":
                            if (oralInfo == null)
                            {
                                oralInfo = new Oral_Interview_Info();
                                Return_Package.Add_Metadata_Module("OralInterview", oralInfo);
                            }
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                oralInfo.Interviewer = Input_XmlReader.Value;
                            }
                            break;
                    }
                }
            }

            // Return false since this read all the way to the end of the steam
            return false;
        }

        private void read_coordinates_info(XmlReader Input_XmlReader, SobekCM_Item Return_Package)
        {
            GeoSpatial_Information geoInfo = Return_Package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if (geoInfo == null)
            {
                geoInfo = new GeoSpatial_Information();
                Return_Package.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo);
            }

            while (Input_XmlReader.Read())
            {
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Coordinates"))
                {
                    return;
                }

                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (Input_XmlReader.Name.Replace(sobekcm_namespace + ":", ""))
                    {
                        case "KML":
                            if (!Input_XmlReader.IsEmptyElement)
                            {
                                Input_XmlReader.Read();
                                if (Input_XmlReader.NodeType == XmlNodeType.Text)
                                    geoInfo.KML_Reference = Input_XmlReader.Value;
                            }
                            break;
                        case "Point":
                            geoInfo.Add_Point(read_point(Input_XmlReader));
                            break;

                        case "Line":
                            Coordinate_Line newLine = new Coordinate_Line();
                            if (Input_XmlReader.MoveToAttribute("label"))
                                newLine.Label = Input_XmlReader.Value;
                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Line"))
                                {
                                    geoInfo.Add_Line(newLine);
                                    break;
                                }

                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (Input_XmlReader.Name == sobekcm_namespace + ":Point"))
                                {
                                    newLine.Add_Point(read_point(Input_XmlReader));
                                }
                            }
                            break;

                        case "Polygon":
                            Coordinate_Polygon newPolygon = new Coordinate_Polygon();
                            if (Input_XmlReader.MoveToAttribute("label"))
                                newPolygon.Label = Input_XmlReader.Value;
                            if (Input_XmlReader.MoveToAttribute("ID"))
                                newPolygon.ID = Input_XmlReader.Value;
                            if (Input_XmlReader.MoveToAttribute("pageSeq"))
                            {
                                try
                                {
                                    newPolygon.Page_Sequence = Convert.ToUInt16(Input_XmlReader.Value);
                                }
                                catch
                                {
                                }
                            }

                            while (Input_XmlReader.Read())
                            {
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Polygon"))
                                {
                                    geoInfo.Add_Polygon(newPolygon);
                                    break;
                                }

                                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                                {
                                    if (Input_XmlReader.Name == sobekcm_namespace + ":Edge")
                                    {
                                        while (Input_XmlReader.Read())
                                        {
                                            if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Edge"))
                                            {
                                                break;
                                            }

                                            if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (Input_XmlReader.Name == sobekcm_namespace + ":Point"))
                                            {
                                                newPolygon.Add_Edge_Point(read_point(Input_XmlReader));
                                            }
                                        }
                                    }

                                    if (Input_XmlReader.Name == sobekcm_namespace + ":Internal")
                                    {
                                        while (Input_XmlReader.Read())
                                        {
                                            if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (Input_XmlReader.Name == sobekcm_namespace + ":Internal"))
                                            {
                                                break;
                                            }

                                            if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (Input_XmlReader.Name == sobekcm_namespace + ":Point"))
                                            {
                                                newPolygon.Add_Inner_Point(read_point(Input_XmlReader));
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static Coordinate_Point read_point(XmlReader Input_XmlReader)
        {
            try
            {
                Coordinate_Point newPoint = new Coordinate_Point();
                if (Input_XmlReader.MoveToAttribute("latitude"))
                    newPoint.Latitude = Convert.ToDouble(Input_XmlReader.Value.Replace("°", ""));
                if (Input_XmlReader.MoveToAttribute("longitude"))
                    newPoint.Longitude = Convert.ToDouble(Input_XmlReader.Value.Replace("°", ""));
                if (Input_XmlReader.MoveToAttribute("altitude"))
                {
                    try
                    {
                        newPoint.Altitude = (long) Convert.ToDouble(Input_XmlReader.Value);
                    }
                    catch
                    {
                    }
                }
                if (Input_XmlReader.MoveToAttribute("order"))
                {
                    try
                    {
                        newPoint.Order_From_XML_Read = Convert.ToInt32(Input_XmlReader.Value);
                    }
                    catch
                    {
                    }
                }
                if (Input_XmlReader.MoveToAttribute("label"))
                    newPoint.Label = Input_XmlReader.Value;
                return newPoint;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return true;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            string sobekcm_ref = "sobekcm=\"http://sobekrepository.org/schemas/sobekcm/\"";
			string oral_ref = "oral=\"http://sobekrepository.org/schemas/sobekcm_oral/\"";
			string part_ref = "part=\"http://sobekrepository.org/schemas/sobekcm_part/\"";

            Performing_Arts_Info partInfo = METS_Item.Get_Metadata_Module("PerformingArts") as Performing_Arts_Info;
            Oral_Interview_Info oralInfo = METS_Item.Get_Metadata_Module("OralInterview") as Oral_Interview_Info;

            if (((oralInfo == null) || (!oralInfo.hasData)) && ((partInfo == null) || (!partInfo.hasData)))
                return new string[] {sobekcm_ref};

            if (((oralInfo != null) && (oralInfo.hasData)) && ((partInfo != null) && (partInfo.hasData)))
                return new string[] {sobekcm_ref, oral_ref, part_ref};

            if ((oralInfo != null) && (oralInfo.hasData))
            {
                return new string[] {sobekcm_ref, oral_ref};
            }

            if ((partInfo != null) && (partInfo.hasData))
            {
                return new string[] {sobekcm_ref, part_ref};
            }

            return new string[] {};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
			string sobekcm_ref = "    http://sobekrepository.org/schemas/sobekcm/\r\n    http://sobekrepository.org/schemas/sobekcm.xsd";
			string oral_ref = "    http://sobekrepository.org/schemas/sobekcm_oral/\r\n    http://sobekrepository.org/schemas/sobekcm_oral.xsd";
			string part_ref = "    http://sobekrepository.org/schemas/sobekcm_part/\r\n    http://sobekrepository.org/schemas/sobekcm_part.xsd";

            Performing_Arts_Info partInfo = METS_Item.Get_Metadata_Module("PerformingArts") as Performing_Arts_Info;
            Oral_Interview_Info oralInfo = METS_Item.Get_Metadata_Module("OralInterview") as Oral_Interview_Info;

            if (((oralInfo == null) || (!oralInfo.hasData)) && ((partInfo == null) || (!partInfo.hasData)))
                return new string[] {sobekcm_ref};

            if (((oralInfo != null) && (oralInfo.hasData)) && ((partInfo != null) && (partInfo.hasData)))
                return new string[] {sobekcm_ref, oral_ref, part_ref};

            if ((oralInfo != null) && (oralInfo.hasData))
            {
                return new string[] {sobekcm_ref, oral_ref};
            }

            if ((partInfo != null) && (partInfo.hasData))
            {
                return new string[] {sobekcm_ref, part_ref};
            }

            return new string[] {};
        }

        #endregion
    }
}