using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SobekCM.Bib_Package.Writers;

namespace SobekCM.Bib_Package
{
	/// <summary> Enumeration of the different possible METS record status </summary>
	public enum METS_Record_Status : byte
	{
        /// <summary> This record status indicated this is bibliographic-level METS file which is 
        /// derived from SobekCM single item METS files.  </summary>
        BIB_LEVEL = 0,

		/// <summary> This record status indicates the package is a complete package and instructs the
        /// SobekCM bulk loader to check for the existence of each file within the package. </summary>
		COMPLETE,

        /// <summary> This record status indicates this is a request to delete the pacakge </summary>
        DELETE,

        /// <summary> This record status indicates that this is a partial refresh of the digital resource
        /// in the SobekCM system.  This instructs the SobekCM bulk loader to skip checking for existence
        /// of each file listed in the METS during validation. </summary>
		PARTIAL,

		/// <summary> This record status indicates this is a metadata update only, and should not be
        /// accompanied with any additional files if loaded through the SobekCM bulk loader. </summary>
		METADATA_UPDATE,

        /// <summary> Some other record status value is used here, unrecognized by the SobekCM system.
        /// The unrecognized status is retained in another field. </summary>
        OTHER
	}

	/// <summary> Stores information specific to the creation of the METS format XML for this resource. This data is generally
    /// found within the METS header of a resulting METS file.  </summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
	public class METS_Info
	{
	
		private DateTime create_date, modify_date;
		private METS_Record_Status recordStatus_enum;


        private string objectID, creator_org, creator_software, creator_individual, recordStatus;
		private List<string> creator_org_notes;
        private List<string> creator_individual_notes;

		/// <summary> Constructor creates a new instance of the METS_Info class </summary>
		public METS_Info()
		{
			// Set all to default
            recordStatus_enum = METS_Record_Status.COMPLETE;
            recordStatus = "COMPLETE";
			create_date = DateTime.Now;
			modify_date = DateTime.Now;
		}

        /// <summary> Clears the collection of notes listed in the METS header under the creator organization </summary>
        public void Clear_Creator_Org_Notes()
        {
            if (creator_org_notes != null)
                creator_org_notes.Clear();
        }

        /// <summary> Gets the number of notes listed in the METS header under the creator organization </summary>
        public int Creator_Org_Notes_Count
        {
            get
            {
                if (creator_org_notes == null)
                    return 0;
                else
                    return creator_org_notes.Count;
            }
        }

