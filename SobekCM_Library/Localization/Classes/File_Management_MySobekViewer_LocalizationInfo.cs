namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the File_Management_MySobekViewer class </summary>
    public class File_Management_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the File_Management_MySobekViewer_Localization class </summary>
        public File_Management_MySobekViewer_LocalizationInfo() : base()
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
                case "Add A New File For This Package":
                    AddANewFileForThisPackage = Value;
                    break;

                case "Click Here To Upload Page Images Instead":
                    ClickHereToUploadPageImagesInstead = Value;
                    break;

                case "ERROR CAUGHT WHILE SAVING DIGITAL RESOURCE":
                    ERRORCAUGHTWHILESAVINGDIGITALRESOURCE = Value;
                    break;

                case "Error During File Management For XXX":
                    ErrorDuringFileManagementForXXX = Value;
                    break;

                case "ERROR ENCOUNTERED DURING ONLINE FILE MANAGEMENT":
                    ERRORENCOUNTEREDDURINGONLINEFILEMANAGEMENT = Value;
                    break;

                case "Manage Downloads":
                    ManageDownloads = Value;
                    break;

                case "Upload The Download Files For Your Item You Can Also Provide Labels For Each File Once They Are Uploaded":
                    UploadTheDownloadFilesForYourItemYouCanAlsoProvideLabelsForEachFileOnceTheyAreUploaded = Value;
                    break;

            }
        }
        /// <remarks> 'Add a new file for this package' localization string </remarks>
        public string AddANewFileForThisPackage { get; private set; }

        /// <remarks> 'Click here to upload page images instead.' localization string </remarks>
        public string ClickHereToUploadPageImagesInstead { get; private set; }

        /// <remarks> 'ERROR CAUGHT WHILE SAVING DIGITAL RESOURCE' localization string </remarks>
        public string ERRORCAUGHTWHILESAVINGDIGITALRESOURCE { get; private set; }

        /// <remarks> 'Error during file management for %1' localization string </remarks>
        public string ErrorDuringFileManagementForXXX { get; private set; }

        /// <remarks> 'ERROR ENCOUNTERED DURING ONLINE FILE MANAGEMENT' localization string </remarks>
        public string ERRORENCOUNTEREDDURINGONLINEFILEMANAGEMENT { get; private set; }

        /// <remarks> 'Manage Downloads' localization string </remarks>
        public string ManageDownloads { get; private set; }

        /// <remarks> '"Upload the download files for your item.  You can also provide labels for each file, once they are uploaded."' localization string </remarks>
        public string UploadTheDownloadFilesForYourItemYouCanAlsoProvideLabelsForEachFileOnceTheyAreUploaded { get; private set; }

    }
}
