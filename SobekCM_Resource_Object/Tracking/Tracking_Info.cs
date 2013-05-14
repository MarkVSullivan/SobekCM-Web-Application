#region Using directives

using System;
using System.Collections.Generic;
using System.Data;

#endregion

namespace SobekCM.Resource_Object.Tracking
{
    /// <summary> Class stores all of the tracking-specific data about a digital resource </summary>
    /// <remarks> This class is usually empty and is only populate if an internal user or a user who 
    /// can edit this item looks at an the citation pages online. </remarks>
    [Serializable]
    public class Tracking_Info
    {
        private List<Tracking_ArchiveMedia> archiveMedia;
        private Nullable<DateTime> digitalAcquisition;
        private short dispositionAdvice;
        private string dispositionAdviceNotes;
        private string dispositionNotes;
        private short dispositionType;

        private Nullable<DateTime> disposition_date;
        private byte firstFlagsByte;
        private Nullable<DateTime> imageProcessing;
        private string internal_comments;
        private short lastMilestone;
        private Nullable<DateTime> material_recd_date;
        private string material_recd_notes;
        private Nullable<DateTime> onlineComplete;
        private Nullable<DateTime> qualityControl;
        private byte secondFlagsByte;

        private string trackingBox;

        private string vid_source;
        private List<Tracking_Progress> worklogWorkHistory;

        /// <summary> Constructor for a new instance of the Tracking_Info class </summary>
        public Tracking_Info()
        {
            dispositionAdvice = -1;
            lastMilestone = -1;
            firstFlagsByte = 0;
            secondFlagsByte = 0;
            worklogWorkHistory = new List<Tracking_Progress>();
            archiveMedia = new List<Tracking_ArchiveMedia>();
        }

        #region Methods relating to the firstFlagsByte byte which contains several boolean values

        // THE FIRST FLAG BYTE CONTAINS THE FOLLOWING SINGLE VALUES
        // 1's bit = Material_Rec_Date_Estimated
        // 2's bit = Born_Digital NULL value
        // 4's bit = Born_Digital
        // 8's bit = Tracking_Info_Pulled
        // 16's bit = Locally_Archived
        // 32's bit = Remotely_Archived
        // 64's bit = hasArchiveInformation
        // 128's bit = Track_By_Month

