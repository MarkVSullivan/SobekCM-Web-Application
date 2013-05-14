#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

#endregion

namespace SobekCM.Resource_Object
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
    public class METS_Header_Info
    {
        private DateTime create_date;
        private string creator_individual;
        private List<string> creator_individual_notes;
        private string creator_org;
        private List<string> creator_org_notes;
        private string creator_software;
        private DateTime modify_date;
        private string objectID;
        private string recordStatus;
        private METS_Record_Status recordStatus_enum;

        /// <summary> Constructor creates a new instance of the METS_Header_Info class </summary>
        public METS_Header_Info()
        {
            // Set all to default
            recordStatus_enum = METS_Record_Status.COMPLETE;
            recordStatus = "COMPLETE";
            create_date = DateTime.Now;
            modify_date = DateTime.Now;
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

        /// <summary> Gets and sets the date this METS file was first created </summary>
        public DateTime Create_Date
        {
            get { return create_date; }
            set { create_date = value; }
        }

        /// <summary> Gets and sets the date this METS file was modified </summary>
        public DateTime Modify_Date
        {
            get { return modify_date; }
            set { modify_date = value; }
        }

        /// <summary> Gets or sets the SobekCM-controlled METS record status </summary>
        public METS_Record_Status RecordStatus_Enum
        {
            get { return recordStatus_enum; }
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
            set { creator_software = value; }
        }

        /// <summary> Gets or sets the individual who created this METS file </summary>
        public string Creator_Individual
        {
            get { return creator_individual ?? String.Empty; }
            set { creator_individual = value; }
        }

        /// <summary> Gets and sets the Object ID for this METS file </summary>
        /// <remarks>This is simply the BibID, followed by the VID (i.e., 'UF12345678_00001')</remarks>
        public string ObjectID
        {
            get { return objectID ?? String.Empty; }
            set { objectID = value; }
        }

        /// <summary> Clears the collection of notes listed in the METS header under the creator organization </summary>
        public void Clear_Creator_Org_Notes()
        {
            if (creator_org_notes != null)
                creator_org_notes.Clear();
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

        internal void Add_METS(SobekCM_Item thisBib, TextWriter results )
        {
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

        private string Date_to_METS(DateTime thisDate)
        {
            StringBuilder returnVal = new StringBuilder(thisDate.Year.ToString() + "-");
            if (thisDate.Month.ToString().Length == 2) returnVal.Append(thisDate.Month.ToString() + "-");
            else returnVal.Append("0" + thisDate.Month + "-");
            if (thisDate.Day.ToString().Length == 2) returnVal.Append(thisDate.Day);
            else returnVal.Append("0" + thisDate.Day);
            returnVal.Append("T");
            if (thisDate.Hour.ToString().Length == 2) returnVal.Append(thisDate.Hour + ":");
            else returnVal.Append("0" + thisDate.Hour + ":");
            if (thisDate.Minute.ToString().Length == 2) returnVal.Append(thisDate.Minute + ":");
            else returnVal.Append("0" + thisDate.Minute + ":");
            if (thisDate.Second.ToString().Length == 2) returnVal.Append(thisDate.Second);
            else returnVal.Append("0" + thisDate.Second);
            returnVal.Append("Z");
            return returnVal.ToString();
        }
    }
}