namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the QC_ItemViewer class </summary>
    public class QC_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the QC_ItemViewer_Localization class </summary>
        public QC_ItemViewer_LocalizationInfo() : base()
        {
            // Do nothing
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
                case "Thumbnails Per Page":
                    ThumbnailsPerPage = Value;
                    break;

                case "After":
                    After = Value;
                    break;

                case "All Thumbnails":
                    AllThumbnails = Value;
                    break;

                case "Automatic Numbering":
                    AutomaticNumbering = Value;
                    break;

                case "Before":
                    Before = Value;
                    break;

                case "Blur Needed":
                    BlurNeeded = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "CANCEL":
                    CANCEL = Value;
                    break;

                case "Cancel This Move":
                    CancelThisMove = Value;
                    break;

                case "Check For The Beginning Of A New Division Type":
                    CheckForTheBeginningOfANewDivisionType = Value;
                    break;

                case "Choose Main Thumbnail":
                    ChooseMainThumbnail = Value;
                    break;

                case "Clear All Reorder Pages":
                    ClearAllReorderPages = Value;
                    break;

                case "Clear Pagination":
                    ClearPagination = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "Complete":
                    Complete = Value;
                    break;

                case "Critical Volume Error":
                    CriticalVolumeError = Value;
                    break;

                case "Delete Multiple Pages":
                    DeleteMultiplePages = Value;
                    break;

                case "Delete This Page And Related Files":
                    DeleteThisPageAndRelatedFiles = Value;
                    break;

                case "Disabled":
                    Disabled = Value;
                    break;

                case "Division":
                    Division = Value;
                    break;

                case "Drag Drop Pages":
                    DragDropPages = Value;
                    break;

                case "Edit":
                    Edit = Value;
                    break;

                case "Enabled":
                    Enabled = Value;
                    break;

                case "Enabled With Confirmation":
                    EnabledWithConfirmation = Value;
                    break;

                case "Entire Document":
                    EntireDocument = Value;
                    break;

                case "FILE ERROR":
                    FILEERROR = Value;
                    break;

                case "Go To Thumbnail":
                    GoToThumbnail = Value;
                    break;

                case "Go To":
                    GoTo = Value;
                    break;

                case "Help":
                    Help = Value;
                    break;

                case "Image Quality Error":
                    ImageQualityError = Value;
                    break;

                case "Incorrect Volume":
                    IncorrectVolume = Value;
                    break;

                case "Invalid Images":
                    InvalidImages = Value;
                    break;

                case "Large Thumbnails":
                    LargeThumbnails = Value;
                    break;

                case "Mark An Error On This Page Image":
                    MarkAnErrorOnThisPageImage = Value;
                    break;

                case "Medium Thumbnails":
                    MediumThumbnails = Value;
                    break;

                case "Missing Icon":
                    MissingIcon = Value;
                    break;

                case "Missing Thumbnail":
                    MissingThumbnail = Value;
                    break;

                case "Move Multiple Pages":
                    MoveMultiplePages = Value;
                    break;

                case "MOVE SELECTED PAGES":
                    MOVESELECTEDPAGES = Value;
                    break;

                case "No Automatic Numbering":
                    NoAutomaticNumbering = Value;
                    break;

                case "No File Error":
                    NoFileError = Value;
                    break;

                case "No Volume Error":
                    NoVolumeError = Value;
                    break;

                case "Open This Page In A New Window":
                    OpenThisPageInANewWindow = Value;
                    break;

                case "Orientation Error":
                    OrientationError = Value;
                    break;

                case "Other Specify":
                    OtherSpecify = Value;
                    break;

                case "Overcropped":
                    Overcropped = Value;
                    break;

                case "Page":
                    Page = Value;
                    break;

                case "Pagination":
                    Pagination = Value;
                    break;

                case "PREVIEW":
                    PREVIEW = Value;
                    break;

                case "Processing Required":
                    ProcessingRequired = Value;
                    break;

                case "Recapture Required":
                    RecaptureRequired = Value;
                    break;

                case "Resource":
                    Resource = Value;
                    break;

                case "Save":
                    Save = Value;
                    break;

                case "Save The Resource And Apply Your Changes":
                    SaveTheResourceAndApplyYourChanges = Value;
                    break;

                case "Save This Error":
                    SaveThisError = Value;
                    break;

                case "Settings":
                    Settings = Value;
                    break;

                case "Skew Error":
                    SkewError = Value;
                    break;

                case "Small Thumbnails":
                    SmallThumbnails = Value;
                    break;

                case "Standard Cursor":
                    StandardCursor = Value;
                    break;

                case "SUBMIT":
                    SUBMIT = Value;
                    break;

                case "Technical Specification Error":
                    TechnicalSpecificationError = Value;
                    break;

                case "Thumbnail Size":
                    ThumbnailSize = Value;
                    break;

                case "Unblur Needed":
                    UnblurNeeded = Value;
                    break;

                case "Undercropped":
                    Undercropped = Value;
                    break;

                case "Upload New Page Image Files":
                    UploadNewPageImageFiles = Value;
                    break;

                case "View":
                    View = Value;
                    break;

                case "View Directory":
                    ViewDirectory = Value;
                    break;

                case "View METS":
                    ViewMETS = Value;
                    break;

                case "View QC History":
                    ViewQCHistory = Value;
                    break;

                case "View Technical Image Information":
                    ViewTechnicalImageInformation = Value;
                    break;

                case "Within Same Division":
                    WithinSameDivision = Value;
                    break;

            }
        }
        /// <remarks> Number of thumbnails to show per page </remarks>
        public string ThumbnailsPerPage { get; private set; }

        /// <remarks> 'After' localization string </remarks>
        public string After { get; private set; }

        /// <remarks> Show all thumbnails on one page </remarks>
        public string AllThumbnails { get; private set; }

        /// <remarks> 'Automatic numbering' localization string </remarks>
        public string AutomaticNumbering { get; private set; }

        /// <remarks> 'Before' localization string </remarks>
        public string Before { get; private set; }

        /// <remarks> 'Blur needed' localization string </remarks>
        public string BlurNeeded { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> 'CANCEL' localization string </remarks>
        public string CANCEL { get; private set; }

        /// <remarks> 'Cancel this move' localization string </remarks>
        public string CancelThisMove { get; private set; }

        /// <remarks> 'Check for the beginning of a new division type' localization string </remarks>
        public string CheckForTheBeginningOfANewDivisionType { get; private set; }

        /// <remarks> 'Choose main thumbnail' localization string </remarks>
        public string ChooseMainThumbnail { get; private set; }

        /// <remarks> 'Clear All & Reorder Pages' localization string </remarks>
        public string ClearAllReorderPages { get; private set; }

        /// <remarks> 'Clear Pagination' localization string </remarks>
        public string ClearPagination { get; private set; }

        /// <remarks> 'Comments:' localization string </remarks>
        public string Comments { get; private set; }

        /// <remarks> 'Complete' localization string </remarks>
        public string Complete { get; private set; }

        /// <remarks> 'Critical Volume error' localization string </remarks>
        public string CriticalVolumeError { get; private set; }

        /// <remarks> 'Delete multiple pages' localization string </remarks>
        public string DeleteMultiplePages { get; private set; }

        /// <remarks> 'Delete this page and related files' localization string </remarks>
        public string DeleteThisPageAndRelatedFiles { get; private set; }

        /// <remarks> 'Disabled' localization string </remarks>
        public string Disabled { get; private set; }

        /// <remarks> 'Division' localization string </remarks>
        public string Division { get; private set; }

        /// <remarks> 'Drag & drop pages' localization string </remarks>
        public string DragDropPages { get; private set; }

        /// <remarks> 'Edit' localization string </remarks>
        public string Edit { get; private set; }

        /// <remarks> 'Enabled' localization string </remarks>
        public string Enabled { get; private set; }

        /// <remarks> 'Enabled with confirmation' localization string </remarks>
        public string EnabledWithConfirmation { get; private set; }

        /// <remarks> 'Entire document' localization string </remarks>
        public string EntireDocument { get; private set; }

        /// <remarks> 'FILE ERROR' localization string </remarks>
        public string FILEERROR { get; private set; }

        /// <remarks> 'Go to thumbnail:' localization string </remarks>
        public string GoToThumbnail { get; private set; }

        /// <remarks> 'Go to:' localization string </remarks>
        public string GoTo { get; private set; }

        /// <remarks> 'Help' localization string </remarks>
        public string Help { get; private set; }

        /// <remarks> 'Image Quality Error' localization string </remarks>
        public string ImageQualityError { get; private set; }

        /// <remarks> 'Incorrect Volume' localization string </remarks>
        public string IncorrectVolume { get; private set; }

        /// <remarks> 'Invalid Images' localization string </remarks>
        public string InvalidImages { get; private set; }

        /// <remarks> Used for hover over for thumbnail sizes </remarks>
        public string LargeThumbnails { get; private set; }

        /// <remarks> 'Mark an error on this page image' localization string </remarks>
        public string MarkAnErrorOnThisPageImage { get; private set; }

        /// <remarks> Used for hover over for thumbnail sizes </remarks>
        public string MediumThumbnails { get; private set; }

        /// <remarks> 'Missing icon' localization string </remarks>
        public string MissingIcon { get; private set; }

        /// <remarks> 'Missing thumbnail' localization string </remarks>
        public string MissingThumbnail { get; private set; }

        /// <remarks> 'Move multiple pages' localization string </remarks>
        public string MoveMultiplePages { get; private set; }

        /// <remarks> 'MOVE SELECTED PAGES' localization string </remarks>
        public string MOVESELECTEDPAGES { get; private set; }

        /// <remarks> 'No automatic numbering' localization string </remarks>
        public string NoAutomaticNumbering { get; private set; }

        /// <remarks> 'No File error' localization string </remarks>
        public string NoFileError { get; private set; }

        /// <remarks> 'No Volume Error' localization string </remarks>
        public string NoVolumeError { get; private set; }

        /// <remarks> 'Open this page in a new window' localization string </remarks>
        public string OpenThisPageInANewWindow { get; private set; }

        /// <remarks> 'Orientation error' localization string </remarks>
        public string OrientationError { get; private set; }

        /// <remarks> 'Other (specify)' localization string </remarks>
        public string OtherSpecify { get; private set; }

        /// <remarks> 'Overcropped' localization string </remarks>
        public string Overcropped { get; private set; }

        /// <remarks> 'Page' localization string </remarks>
        public string Page { get; private set; }

        /// <remarks> 'Pagination' localization string </remarks>
        public string Pagination { get; private set; }

        /// <remarks> 'PREVIEW' localization string </remarks>
        public string PREVIEW { get; private set; }

        /// <remarks> 'Processing required' localization string </remarks>
        public string ProcessingRequired { get; private set; }

        /// <remarks> 'Recapture required' localization string </remarks>
        public string RecaptureRequired { get; private set; }

        /// <remarks> 'Resource' localization string </remarks>
        public string Resource { get; private set; }

        /// <remarks> 'Save' localization string </remarks>
        public string Save { get; private set; }

        /// <remarks> 'Save the resource and apply your changes' localization string </remarks>
        public string SaveTheResourceAndApplyYourChanges { get; private set; }

        /// <remarks> 'Save this error' localization string </remarks>
        public string SaveThisError { get; private set; }

        /// <remarks> 'Settings' localization string </remarks>
        public string Settings { get; private set; }

        /// <remarks> 'Skew error' localization string </remarks>
        public string SkewError { get; private set; }

        /// <remarks> Used for hover over for thumbnail sizes </remarks>
        public string SmallThumbnails { get; private set; }

        /// <remarks> 'Standard cursor' localization string </remarks>
        public string StandardCursor { get; private set; }

        /// <remarks> 'SUBMIT' localization string </remarks>
        public string SUBMIT { get; private set; }

        /// <remarks> 'Technical Specification Error' localization string </remarks>
        public string TechnicalSpecificationError { get; private set; }

        /// <remarks> 'Thumbnail size' localization string </remarks>
        public string ThumbnailSize { get; private set; }

        /// <remarks> 'Unblur needed' localization string </remarks>
        public string UnblurNeeded { get; private set; }

        /// <remarks> 'Undercropped' localization string </remarks>
        public string Undercropped { get; private set; }

        /// <remarks> 'Upload new page image files' localization string </remarks>
        public string UploadNewPageImageFiles { get; private set; }

        /// <remarks> 'View' localization string </remarks>
        public string View { get; private set; }

        /// <remarks> 'View Directory' localization string </remarks>
        public string ViewDirectory { get; private set; }

        /// <remarks> 'View METS' localization string </remarks>
        public string ViewMETS { get; private set; }

        /// <remarks> 'View QC History' localization string </remarks>
        public string ViewQCHistory { get; private set; }

        /// <remarks> 'View technical image information' localization string </remarks>
        public string ViewTechnicalImageInformation { get; private set; }

        /// <remarks> 'Within same division' localization string </remarks>
        public string WithinSameDivision { get; private set; }

    }
}