        /// <summary> Flag indicates that any material received date for the physical
        /// manifestation of this digital resource is an estimated date only </summary>
        /// <remarks> For memory space savings, this is saved in the first bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool Material_Rec_Date_Estimated
        {
            get
            {
                if ((firstFlagsByte & ((byte) 1)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    firstFlagsByte = (byte) (firstFlagsByte | ((byte) 1));
                }
                else
                {
                    if (Material_Rec_Date_Estimated)
                        firstFlagsByte = (byte) (firstFlagsByte ^ ((byte) 1));
                }
            }
        }

        /// <summary> Flag indicates if this material is flagged as being born digital or if that value
        /// should currently be considered NULL. </summary>
        /// <remarks> For memory space savings, this is saved in the second bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool Born_Digital_Is_Null
        {
            get
            {
                if ((firstFlagsByte & ((byte) 2)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    firstFlagsByte = (byte) (firstFlagsByte | ((byte) 2));
                }
                else
                {
                    firstFlagsByte = (byte) (firstFlagsByte & ((byte) 253));
                }
            }
        }

        /// <summary> Flag indicates this digital resource was born digital, and either never
        /// had a physical manifestation or is not derived from a physical manifestation. </summary>
        /// <remarks> For memory space savings, this is saved in the third bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool Born_Digital
        {
            get
            {
                if ((firstFlagsByte & ((byte) 4)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                Born_Digital_Is_Null = false;
                if (value)
                {
                    firstFlagsByte = (byte) (firstFlagsByte | ((byte) 4));
                }
                else
                {
                    if (Born_Digital)
                        firstFlagsByte = (byte) (firstFlagsByte ^ ((byte) 4));
                }
            }
        }

        /// <summary> Flag indicates if the tracking information ( history, media, and archives ) 
        /// has been pulled for this item </summary>
        /// <remarks> For memory space savings, this is saved in the fourth bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool Tracking_Info_Pulled
        {
            get
            {
                if ((firstFlagsByte & ((byte) 8)) > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary> Flag indicates that some of the digital resource files for this item 
        /// have been locally archived for safe-keeping.</summary>
        /// <remarks> For memory space savings, this is saved in the fifth bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool Locally_Archived
        {
            get
            {
                if ((firstFlagsByte & ((byte) 16)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    firstFlagsByte = (byte) (firstFlagsByte | ((byte) 16));
                }
                else
                {
                    if (Locally_Archived)
                        firstFlagsByte = (byte) (firstFlagsByte ^ ((byte) 16));
                }
            }
        }

        /// <summary> Flag indicates that some of the digital resource files for this item 
        /// have been remotely archived for safe-keeping.</summary>
        /// <remarks> For memory space savings, this is saved in the sixth bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool Remotely_Archived
        {
            get
            {
                if ((firstFlagsByte & ((byte) 32)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    firstFlagsByte = (byte) (firstFlagsByte | ((byte) 32));
                }
                else
                {
                    if (Remotely_Archived)
                        firstFlagsByte = (byte) (firstFlagsByte ^ ((byte) 32));
                }
            }
        }


        /// <summary> Flag indicates if this item has any archive information to display </summary>
        /// <remarks> For memory space savings, this is saved in the seventh bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. </remarks>
        public bool hasArchiveInformation
        {
            get
            {
                if ((firstFlagsByte & ((byte) 64)) > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary> Flag indicates that this material is generally tracked by month </summary>
        /// <remarks> For memory space savings, this is saved in the eighth bit of a byte field (the first flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations.<br /><br />
        /// This value is actually set at the item group (bibid) level </remarks>
        public bool Track_By_Month
        {
            get
            {
                if ((firstFlagsByte & ((byte) 128)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    firstFlagsByte = (byte) (firstFlagsByte | ((byte) 128));
                }
                else
                {
                    if (Track_By_Month)
                        firstFlagsByte = (byte) (firstFlagsByte ^ ((byte) 128));
                }
            }
        }

        // THE SECOND FLAG BYTE CONTAINS THE FOLLOWING SINGLE VALUES
        // 1's bit = Large Format
        // 2's bit = Never Overlay Record

        /// <summary> Flag indicates if the item is considered large format, which will
        /// impact the size of the resulting derivative files </summary>
        /// <remarks> For memory space savings, this is saved in the first bit of a byte field (the second flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations. <br /><br />
        /// This value is actually set at the item group (bibid) level </remarks>
        public bool Large_Format
        {
            get
            {
                if ((secondFlagsByte & ((byte) 1)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    secondFlagsByte = (byte) (secondFlagsByte | ((byte) 1));
                }
                else
                {
                    if (Large_Format)
                        secondFlagsByte = (byte) (secondFlagsByte ^ ((byte) 1));
                }
            }
        }

        /// <summary> Flag indicates the originating record was significantly changed during import
        /// and the resulting METS files should never be overlayed from the source MARC file </summary>
        /// <remarks> For memory space savings, this is saved in the second bit of a byte field (the second flags byte),
        /// along with other flags necessary for this object.  Checking and setting this
        /// flag requires bitwise operations.<br /><br />
        /// This value is actually set at the item group (bibid) level </remarks>
        public bool Never_Overlay_Record
        {
            get
            {
                if ((secondFlagsByte & ((byte) 2)) > 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    secondFlagsByte = (byte) (secondFlagsByte | ((byte) 2));
                }
                else
                {
                    if (Never_Overlay_Record)
                        secondFlagsByte = (byte) (secondFlagsByte ^ ((byte) 2));
                }
            }
        }

        #endregion

        #region Simple Public Properties

        /// <summary> Value advices how the physical material SHOULD BE disposed of after digitization </summary>
        /// <remarks> This points to a value held in a database table, or -1 if no advice is given </remarks>
        public short Disposition_Advice
        {
            get { return dispositionAdvice; }
            set { dispositionAdvice = value; }
        }

        /// <summary> Notes regarding how this item should be disposed of (i.e., returned to whom, etc..) </summary>
        public string Disposition_Advice_Notes
        {
            get { return dispositionAdviceNotes ?? String.Empty; }
            set { dispositionAdviceNotes = value; }
        }

        /// <summary> Value indicates how the physical material ACTUALLY WAS disposed of after digitization </summary>
        /// <remarks> This points to a value held in a database table, or -1 if no disposition type was given </remarks>
        public short Disposition_Type
        {
            get { return dispositionType; }
            set { dispositionType = value; }
        }

        /// <summary> Notes regarding how this item was disposed of (i.e., returned to whom, etc..) </summary>
        public string Disposition_Notes
        {
            get { return dispositionNotes ?? String.Empty; }
            set { dispositionNotes = value; }
        }

        /// <summary> Value indicates which digitization milestone was last met.  </summary>
        /// <remarks> The true values are derived from the dates for each milestone.  This value is
        /// mostly used for quick report creation. </remarks>
        public short Last_Milestone
        {
            get { return lastMilestone; }
            set { lastMilestone = value; }
        }

        /// <summary> Gets and sets the internal comments, which accompany this material but do not publicly show online </summary>
        public string Internal_Comments
        {
            get { return internal_comments ?? String.Empty; }
            set { internal_comments = value; }
        }

        /// <summary> Value which represents how this item was originally added to the database </summary>
        public string VID_Source
        {
            get { return vid_source ?? String.Empty; }
            set { vid_source = value; }
        }

        /// <summary> Date the physical material was received in the office for digitizing </summary>
        public Nullable<DateTime> Material_Received_Date
        {
            get { return material_recd_date; }
            set { material_recd_date = value; }
        }

        /// <summary> Notes associated with the material receipt </summary>
        /// <remarks> This is just used to input notes about receipt while creating a new record.  This is stored in the worklog and 
        /// is not repopulated when an item is created.  </remarks>
        public string Material_Received_Notes
        {
            get
            {
                return material_recd_notes ?? String.Empty;
                ;
            }
            set { material_recd_notes = value; }
        }

        /// <summary> Date the physical material left the office after digitizing </summary>
        public Nullable<DateTime> Disposition_Date
        {
            get { return disposition_date; }
            set { disposition_date = value; }
        }

        /// <summary> First digitization milestone which represents when digital files were acquired from the 
        /// physical original, or the date the born-digital file was uploaded or created </summary>
        public Nullable<DateTime> Digital_Acquisition_Milestone
        {
            get { return digitalAcquisition; }
            set { digitalAcquisition = value; }
        }

        /// <summary> Second digitization milestone which represents when digital files were post-acquisition
        /// proccessed, for everything from color balance, cropping to copyright blur. </summary>
        public Nullable<DateTime> Image_Processing_Milestone
        {
            get { return imageProcessing; }
            set { imageProcessing = value; }
        }

        /// <summary> Third digitization milestone which represents when digital files had quality control performed </summary>
        public Nullable<DateTime> Quality_Control_Milestone
        {
            get { return qualityControl; }
            set { qualityControl = value; }
        }

        /// <summary> Fourth digitization milestone which represents when digital files were
        /// marked as complete and made either public or IP restricted, depending on the intended
        /// audience.  </summary>
        public Nullable<DateTime> Online_Complete_Milestone
        {
            get { return onlineComplete; }
            set { onlineComplete = value; }
        }

        /// <summary> Indicates the current (or previous) tracking box in which this material 
        /// was contained and generally stepped through the workflow milestones with </summary>
        public string Tracking_Box
        {
            get { return trackingBox ?? String.Empty; }
            set { trackingBox = value; }
        }

        /// <summary> Datatable holds all the history/status information for this item </summary>
        public List<Tracking_Progress> Work_History
        {
            get { return worklogWorkHistory; }
        }


        /// <summary> Datatable holds all the information about any media linked to this item </summary>
        public List<Tracking_ArchiveMedia> Archive_Media
        {
            get { return archiveMedia; }
        }

        ///// <summary> Any UMI flag information associated with this item (used for ETDs) </summary>
        //public string UMI_Flag
        //{
        //    get { return umi_flag ?? String.Empty; }
        //    set { umi_flag = value; }
        //}

        /// <summary> Flag indicates if this item has any media information to display </summary>
        public bool hasMediaInformation
        {
            get
            {
                if ((archiveMedia == null) || (archiveMedia.Count == 0))
                    return false;
                else
                    return true;
            }
        }


        /// <summary> Flag indicates if this item has any history/worklog information to display </summary>
        public bool hasHistoryInformation
        {
            get
            {
                if ((worklogWorkHistory == null) || (worklogWorkHistory.Count == 0))
                    return false;
                else
                    return true;
            }
        }

        #endregion

        #region Method to read the tracking information dataset from the SobekCM database 

        /// <summary> Method to read the tracking information dataset from the SobekCM database
        /// and set all the internal values in this object </summary>
        /// <param name="Tracking_Info"> DataSet with all the history, media, and archives information </param>
        public void Set_Tracking_Info(DataSet Tracking_Info)
        {
            firstFlagsByte = (byte) 8;

            // Pull all the worklog history values out of the table
            worklogWorkHistory.Clear();
            DataColumn workflowNameColumn = Tracking_Info.Tables[1].Columns["Workflow Name"];
            DataColumn workflowDateColumn = Tracking_Info.Tables[1].Columns["Completed Date"];
            DataColumn workflowAuthorColumn = Tracking_Info.Tables[1].Columns["WorkPerformedBy"];
            DataColumn workflowFilepathColumn = Tracking_Info.Tables[1].Columns["WorkingFilePath"];
            DataColumn workflowNoteColumn = Tracking_Info.Tables[1].Columns["Note"];
            DataTable workTable = Tracking_Info.Tables[1];
            foreach (DataRow thisRow in Tracking_Info.Tables[1].Rows)
            {
                Tracking_Progress addProgress = null;
                if (thisRow[workflowDateColumn] == DBNull.Value)
                    addProgress = new Tracking_Progress(thisRow[workflowNameColumn].ToString(), thisRow[workflowAuthorColumn].ToString(), thisRow[workflowFilepathColumn].ToString(), thisRow[workflowNoteColumn].ToString(), null);
                else
                {
                    string dateAsString = thisRow[workflowDateColumn].ToString().Trim();
                    if (dateAsString.Length > 0)
                    {
                        addProgress = new Tracking_Progress(thisRow[workflowNameColumn].ToString(), thisRow[workflowAuthorColumn].ToString(), thisRow[workflowFilepathColumn].ToString(), thisRow[workflowNoteColumn].ToString(), Convert.ToDateTime(thisRow[workflowDateColumn]));
                    }
                    else
                    {
                        addProgress = new Tracking_Progress(thisRow[workflowNameColumn].ToString(), thisRow[workflowAuthorColumn].ToString(), thisRow[workflowFilepathColumn].ToString(), thisRow[workflowNoteColumn].ToString(), null);
                    }
                }

                worklogWorkHistory.Add(addProgress);
            }

            // Set flag about archived files ( but don't keep the files right now )
            if (Tracking_Info.Tables[2].Rows.Count > 0)
                firstFlagsByte = (byte) (firstFlagsByte | ((byte) 64));

            // Pull all the archive media values out of the table
            archiveMedia.Clear();
            if (Tracking_Info.Tables[0].Rows.Count > 0)
            {
                DataColumn mediaNumberColumn = Tracking_Info.Tables[0].Columns["CD_Number"];
                DataColumn mediaFilerangeColumn = Tracking_Info.Tables[0].Columns["File_Range"];
                DataColumn mediaSizeColumn = Tracking_Info.Tables[0].Columns["Size"];
                DataColumn mediaImgesColumn = Tracking_Info.Tables[0].Columns["Images"];
                DataColumn mediaDateColumn = Tracking_Info.Tables[0].Columns["Date_Burned"];

                foreach (DataRow thisRow in Tracking_Info.Tables[0].Rows)
                {
                    string size = String.Empty;
                    if (thisRow[mediaSizeColumn] != DBNull.Value)
                        size = thisRow[mediaSizeColumn].ToString();
                    int images = 0;
                    if (thisRow[mediaImgesColumn] != DBNull.Value)
                        images = Convert.ToInt32(thisRow[mediaImgesColumn]);
                    Tracking_ArchiveMedia addMedia = new Tracking_ArchiveMedia(thisRow[mediaNumberColumn].ToString(), thisRow[mediaFilerangeColumn].ToString(), images, size, Convert.ToDateTime(thisRow[mediaDateColumn]));
                    archiveMedia.Add(addMedia);
                }
            }

            // Pull all the single tracking values out of the dataset
            DataRow itemRow = Tracking_Info.Tables[3].Rows[0];
            Locally_Archived = Convert.ToBoolean(itemRow["Locally_Archived"]);
            Remotely_Archived = Convert.ToBoolean(itemRow["Remotely_Archived"]);
            Born_Digital = Convert.ToBoolean(itemRow["Born_Digital"]);
            if (itemRow["Disposition_Advice"] == DBNull.Value)
                dispositionAdvice = -1;
            else
                dispositionAdvice = Convert.ToInt16(itemRow["Disposition_Advice"]);
            if (itemRow["Material_Received_Date"] == DBNull.Value)
                material_recd_date = null;
            else
                material_recd_date = Convert.ToDateTime(itemRow["Material_Received_Date"]);
            Material_Rec_Date_Estimated = Convert.ToBoolean(itemRow["Material_Recd_Date_Estimated"]);
            if (itemRow["VIDSource"] != DBNull.Value)
                vid_source = itemRow["VIDSource"].ToString();
            lastMilestone = Convert.ToInt16(itemRow["Last_Milestone"]);
            if (itemRow["Milestone_DigitalAcquisition"] == DBNull.Value)
                digitalAcquisition = null;
            else
                digitalAcquisition = Convert.ToDateTime(itemRow["Milestone_DigitalAcquisition"]);
            if (itemRow["Milestone_ImageProcessing"] == DBNull.Value)
                imageProcessing = null;
            else
                imageProcessing = Convert.ToDateTime(itemRow["Milestone_ImageProcessing"]);
            if (itemRow["Milestone_QualityControl"] == DBNull.Value)
                qualityControl = null;
            else
                qualityControl = Convert.ToDateTime(itemRow["Milestone_QualityControl"]);
            if (itemRow["Milestone_OnlineComplete"] == DBNull.Value)
                onlineComplete = null;
            else
                onlineComplete = Convert.ToDateTime(itemRow["Milestone_OnlineComplete"]);
            if (itemRow["Disposition_Date"] == DBNull.Value)
                disposition_date = null;
            else
                disposition_date = Convert.ToDateTime(itemRow["Disposition_Date"]);
            if (itemRow["Disposition_Type"] == DBNull.Value)
                dispositionType = -1;
            else
                dispositionType = Convert.ToInt16(itemRow["Disposition_Type"]);
            if (itemRow["Tracking_Box"] == DBNull.Value)
                trackingBox = String.Empty;
            else
                trackingBox = itemRow["Tracking_Box"].ToString();
            if (itemRow["Disposition_Advice_Notes"] == DBNull.Value)
                dispositionAdviceNotes = String.Empty;
            else
                dispositionAdviceNotes = itemRow["Disposition_Advice_Notes"].ToString();
            if (itemRow["Disposition_Notes"] == DBNull.Value)
                dispositionNotes = String.Empty;
            else
                dispositionNotes = itemRow["Disposition_Notes"].ToString();
            Born_Digital = Convert.ToBoolean(itemRow["Born_Digital"]);
        }

        #endregion
    }
}