        /// <summary> Gets the collection of notes listed in the METS header under the creator organization </summary>
        /// <remarks>This is used for PALMM materials</remarks>
        public ReadOnlyCollection<string> Creator_Org_Notes
        {
            get
            {
                if (creator_org_notes == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(creator_org_notes);
            }
        }

        /// <summary> Adds a note listed in the METS header under the creator organization </summary>
        /// <param name="New_Note">New creator organization note </param>
        public void Add_Creator_Org_Notes(string New_Note)
        {
            if (New_Note.Length == 0)
                return;

            if (creator_org_notes == null)
                creator_org_notes = new List<string>();

            if (!creator_org_notes.Contains(New_Note))
                creator_org_notes.Add(New_Note);
        }

        /// <summary> Replace the creator organizational notes </summary>
        /// <param name="Original_Note"> Note to be replaced </param>
        /// <param name="New_Note"> New note to use </param>
        public void Replace_Creator_Org_Notes(string Original_Note, string New_Note)
        {
            if (creator_org_notes == null)
                creator_org_notes = new List<string>();

            creator_org_notes.Remove(Original_Note);

            if (New_Note.Length > 0)
            {
                if (!creator_org_notes.Contains(New_Note))
                    creator_org_notes.Add(New_Note);
            }
        }

        /// <summary> Clears the collection of notes listed in the METS header under the individual creator </summary>
        public void Clear_Creator_Individual_Notes()
        {
            if (creator_individual_notes != null)
                creator_individual_notes.Clear();
        }

        /// <summary> Gets the number of notes listed in the METS header under the individual creator </summary>
        public int Creator_Individual_Notes_Count
        {
            get
            {
                if (creator_individual_notes == null)
                    return 0;
                else
                    return creator_individual_notes.Count;
            }
        }

        /// <summary> Gets the collection of notes listed in the METS header under the individual creator </summary>
        /// <remarks>This is used for PALMM materials</remarks>
        public ReadOnlyCollection<string> Creator_Individual_Notes
        {
            get
            {
                if (creator_individual_notes == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(creator_individual_notes);
            }
        }

        /// <summary> Adds a note listed in the METS header under the individual creator </summary>
        /// <param name="New_Note">New creator individual note </param>
        public void Add_Creator_Individual_Notes(string New_Note)
        {
            if (New_Note.Length == 0)
                return;

            if (creator_individual_notes == null)
                creator_individual_notes = new List<string>();

            if (!creator_individual_notes.Contains(New_Note))
                creator_individual_notes.Add(New_Note);
        }

		/// <summary> Gets and sets the date this METS file was first created </summary>
		public DateTime Create_Date
		{
			get		{		return create_date;			}
			set		{		create_date = value;		}
		}

		/// <summary> Gets and sets the date this METS file was modified </summary>
		public DateTime Modify_Date
		{
			get		{		return modify_date;			}
			set		{		modify_date = value;		}
		}

		/// <summary> Gets or sets the SobekCM-controlled METS record status </summary>
		public METS_Record_Status RecordStatus_Enum
		{
			get		{		return recordStatus_enum;		}
			set		
            {
                switch (value)
                {
                    case METS_Record_Status.BIB_LEVEL:
                        recordStatus = "BIB_LEVEL";
                        break;

                    case METS_Record_Status.PARTIAL:
                        recordStatus = "PARTIAL";
                        break;

                    case METS_Record_Status.COMPLETE:
                        recordStatus = "COMPLETE";
                        break;

                    case METS_Record_Status.DELETE:
                        recordStatus = "DELETE";
                        break;

                    case METS_Record_Status.METADATA_UPDATE:
                        recordStatus = "PARTIAL";
                        break;
                }
                recordStatus_enum = value;		
            }
		}

        /// <summary> Gets or sets the SobekCM-controlled METS record status </summary>
        public string RecordStatus
        {
            get { return recordStatus; }
            set 
            {
                string value_upper = value.ToUpper();
                switch (value_upper)
                {
                    case "COMPLETE":
                    case "NEW":
                    case "REPLACEMENT":
                        recordStatus_enum = METS_Record_Status.COMPLETE;
                        break;

                    case "METADATA_UPDATE":
                        recordStatus_enum = METS_Record_Status.METADATA_UPDATE;
                        break;

                    case "PARTIAL":
                        recordStatus_enum = METS_Record_Status.PARTIAL;
                        break;

                    case "BIB_LEVEL":
                        recordStatus_enum = METS_Record_Status.BIB_LEVEL;
                        break;

                    case "DELETE":
                        recordStatus_enum = METS_Record_Status.DELETE;
                        break;
                }
                recordStatus = value_upper;
            }
        }

        /// <summary> Gets or sets the organization which created this METS file </summary>
        public string Creator_Organization
        {
            get { return creator_org ?? String.Empty; }
            set { creator_org = value; }
        }

		/// <summary> Gets or sets the software which created this METS file </summary>
		public string Creator_Software
		{
            get { return creator_software ?? String.Empty; }
			set		{		creator_software = value;		}
		}

		/// <summary> Gets or sets the individual who created this METS file </summary>
		public string Creator_Individual
		{
            get { return creator_individual ?? String.Empty; }
			set		{		creator_individual = value;		}
		}

        /// <summary> Gets and sets the Object ID for this METS file </summary>
        /// <remarks>This is simply the BibID, followed by the VID (i.e., 'UF12345678_00001')</remarks>
        public string ObjectID
        {
            get { return objectID ?? String.Empty; }
            set { objectID = value; }
        }

        internal void Add_METS( SobekCM_Item thisBib, System.IO.TextWriter results, List<Metadata_Type_Enum> embedded_metadata_types )
        {
            ////For encoding Simple Dublin Core metadata within a METS document:
            ////The DCMI Simple DC XML Schema
            ////(http://www.dublincore.org/schemas/xmls/)
            ////http://www.dublincore.org/schemas/xmls/simpledc20020312.xsd


            ////For encoding MARC 21 metadata within a METS document:
            ////MARCXML MARC 21 XML Schema Implementation
            ////(http://www.loc.gov/standards/marcxml/)
            ////http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd


            // Add the METS declaration information
            results.Write("<METS:mets OBJID=\"" + ObjectID + "\"\r\n");
            results.Write("  xmlns:METS=\"http://www.loc.gov/METS/\"\r\n");
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.MODS))
            {
                results.Write("  xmlns:mods=\"http://www.loc.gov/mods/v3\"\r\n");
            }
            results.Write("  xmlns:xlink=\"http://www.w3.org/1999/xlink\"\r\n");
            results.Write("  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" \r\n");
            if (( embedded_metadata_types.Contains( Metadata_Type_Enum.DAITSS )) && ( thisBib.hasDaittsInformation))
            {
                results.Write("  xmlns:daitss=\"http://www.fcla.edu/dls/md/daitss/\"\r\n");
            }

            if ((embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_BibDesc)) || (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_FileSpecs)) || (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_ProcParam)))
            {
                results.Write("  xmlns:sobekcm=\"http://digital.uflib.ufl.edu/metadata/sobekcm/\"\r\n");

                if ( thisBib.hasOralHistoryInformation)
                {
                    results.Write("  xmlns:oral=\"http://digital.uflib.ufl.edu/metadata/oral/\"\r\n");
                }
                if (thisBib.hasPerformingArtsInformation)
                {
                    results.Write("  xmlns:part=\"http://digital.uflib.ufl.edu/metadata/part/\"\r\n");
                }
            }

