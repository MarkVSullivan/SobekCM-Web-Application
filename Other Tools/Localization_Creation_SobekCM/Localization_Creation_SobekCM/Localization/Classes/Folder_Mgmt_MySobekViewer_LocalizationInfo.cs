namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Folder_Mgmt_MySobekViewer class </summary>
    public class Folder_Mgmt_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Folder_Mgmt_MySobekViewer_Localization class </summary>
        public Folder_Mgmt_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Folder_Mgmt_MySobekViewer";
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
                case "ACTIONS":
                    ACTIONS = Value;
                    break;

                case "Add New Bookshelf":
                    AddNewBookshelf = Value;
                    break;

                case "Addedit Notes For Bookshelf Item":
                    AddeditNotesForBookshelfItem = Value;
                    break;

                case "BOOKSHELF NAME":
                    BOOKSHELFNAME = Value;
                    break;

                case "Bookshelf":
                    Bookshelf = Value;
                    break;

                case "Click To Add A New Bookshelf":
                    ClickToAddANewBookshelf = Value;
                    break;

                case "Click To Delete This Bookshelf":
                    ClickToDeleteThisBookshelf = Value;
                    break;

                case "Click To Manage This Bookshelf":
                    ClickToManageThisBookshelf = Value;
                    break;

                case "CLOSE":
                    CLOSE = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "Enter Notes For This Item In Your Bookshelf":
                    EnterNotesForThisItemInYourBookshelf = Value;
                    break;

                case "Enter The Email Information Below":
                    EnterTheEmailInformationBelow = Value;
                    break;

                case "Enter The Information For Your New Bookshelf":
                    EnterTheInformationForYourNewBookshelf = Value;
                    break;

                case "Format":
                    Format = Value;
                    break;

                case "Make Private":
                    MakePrivate = Value;
                    break;

                case "Make Public":
                    MakePublic = Value;
                    break;

                case "Make This Bookshelf Private":
                    MakeThisBookshelfPrivate = Value;
                    break;

                case "Make This Bookshelf Public":
                    MakeThisBookshelfPublic = Value;
                    break;

                case "Manage":
                    Manage = Value;
                    break;

                case "Manage My Bookshelves":
                    ManageMyBookshelves = Value;
                    break;

                case "Manage My Library":
                    ManageMyLibrary = Value;
                    break;

                case "Move Item Between Bookshelves":
                    MoveItemBetweenBookshelves = Value;
                    break;

                case "My Collections Home":
                    MyCollectionsHome = Value;
                    break;

                case "My Library":
                    MyLibrary = Value;
                    break;

                case "My Saved Searches":
                    MySavedSearches = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "New Bookshelf":
                    NewBookshelf = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

                case "Parent":
                    Parent = Value;
                    break;

                case "Public Folder":
                    PublicFolder = Value;
                    break;

                case "Refresh Bookshelves List":
                    RefreshBookshelvesList = Value;
                    break;

                case "Select New Bookshelf For This Item":
                    SelectNewBookshelfForThisItem = Value;
                    break;

                case "This Bookshelf Is Currently Empty":
                    ThisBookshelfIsCurrentlyEmpty = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "View My Collections Home Page":
                    ViewMyCollectionsHomePage = Value;
                    break;

                case "View My Saved Searches":
                    ViewMySavedSearches = Value;
                    break;

                case "You Cannot Delete Bookshelves Which Contain Other Bookshelves":
                    YouCannotDeleteBookshelvesWhichContainOtherBookshelves = Value;
                    break;

                case "You Cannot Delete Your Last Bookshelf":
                    YouCannotDeleteYourLastBookshelf = Value;
                    break;

            }
        }
        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

        /// <remarks> Link to add a new bookshelf </remarks>
        public string AddNewBookshelf { get; private set; }

        /// <remarks> Title for the pop-up form to add/edit user notes assigned to an item </remarks>
        public string AddeditNotesForBookshelfItem { get; private set; }

        /// <remarks> Header text for the table of all bookshelves a user has </remarks>
        public string BOOKSHELFNAME { get; private set; }

        /// <remarks> Prompt for the new bookshelf when moving between bookshelves from pop-up form. </remarks>
        public string Bookshelf { get; private set; }

        /// <remarks> Hover-over text for adding a new bookshelf </remarks>
        public string ClickToAddANewBookshelf { get; private set; }

        /// <remarks> Hover-over text for the delete action </remarks>
        public string ClickToDeleteThisBookshelf { get; private set; }

        /// <remarks> Hover-over text for the 'manage' action </remarks>
        public string ClickToManageThisBookshelf { get; private set; }

        /// <remarks> 'CLOSE' localization string </remarks>
        public string CLOSE { get; private set; }

        /// <remarks> 'Comments' localization string </remarks>
        public string Comments { get; private set; }

        /// <remarks> Prompt for the pop-up form to add/edit user notes assigned to an item </remarks>
        public string EnterNotesForThisItemInYourBookshelf { get; private set; }

        /// <remarks> 'Enter the email information below' localization string </remarks>
        public string EnterTheEmailInformationBelow { get; private set; }

        /// <remarks> Prompt for the pop-up form for creating a new bookshelf </remarks>
        public string EnterTheInformationForYourNewBookshelf { get; private set; }

        /// <remarks> 'Format:' localization string </remarks>
        public string Format { get; private set; }

        /// <remarks> Link text for making a folder private </remarks>
        public string MakePrivate { get; private set; }

        /// <remarks> Link text for making a folder public </remarks>
        public string MakePublic { get; private set; }

        /// <remarks> Hover-over text for the 'make private' action </remarks>
        public string MakeThisBookshelfPrivate { get; private set; }

        /// <remarks> Hover-over text for the 'make public' action </remarks>
        public string MakeThisBookshelfPublic { get; private set; }

        /// <remarks> Link text for managing the items within a bookshelf </remarks>
        public string Manage { get; private set; }

        /// <remarks> Link to manage your bookshelves </remarks>
        public string ManageMyBookshelves { get; private set; }

        /// <remarks> Top-level text for managing your library </remarks>
        public string ManageMyLibrary { get; private set; }

        /// <remarks> Title for the pop-up form which allows you to move an item to another bookshelf </remarks>
        public string MoveItemBetweenBookshelves { get; private set; }

        /// <remarks> Link for viewing your personalized home page  </remarks>
        public string MyCollectionsHome { get; private set; }

        /// <remarks> Title for the folder management page </remarks>
        public string MyLibrary { get; private set; }

        /// <remarks> Link for viewing saved searches </remarks>
        public string MySavedSearches { get; private set; }

        /// <remarks> Used on the pop-up form for creating a new bookshelf </remarks>
        public string Name { get; private set; }

        /// <remarks> Title for the pop-up form for creating a new bookshelf </remarks>
        public string NewBookshelf { get; private set; }

        /// <remarks> Prompt for the new notes on the pop-up form for adding/editing user notes assigned to an item </remarks>
        public string Notes { get; private set; }

        /// <remarks> Used on the pop-up form for creating a new bookshelf </remarks>
        public string Parent { get; private set; }

        /// <remarks> Hover-over text for a folder which is public (to help explain the icon) </remarks>
        public string PublicFolder { get; private set; }

        /// <remarks> "Prompt in case there is some problem, or user has the bookshelves open in an alternate window" </remarks>
        public string RefreshBookshelvesList { get; private set; }

        /// <remarks> Prompt on the pop-up form which allows you to move an item to another bookshelf </remarks>
        public string SelectNewBookshelfForThisItem { get; private set; }

        /// <remarks> Message if the bookshelf a user is managing is empty </remarks>
        public string ThisBookshelfIsCurrentlyEmpty { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> Hover-over text for viewing your personalized home page </remarks>
        public string ViewMyCollectionsHomePage { get; private set; }

        /// <remarks> Hover-over text for viewing saved searches </remarks>
        public string ViewMySavedSearches { get; private set; }

        /// <remarks> Warning message if you try to delete a bookshelf which contains other bookshelves </remarks>
        public string YouCannotDeleteBookshelvesWhichContainOtherBookshelves { get; private set; }

        /// <remarks> Warning message if you try to delete your last bookshelf </remarks>
        public string YouCannotDeleteYourLastBookshelf { get; private set; }

    }
}
