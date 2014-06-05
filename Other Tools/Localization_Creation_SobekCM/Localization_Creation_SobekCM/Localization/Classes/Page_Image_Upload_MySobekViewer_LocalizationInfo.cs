namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Page_Image_Upload_MySobekViewer class </summary>
    public class Page_Image_Upload_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Page_Image_Upload_MySobekViewer_Localization class </summary>
        public Page_Image_Upload_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Page_Image_Upload_MySobekViewer";
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
                case "ACTION":
                    ACTION = Value;
                    break;

                case "CANCEL":
                    CANCEL = Value;
                    break;

                case "Cancel This And Remove The Recentely Uploaded Images":
                    CancelThisAndRemoveTheRecentelyUploadedImages = Value;
                    break;

                case "Click Here To Add Download Files Instead":
                    ClickHereToAddDownloadFilesInstead = Value;
                    break;

                case "DATE UPLOADED":
                    DATEUPLOADED = Value;
                    break;

                case "FILENAME":
                    FILENAME = Value;
                    break;

                case "Once All Images Are Uploaded Press SUBMIT To Finish This Item":
                    OnceAllImagesAreUploadedPressSUBMITToFinishThisItem = Value;
                    break;

                case "SIZE":
                    SIZE = Value;
                    break;

                case "SUBMIT":
                    SUBMIT = Value;
                    break;

                case "Submit The Recently Uploaded Page Images And Complete The Process":
                    SubmitTheRecentlyUploadedPageImagesAndCompleteTheProcess = Value;
                    break;

                case "The Following Extensions Are Accepted":
                    TheFollowingExtensionsAreAccepted = Value;
                    break;

                case "The Following New Page Images Will Be Added To The Item Once You Click SUBMIT":
                    TheFollowingNewPageImagesWillBeAddedToTheItemOnceYouClickSUBMIT = Value;
                    break;

                case "Upload Page Images":
                    UploadPageImages = Value;
                    break;

                case "Upload The Page Images For Your Item You Will Then Be Directed To Manage The Pages And Divisions To Ensure The New Page Images Appear In The Correct Order And Are Reflected In The Table Of Contents":
                    UploadThePageImagesForYourItemYouWillThenBeDirectedToManageThePagesAndDivisionsToEnsureTheNewPageImagesAppearInTheCorrectOrderAndAreReflectedInTheTableOfContents = Value;
                    break;

            }
        }
        /// <remarks> 'ACTION' localization string </remarks>
        public string ACTION { get; private set; }

        /// <remarks> 'CANCEL' localization string </remarks>
        public string CANCEL { get; private set; }

        /// <remarks> 'Cancel this and remove the recentely uploaded images' localization string </remarks>
        public string CancelThisAndRemoveTheRecentelyUploadedImages { get; private set; }

        /// <remarks> 'Click here to add download files instead.' localization string </remarks>
        public string ClickHereToAddDownloadFilesInstead { get; private set; }

        /// <remarks> 'DATE UPLOADED' localization string </remarks>
        public string DATEUPLOADED { get; private set; }

        /// <remarks> 'FILENAME' localization string </remarks>
        public string FILENAME { get; private set; }

        /// <remarks> '"Once all images are uploaded, press SUBMIT to finish this item"' localization string </remarks>
        public string OnceAllImagesAreUploadedPressSUBMITToFinishThisItem { get; private set; }

        /// <remarks> 'SIZE' localization string </remarks>
        public string SIZE { get; private set; }

        /// <remarks> 'SUBMIT' localization string </remarks>
        public string SUBMIT { get; private set; }

        /// <remarks> 'Submit the recently uploaded page images and complete the process' localization string </remarks>
        public string SubmitTheRecentlyUploadedPageImagesAndCompleteTheProcess { get; private set; }

        /// <remarks> 'The following extensions are accepted' localization string </remarks>
        public string TheFollowingExtensionsAreAccepted { get; private set; }

        /// <remarks> 'The following new page images will be added to the item once you click SUBMIT' localization string </remarks>
        public string TheFollowingNewPageImagesWillBeAddedToTheItemOnceYouClickSUBMIT { get; private set; }

        /// <remarks> 'Upload Page Images' localization string </remarks>
        public string UploadPageImages { get; private set; }

        /// <remarks> 'Upload the page images for your item.  You will then be directed to manage the pages and divisions to ensure the new page images appear in the correct order and are reflected in the table of contents.' localization string </remarks>
        public string UploadThePageImagesForYourItemYouWillThenBeDirectedToManageThePagesAndDivisionsToEnsureTheNewPageImagesAppearInTheCorrectOrderAndAreReflectedInTheTableOfContents { get; private set; }

    }
}