            if (( embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_Map)) && (thisBib.has_Map_Data))
            {
                results.Write("  xmlns:map=\"http://digital.uflib.ufl.edu/metadata/ufdc_map/\"\r\n");
            }
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.DublinCore))
            {
                results.Write("  xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\r\n");
            }
            if (thisBib.hasZoologicalTaxonomy)
            {               
                results.Write("  xmlns:dwc=\"http://rs.tdwg.org/dwc/terms/\"\r\n");
                results.Write("  xmlns:dwr=\"http://rs.tdwg.org/dwc/xsd/simpledarwincore/\"\r\n");
            }
            if (thisBib.hasThesisDisserationInformation)
            {
                results.Write("  xmlns:palmm=\"http://www.fcla.edu/dls/md/palmm/\"\r\n");
            }

            results.Write("  xsi:schemaLocation=\"http://www.loc.gov/METS/\r\n");
            results.Write("    http://www.loc.gov/standards/mets/mets.xsd \r\n");

            if (embedded_metadata_types.Contains(Metadata_Type_Enum.MODS))
            {
                results.Write("    http://www.loc.gov/mods/v3\r\n");
                results.Write("    http://www.loc.gov/mods/v3/mods-3-4.xsd\r\n");
            }

            if ((embedded_metadata_types.Contains(Metadata_Type_Enum.DAITSS)) && (thisBib.hasDaittsInformation))
            {
                results.Write("    http://www.fcla.edu/dls/md/daitss/\r\n");
                results.Write("    http://www.fcla.edu/dls/md/daitss/daitss.xsd\r\n");
            }

            if ((embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_BibDesc)) || (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_FileSpecs)) || (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_ProcParam)))
            {
                results.Write("    http://digital.uflib.ufl.edu/metadata/sobekcm/\r\n");
                results.Write("    http://digital.uflib.ufl.edu/metadata/sobekcm/sobekcm.xsd\r\n");

                if (thisBib.hasOralHistoryInformation)
                {
                    results.Write("    http://digital.uflib.ufl.edu/metadata/oral/\r\n");
                    results.Write("    http://digital.uflib.ufl.edu/metadata/oral/oral.xsd\r\n");
                }
                if (thisBib.hasPerformingArtsInformation)
                {
                    results.Write("    http://digital.uflib.ufl.edu/metadata/part/\r\n");
                    results.Write("    http://digital.uflib.ufl.edu/metadata/part/part.xsd\r\n");
                }
            }
            if ((embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_Map)) && (thisBib.has_Map_Data))
            {
                results.Write("    http://digital.uflib.ufl.edu/metadata/ufdc_map/\r\n");
                results.Write("    http://digital.uflib.ufl.edu/metadata/ufdc_map/ufdc_map.xsd\r\n");
            }
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.DublinCore))
            {
                results.Write("    http://purl.org/dc/elements/1.1/\r\n");
                results.Write("    http://dublincore.org/schemas/xmls/simpledc20021212.xsd\r\n");
            }
            if (thisBib.hasZoologicalTaxonomy)
            {
                results.Write("    http://rs.tdwg.org/dwc/terms/\r\n");
                results.Write("    http://rs.tdwg.org/dwc/xsd/tdwg_dwcterms.xsd\r\n");
                results.Write("    http://rs.tdwg.org/dwc/xsd/simpledarwincore/\r\n");
                results.Write("    http://rs.tdwg.org/dwc/xsd/tdwg_dwc_simple.xsd\r\n");
            }
            if (thisBib.hasThesisDisserationInformation)
            {
                results.Write("    http://www.fcla.edu/dls/md/palmm/\r\n");
                results.Write("    http://www.fcla.edu/dls/md/palmm.xsd\r\n");
            }


            results.Write("\">\r\n");

            // Start the METS Header
            results.Write("<METS:metsHdr CREATEDATE=\"" + Date_to_METS(create_date) + "\" ID=\"" + ObjectID + "\" LASTMODDATE=\"" + Date_to_METS(modify_date) + "\" RECORDSTATUS=\"" + RecordStatus + "\">\r\n");

            // Add the organizational creator, if there is one
            if (!String.IsNullOrEmpty(creator_org))
            {
                results.Write("<METS:agent ROLE=\"CREATOR\" TYPE=\"ORGANIZATION\"> \r\n");
                results.Write("<METS:name>" + creator_org + "</METS:name>\r\n");
                if (creator_org_notes != null)
                {
                    foreach (string thisNote in creator_org_notes)
                    {
                        if (thisNote.Trim().Length > 0)
                        {
                            results.Write("<METS:note>" + thisNote + "</METS:note>\r\n");
                        }
                    }
                }
                results.Write("</METS:agent>\r\n");
            }

            // Add the software creator, if there is one
            if (!String.IsNullOrEmpty(creator_software))
            {
                results.Write("<METS:agent OTHERTYPE=\"SOFTWARE\" ROLE=\"CREATOR\" TYPE=\"OTHER\"> \r\n");
                results.Write("<METS:name>" + creator_software + "</METS:name>\r\n");
                results.Write("</METS:agent>\r\n");
            }

            // Add the organizational individual, if there is one
            if (!String.IsNullOrEmpty(creator_individual))
            {
                results.Write("<METS:agent ROLE=\"CREATOR\" TYPE=\"INDIVIDUAL\"> \r\n");
                results.Write("<METS:name>" + creator_individual + "</METS:name>\r\n");
                if (creator_individual_notes != null)
                {
                    foreach (string thisNote in creator_individual_notes)
                    {
                        results.Write("<METS:note>" + thisNote + "</METS:note>\r\n");
                    }
                }
                results.Write("</METS:agent>\r\n");
            }

            // Close out this section
            results.Write("</METS:metsHdr>\r\n");
        }

		private string Date_to_METS( DateTime thisDate )
		{
			StringBuilder returnVal = new StringBuilder(thisDate.Year.ToString() + "-" );
			if ( thisDate.Month.ToString().Length == 2 ) returnVal.Append( thisDate.Month.ToString() + "-" );
			else	returnVal.Append("0" + thisDate.Month + "-" );
			if ( thisDate.Day.ToString().Length == 2 ) returnVal.Append( thisDate.Day );
			else returnVal.Append("0" + thisDate.Day );
			returnVal.Append("T");
			if ( thisDate.Hour.ToString().Length == 2 ) returnVal.Append( thisDate.Hour + ":" );
			else returnVal.Append("0" + thisDate.Hour + ":" );
			if ( thisDate.Minute.ToString().Length == 2 ) returnVal.Append( thisDate.Minute + ":" );
			else returnVal.Append("0" + thisDate.Minute + ":" );
			if ( thisDate.Second.ToString().Length == 2 ) returnVal.Append( thisDate.Second );
			else returnVal.Append("0" + thisDate.Second);				
			returnVal.Append("Z");
			return returnVal.ToString();	
		}
	}
}
