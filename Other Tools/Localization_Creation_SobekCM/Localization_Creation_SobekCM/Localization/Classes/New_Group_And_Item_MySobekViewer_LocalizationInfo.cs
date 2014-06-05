namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the New_Group_And_Item_MySobekViewer class </summary>
    public class New_Group_And_Item_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the New_Group_And_Item_MySobekViewer_Localization class </summary>
        public New_Group_And_Item_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "New_Group_And_Item_MySobekViewer";
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
                case "Above":
                    Above = Value;
                    break;

                case "ACCEPT":
                    ACCEPT = Value;
                    break;

                case "ACTION":
                    ACTION = Value;
                    break;

                case "Add A New Item For This Package":
                    AddANewItemForThisPackage = Value;
                    break;

                case "Add Another Item":
                    AddAnotherItem = Value;
                    break;

                case "An Email Has Been Sent To The Programmer Who Will Attempt To Correct Your Issue You Should Be Contacted Within The Next 2448 Hours Regarding This Issue":
                    AnEmailHasBeenSentToTheProgrammerWhoWillAttemptToCorrectYourIssueYouShouldBeContactedWithinTheNext2448HoursRegardingThisIssue = Value;
                    break;

                case "An Email Has Been Sent To You With The New Item Information":
                    AnEmailHasBeenSentToYouWithTheNewItemInformation = Value;
                    break;

                case "BACK":
                    BACK = Value;
                    break;

                case "CANCEL":
                    CANCEL = Value;
                    break;

                case "CLEAR":
                    CLEAR = Value;
                    break;

                case "CONGRATULATIONS":
                    CONGRATULATIONS = Value;
                    break;

                case "Date":
                    Date = Value;
                    break;

                case "DATE UPLOADED":
                    DATEUPLOADED = Value;
                    break;

                case "Default Metadata":
                    DefaultMetadata = Value;
                    break;

                case "Edit This Item":
                    EditThisItem = Value;
                    break;

                case "Enter A URL For This New Item":
                    EnterAURLForThisNewItem = Value;
                    break;

                case "Enter The Basic Information To Describe Your New Item":
                    EnterTheBasicInformationToDescribeYourNewItem = Value;
                    break;

                case "Enter URL":
                    EnterURL = Value;
                    break;

                case "FILENAME":
                    FILENAME = Value;
                    break;

                case "Item Description":
                    ItemDescription = Value;
                    break;

                case "My Account":
                    MyAccount = Value;
                    break;

                case "Once All Files Are Uploaded Press SUBMIT To Finish This Item":
                    OnceAllFilesAreUploadedPressSUBMITToFinishThisItem = Value;
                    break;

                case "Once Complete Press SUBMIT To Finish This Item":
                    OnceCompletePressSUBMITToFinishThisItem = Value;
                    break;

                case "Once The URL Is Entered Press SUBMIT To Finish This Item":
                    OnceTheURLIsEnteredPressSUBMITToFinishThisItem = Value;
                    break;

                case "Once You Enter Any Files Andor URL Press SUBMIT To Finish This Item":
                    OnceYouEnterAnyFilesAndorURLPressSUBMITToFinishThisItem = Value;
                    break;

                case "Ooops We Encountered A Problem":
                    OoopsWeEncounteredAProblem = Value;
                    break;

                case "Permissions Agreement":
                    PermissionsAgreement = Value;
                    break;

                case "Return To My Home":
                    ReturnToMyHome = Value;
                    break;

                case "Search Indexes May Take A Couple Minutes To Build At Which Time This Item Will Be Discoverable Through The Search Interface":
                    SearchIndexesMayTakeACoupleMinutesToBuildAtWhichTimeThisItemWillBeDiscoverableThroughTheSearchInterface = Value;
                    break;

                case "SIZE":
                    SIZE = Value;
                    break;

                case "Step 1 Of XXX Confirm Template And Project":
                    Step1OfXXXConfirmTemplateAndProject = Value;
                    break;

                case "Step 1 Of XXX Grant Of Permission":
                    Step1OfXXXGrantOfPermission = Value;
                    break;

                case "SUBMIT":
                    SUBMIT = Value;
                    break;

                case "Template":
                    Template = Value;
                    break;

                case "The Following Errors Were Detected":
                    TheFollowingErrorsWereDetected = Value;
                    break;

                case "The Following Files Are Already Uploaded For This Package And Will Be Included As Downloads":
                    TheFollowingFilesAreAlreadyUploadedForThisPackageAndWillBeIncludedAsDownloads = Value;
                    break;

                case "To Change Your Default XXX Select":
                    ToChangeYourDefaultXXXSelect = Value;
                    break;

                case "Upload Files":
                    UploadFiles = Value;
                    break;

                case "Upload Files Or Enter URL":
                    UploadFilesOrEnterURL = Value;
                    break;

                case "Upload The Related Files For Your New Item You Can Also Provide Labels For Each File Once They Are Uploaded":
                    UploadTheRelatedFilesForYourNewItemYouCanAlsoProvideLabelsForEachFileOnceTheyAreUploaded = Value;
                    break;

                case "Upload The Related Files Or Enter A URL For Your New Item You Can Also Provide Labels For Each File Once They Are Uploaded":
                    UploadTheRelatedFilesOrEnterAURLForYourNewItemYouCanAlsoProvideLabelsForEachFileOnceTheyAreUploaded = Value;
                    break;

                case "User":
                    User = Value;
                    break;

                case "View All My Submitted Items":
                    ViewAllMySubmittedItems = Value;
                    break;

                case "View This Item":
                    ViewThisItem = Value;
                    break;

                case "You May Also Change Your Current XXX For This Submission":
                    YouMayAlsoChangeYourCurrentXXXForThisSubmission = Value;
                    break;

                case "You Must Read And Accept The Above Permissions Agreement To Continue":
                    YouMustReadAndAcceptTheAbovePermissionsAgreementToContinue = Value;
                    break;

                case "You Must Read And Accept The Below Permissions To Continue":
                    YouMustReadAndAcceptTheBelowPermissionsToContinue = Value;
                    break;

                case "Your Item Has Been Successfully Added To The Digital Library And Will Appear Immediately":
                    YourItemHasBeenSuccessfullyAddedToTheDigitalLibraryAndWillAppearImmediately = Value;
                    break;

            }
        }
        /// <remarks> 'above' localization string </remarks>
        public string Above { get; private set; }

        /// <remarks> 'ACCEPT' localization string </remarks>
        public string ACCEPT { get; private set; }

        /// <remarks> 'ACTION' localization string </remarks>
        public string ACTION { get; private set; }

        /// <remarks> 'Add a new item for this package' localization string </remarks>
        public string AddANewItemForThisPackage { get; private set; }

        /// <remarks> 'Add another item' localization string </remarks>
        public string AddAnotherItem { get; private set; }

        /// <remarks> 'An email has been sent to the programmer who will attempt to correct your issue.  You should be contacted within the next 24-48 hours regarding this issue' localization string </remarks>
        public string AnEmailHasBeenSentToTheProgrammerWhoWillAttemptToCorrectYourIssueYouShouldBeContactedWithinTheNext2448HoursRegardingThisIssue { get; private set; }

        /// <remarks> 'An email has been sent to you with the new item information.' localization string </remarks>
        public string AnEmailHasBeenSentToYouWithTheNewItemInformation { get; private set; }

        /// <remarks> 'BACK' localization string </remarks>
        public string BACK { get; private set; }

        /// <remarks> 'CANCEL' localization string </remarks>
        public string CANCEL { get; private set; }

        /// <remarks> 'CLEAR' localization string </remarks>
        public string CLEAR { get; private set; }

        /// <remarks> 'CONGRATULATIONS' localization string </remarks>
        public string CONGRATULATIONS { get; private set; }

        /// <remarks> 'Date' localization string </remarks>
        public string Date { get; private set; }

        /// <remarks> 'DATE UPLOADED' localization string </remarks>
        public string DATEUPLOADED { get; private set; }

        /// <remarks> 'Default Metadata' localization string </remarks>
        public string DefaultMetadata { get; private set; }

        /// <remarks> 'Edit this item' localization string </remarks>
        public string EditThisItem { get; private set; }

        /// <remarks> 'Enter a URL for this new item.' localization string </remarks>
        public string EnterAURLForThisNewItem { get; private set; }

        /// <remarks> 'Enter the basic information to describe your new item' localization string </remarks>
        public string EnterTheBasicInformationToDescribeYourNewItem { get; private set; }

        /// <remarks> 'Enter URL' localization string </remarks>
        public string EnterURL { get; private set; }

        /// <remarks> 'FILENAME' localization string </remarks>
        public string FILENAME { get; private set; }

        /// <remarks> 'Item Description' localization string </remarks>
        public string ItemDescription { get; private set; }

        /// <remarks> 'My Account' localization string </remarks>
        public string MyAccount { get; private set; }

        /// <remarks> '"Once all files are uploaded, press SUBMIT to finish this item"' localization string </remarks>
        public string OnceAllFilesAreUploadedPressSUBMITToFinishThisItem { get; private set; }

        /// <remarks> '"Once complete, press SUBMIT to finish this item"' localization string </remarks>
        public string OnceCompletePressSUBMITToFinishThisItem { get; private set; }

        /// <remarks> '"Once the URL is entered, press SUBMIT to finish this item."' localization string </remarks>
        public string OnceTheURLIsEnteredPressSUBMITToFinishThisItem { get; private set; }

        /// <remarks> '"Once you enter any files and/or URL, press SUBMIT to finish this item"' localization string </remarks>
        public string OnceYouEnterAnyFilesAndorURLPressSUBMITToFinishThisItem { get; private set; }

        /// <remarks> 'Ooops!! We encountered a problem!' localization string </remarks>
        public string OoopsWeEncounteredAProblem { get; private set; }

        /// <remarks> 'Permissions Agreement' localization string </remarks>
        public string PermissionsAgreement { get; private set; }

        /// <remarks> 'Return to my home' localization string </remarks>
        public string ReturnToMyHome { get; private set; }

        /// <remarks> '"Search indexes may take a couple minutes to build, at which time this item will be discoverable through the search interface"' localization string </remarks>
        public string SearchIndexesMayTakeACoupleMinutesToBuildAtWhichTimeThisItemWillBeDiscoverableThroughTheSearchInterface { get; private set; }

        /// <remarks> 'SIZE' localization string </remarks>
        public string SIZE { get; private set; }

        /// <remarks> 'Step 1 of %1: Confirm Template and Project ' localization string </remarks>
        public string Step1OfXXXConfirmTemplateAndProject { get; private set; }

        /// <remarks> 'Step 1 of %1: Grant of Permission' localization string </remarks>
        public string Step1OfXXXGrantOfPermission { get; private set; }

        /// <remarks> 'SUBMIT' localization string </remarks>
        public string SUBMIT { get; private set; }

        /// <remarks> 'Template' localization string </remarks>
        public string Template { get; private set; }

        /// <remarks> 'The following errors were detected:' localization string </remarks>
        public string TheFollowingErrorsWereDetected { get; private set; }

        /// <remarks> 'The following files are already uploaded for this package and will be included as downloads' localization string </remarks>
        public string TheFollowingFilesAreAlreadyUploadedForThisPackageAndWillBeIncludedAsDownloads { get; private set; }

        /// <remarks> '"To change your default %1, select"' localization string </remarks>
        public string ToChangeYourDefaultXXXSelect { get; private set; }

        /// <remarks> 'Upload Files' localization string </remarks>
        public string UploadFiles { get; private set; }

        /// <remarks> 'Upload Files or Enter URL' localization string </remarks>
        public string UploadFilesOrEnterURL { get; private set; }

        /// <remarks> '"Upload the related files for your new item.  You can also provide labels for each file, once they are uploaded."' localization string </remarks>
        public string UploadTheRelatedFilesForYourNewItemYouCanAlsoProvideLabelsForEachFileOnceTheyAreUploaded { get; private set; }

        /// <remarks> '"Upload the related files or enter a URL for your new item.  You can also provide labels for each file, once they are uploaded"' localization string </remarks>
        public string UploadTheRelatedFilesOrEnterAURLForYourNewItemYouCanAlsoProvideLabelsForEachFileOnceTheyAreUploaded { get; private set; }

        /// <remarks> 'User' localization string </remarks>
        public string User { get; private set; }

        /// <remarks> 'View all my submitted items' localization string </remarks>
        public string ViewAllMySubmittedItems { get; private set; }

        /// <remarks> 'View this item' localization string </remarks>
        public string ViewThisItem { get; private set; }

        /// <remarks> 'You may also change your current %1 for this submission.' localization string </remarks>
        public string YouMayAlsoChangeYourCurrentXXXForThisSubmission { get; private set; }

        /// <remarks> 'You must read and accept the above permissions agreement to continue.' localization string </remarks>
        public string YouMustReadAndAcceptTheAbovePermissionsAgreementToContinue { get; private set; }

        /// <remarks> 'You must read and accept the below permissions to continue.' localization string </remarks>
        public string YouMustReadAndAcceptTheBelowPermissionsToContinue { get; private set; }

        /// <remarks> 'Your item has been successfully added to the digital library and will appear immediately.' localization string </remarks>
        public string YourItemHasBeenSuccessfullyAddedToTheDigitalLibraryAndWillAppearImmediately { get; private set; }

    }
}
