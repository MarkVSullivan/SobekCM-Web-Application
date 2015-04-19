namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Tracking_ItemViewer class </summary>
    public class Tracking_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Tracking_ItemViewer_Localization class </summary>
        public Tracking_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Tracking_ItemViewer";
        }

        /// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
        /// <param name="Key"> Key for the new localization string being saved </param>
        /// <param name="Value"> Value for this localization string </param>
        /// <remarks> This overrides the base class's implementation </remarks>
        public override void Add_Localization_String(string Key, string Value)
        {
            // First, add to the localization string dictionary
            base.Add_Localization_String(Key, Value);

            // Assign to custom properties depending on the key
            switch (Key)
            {
                case "Tracking Information":
                    TrackingInformation = Value;
                    break;

                case "Milestones":
                    Milestones = Value;
                    break;

                case "History":
                    History = Value;
                    break;

                case "Media":
                    Media = Value;
                    break;

                case "Archives":
                    Archives = Value;
                    break;

                case "Directory":
                    Directory = Value;
                    break;

                case "ITEM HAS NO HISTORY":
                    ITEMHASNOHISTORY = Value;
                    break;

                case "ITEM HISTORY":
                    ITEMHISTORY = Value;
                    break;

                case "Workflow Name":
                    WorkflowName = Value;
                    break;

                case "Completed Date":
                    CompletedDate = Value;
                    break;

                case "User":
                    User = Value;
                    break;

                case "Location Notes":
                    LocationNotes = Value;
                    break;

                case "ITEM IS NOT ARCHIVED TO MEDIA":
                    ITEMISNOTARCHIVEDTOMEDIA = Value;
                    break;

                case "CDDVD ARCHIVE":
                    CDDVDARCHIVE = Value;
                    break;

                case "CD Number":
                    CDNumber = Value;
                    break;

                case "File Range":
                    FileRange = Value;
                    break;

                case "Images":
                    Images = Value;
                    break;

                case "Size":
                    Size = Value;
                    break;

                case "Date Burned":
                    DateBurned = Value;
                    break;

                case "ITEM HAS NO ARCHIVE INFORMATION":
                    ITEMHASNOARCHIVEINFORMATION = Value;
                    break;

                case "ARCHIVED FILE INFORMATION":
                    ARCHIVEDFILEINFORMATION = Value;
                    break;

                case "Filename":
                    Filename = Value;
                    break;

                case "Last Write Date":
                    LastWriteDate = Value;
                    break;

                case "Archived Date":
                    ArchivedDate = Value;
                    break;

                case "PAGE FILES":
                    PAGEFILES = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "Date Modified":
                    DateModified = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

                case "METADATA FILES":
                    METADATAFILES = Value;
                    break;

                case "OTHER FILES":
                    OTHERFILES = Value;
                    break;

                case "JPEG Image":
                    JPEGImage = Value;
                    break;

                case "Thumbnail Image":
                    ThumbnailImage = Value;
                    break;

                case "Archival TIFF Image":
                    ArchivalTIFFImage = Value;
                    break;

                case "JPEG2000 Zoomable Image":
                    JPEG2000ZoomableImage = Value;
                    break;

                case "Adobe Acrobat Document":
                    AdobeAcrobatDocument = Value;
                    break;

                case "Text File":
                    TextFile = Value;
                    break;

                case "Microsoft Office Excel Worksheet":
                    MicrosoftOfficeExcelWorksheet = Value;
                    break;

                case "Microsoft Office Word Document":
                    MicrosoftOfficeWordDocument = Value;
                    break;

                case "Microsoft Office Powerpoint Presentation":
                    MicrosoftOfficePowerpointPresentation = Value;
                    break;

                case "Shockwave Flash Object":
                    ShockwaveFlashObject = Value;
                    break;

                case "Citationonly METS File":
                    CitationonlyMETSFile = Value;
                    break;

                case "MARC XML File":
                    MARCXMLFile = Value;
                    break;

                case "Sobekcm Service METS File":
                    SobekcmServiceMETSFile = Value;
                    break;

                case "XML Document":
                    XMLDocument = Value;
                    break;

                case "Usersubmitted METS File":
                    UsersubmittedMETSFile = Value;
                    break;

                case "Previous METS File Version":
                    PreviousMETSFileVersion = Value;
                    break;

                case "Backup File":
                    BackupFile = Value;
                    break;

                case "FDA Ingest Report":
                    FDAIngestReport = Value;
                    break;

                case "DIGITIZATION MILESTONES":
                    DIGITIZATIONMILESTONES = Value;
                    break;

                case "Digital Acquisition":
                    DigitalAcquisition = Value;
                    break;

                case "Postacquisition Processing":
                    PostacquisitionProcessing = Value;
                    break;

                case "Quality Control Performed":
                    QualityControlPerformed = Value;
                    break;

                case "Online Complete":
                    OnlineComplete = Value;
                    break;

                case "PHYSICAL MATERIAL MILESTONES":
                    PHYSICALMATERIALMILESTONES = Value;
                    break;

                case "Materials Received":
                    MaterialsReceived = Value;
                    break;

                case "Disposition Date":
                    DispositionDate = Value;
                    break;

                case "Tracking Box":
                    TrackingBox = Value;
                    break;

                case "Born Digital":
                    BornDigital = Value;
                    break;

                case "Disposition Advice":
                    DispositionAdvice = Value;
                    break;

                case "Locally Stored On CD Or Tape":
                    LocallyStoredOnCDOrTape = Value;
                    break;

                case "Archived Remotely FDA":
                    ArchivedRemotelyFDA = Value;
                    break;

                case "NOT ARCHIVED":
                    NOTARCHIVED = Value;
                    break;

                case "PHYSICAL MATERIAL RELATED FIELDS":
                    PHYSICALMATERIALRELATEDFIELDS = Value;
                    break;

                case "ARCHIVING MILESTONES":
                    ARCHIVINGMILESTONES = Value;
                    break;

            }
        }
        /// <remarks> 'Tracking Information' localization string </remarks>
        public string TrackingInformation { get; private set; }

        /// <remarks> 'Milestones' localization string </remarks>
        public string Milestones { get; private set; }

        /// <remarks> 'History' localization string </remarks>
        public string History { get; private set; }

        /// <remarks> 'Media' localization string </remarks>
        public string Media { get; private set; }

        /// <remarks> 'Archives' localization string </remarks>
        public string Archives { get; private set; }

        /// <remarks> 'Directory' localization string </remarks>
        public string Directory { get; private set; }

        /// <remarks> 'ITEM HAS NO HISTORY' localization string </remarks>
        public string ITEMHASNOHISTORY { get; private set; }

        /// <remarks> 'ITEM HISTORY' localization string </remarks>
        public string ITEMHISTORY { get; private set; }

        /// <remarks> 'Workflow Name' localization string </remarks>
        public string WorkflowName { get; private set; }

        /// <remarks> 'Completed Date' localization string </remarks>
        public string CompletedDate { get; private set; }

        /// <remarks> 'User' localization string </remarks>
        public string User { get; private set; }

        /// <remarks> 'Location / Notes' localization string </remarks>
        public string LocationNotes { get; private set; }

        /// <remarks> 'ITEM IS NOT ARCHIVED TO MEDIA' localization string </remarks>
        public string ITEMISNOTARCHIVEDTOMEDIA { get; private set; }

        /// <remarks> 'CD/DVD ARCHIVE' localization string </remarks>
        public string CDDVDARCHIVE { get; private set; }

        /// <remarks> 'CD Number' localization string </remarks>
        public string CDNumber { get; private set; }

        /// <remarks> 'File Range' localization string </remarks>
        public string FileRange { get; private set; }

        /// <remarks> 'Images' localization string </remarks>
        public string Images { get; private set; }

        /// <remarks> 'Size' localization string </remarks>
        public string Size { get; private set; }

        /// <remarks> 'Date Burned' localization string </remarks>
        public string DateBurned { get; private set; }

        /// <remarks> 'ITEM HAS NO ARCHIVE INFORMATION' localization string </remarks>
        public string ITEMHASNOARCHIVEINFORMATION { get; private set; }

        /// <remarks> 'ARCHIVED FILE INFORMATION' localization string </remarks>
        public string ARCHIVEDFILEINFORMATION { get; private set; }

        /// <remarks> 'Filename' localization string </remarks>
        public string Filename { get; private set; }

        /// <remarks> 'Last Write Date' localization string </remarks>
        public string LastWriteDate { get; private set; }

        /// <remarks> 'Archived Date' localization string </remarks>
        public string ArchivedDate { get; private set; }

        /// <remarks> 'PAGE FILES' localization string </remarks>
        public string PAGEFILES { get; private set; }

        /// <remarks> 'Name' localization string </remarks>
        public string Name { get; private set; }

        /// <remarks> 'Date Modified' localization string </remarks>
        public string DateModified { get; private set; }

        /// <remarks> 'Type' localization string </remarks>
        public string Type { get; private set; }

        /// <remarks> 'METADATA FILES' localization string </remarks>
        public string METADATAFILES { get; private set; }

        /// <remarks> 'OTHER FILES' localization string </remarks>
        public string OTHERFILES { get; private set; }

        /// <remarks> 'JPEG image' localization string </remarks>
        public string JPEGImage { get; private set; }

        /// <remarks> 'Thumbnail image' localization string </remarks>
        public string ThumbnailImage { get; private set; }

        /// <remarks> 'Archival TIFF image' localization string </remarks>
        public string ArchivalTIFFImage { get; private set; }

        /// <remarks> 'JPEG2000 Zoomable image' localization string </remarks>
        public string JPEG2000ZoomableImage { get; private set; }

        /// <remarks> 'Adobe Acrobat Document' localization string </remarks>
        public string AdobeAcrobatDocument { get; private set; }

        /// <remarks> 'Text file' localization string </remarks>
        public string TextFile { get; private set; }

        /// <remarks> 'Microsoft Office Excel Worksheet' localization string </remarks>
        public string MicrosoftOfficeExcelWorksheet { get; private set; }

        /// <remarks> 'Microsoft Office Word Document' localization string </remarks>
        public string MicrosoftOfficeWordDocument { get; private set; }

        /// <remarks> 'Microsoft Office Powerpoint Presentation' localization string </remarks>
        public string MicrosoftOfficePowerpointPresentation { get; private set; }

        /// <remarks> 'Shockwave Flash Object' localization string </remarks>
        public string ShockwaveFlashObject { get; private set; }

        /// <remarks> 'Citation-only METS File' localization string </remarks>
        public string CitationonlyMETSFile { get; private set; }

        /// <remarks> 'MARC XML File' localization string </remarks>
        public string MARCXMLFile { get; private set; }

        /// <remarks> 'SobekCM Service METS File' localization string </remarks>
        public string SobekcmServiceMETSFile { get; private set; }

        /// <remarks> 'XML Document' localization string </remarks>
        public string XMLDocument { get; private set; }

        /// <remarks> 'User-submitted METS File' localization string </remarks>
        public string UsersubmittedMETSFile { get; private set; }

        /// <remarks> 'Previous METS File Version' localization string </remarks>
        public string PreviousMETSFileVersion { get; private set; }

        /// <remarks> 'Backup File' localization string </remarks>
        public string BackupFile { get; private set; }

        /// <remarks> 'FDA Ingest Report' localization string </remarks>
        public string FDAIngestReport { get; private set; }

        /// <remarks> 'DIGITIZATION MILESTONES' localization string </remarks>
        public string DIGITIZATIONMILESTONES { get; private set; }

        /// <remarks> 'Digital Acquisition' localization string </remarks>
        public string DigitalAcquisition { get; private set; }

        /// <remarks> 'Post-Acquisition Processing' localization string </remarks>
        public string PostacquisitionProcessing { get; private set; }

        /// <remarks> 'Quality Control Performed' localization string </remarks>
        public string QualityControlPerformed { get; private set; }

        /// <remarks> 'Online Complete' localization string </remarks>
        public string OnlineComplete { get; private set; }

        /// <remarks> 'PHYSICAL MATERIAL MILESTONES' localization string </remarks>
        public string PHYSICALMATERIALMILESTONES { get; private set; }

        /// <remarks> 'Materials Received' localization string </remarks>
        public string MaterialsReceived { get; private set; }

        /// <remarks> 'Disposition Date' localization string </remarks>
        public string DispositionDate { get; private set; }

        /// <remarks> 'Tracking Box' localization string </remarks>
        public string TrackingBox { get; private set; }

        /// <remarks> 'Born Digital' localization string </remarks>
        public string BornDigital { get; private set; }

        /// <remarks> 'Disposition Advice' localization string </remarks>
        public string DispositionAdvice { get; private set; }

        /// <remarks> 'Locally Stored on CD or Tape' localization string </remarks>
        public string LocallyStoredOnCDOrTape { get; private set; }

        /// <remarks> 'Archived Remotely (FDA)' localization string </remarks>
        public string ArchivedRemotelyFDA { get; private set; }

        /// <remarks> 'NOT ARCHIVED' localization string </remarks>
        public string NOTARCHIVED { get; private set; }

        /// <remarks> 'PHYSICAL MATERIAL RELATED FIELDS' localization string </remarks>
        public string PHYSICALMATERIALRELATEDFIELDS { get; private set; }

        /// <remarks> 'ARCHIVING MILESTONES' localization string </remarks>
        public string ARCHIVINGMILESTONES { get; private set; }

    }
}
