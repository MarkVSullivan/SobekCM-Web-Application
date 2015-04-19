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
                case "My Library":
                    MyLibrary = Value;
                    break;

                case "Move Item Between Bookshelves":
                    MoveItemBetweenBookshelves = Value;
                    break;

                case "Select New Bookshelf For This Item":
                    SelectNewBookshelfForThisItem = Value;
                    break;

                case "Bookshelf":
                    Bookshelf = Value;
                    break;

                case "Addedit Notes For Bookshelf Item":
                    AddeditNotesForBookshelfItem = Value;
                    break;

                case "Enter Notes For This Item In Your Bookshelf":
                    EnterNotesForThisItemInYourBookshelf = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

                case "New Bookshelf":
                    NewBookshelf = Value;
                    break;

                case "Enter The Information For Your New Bookshelf":
                    EnterTheInformationForYourNewBookshelf = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "Parent":
                    Parent = Value;
                    break;

                case "Manage My Library":
                    ManageMyLibrary = Value;
                    break;

                case "View My Collections Home Page":
                    ViewMyCollectionsHomePage = Value;
                    break;

                case "My Collections Home":
                    MyCollectionsHome = Value;
                    break;

                case "My Saved Searches":
                    MySavedSearches = Value;
                    break;

                case "View My Saved Searches":
                    ViewMySavedSearches = Value;
                    break;

                case "This Bookshelf Is Currently Empty":
                    ThisBookshelfIsCurrentlyEmpty = Value;
                    break;

                case "Manage My Bookshelves":
                    ManageMyBookshelves = Value;
                    break;

                case "Click To Add A New Bookshelf":
                    ClickToAddANewBookshelf = Value;
                    break;

                case "Add New Bookshelf":
                    AddNewBookshelf = Value;
                    break;

                case "Refresh Bookshelves List":
                    RefreshBookshelvesList = Value;
                    break;

                case "BOOKSHELF NAME":
                    BOOKSHELFNAME = Value;
                    break;

                case "Click To Delete This Bookshelf":
                    ClickToDeleteThisBookshelf = Value;
                    break;

                case "Make This Bookshelf Private":
                    MakeThisBookshelfPrivate = Value;
                    break;

                case "Make This Bookshelf Public":
                    MakeThisBookshelfPublic = Value;
                    break;

                case "Click To Manage This Bookshelf":
                    ClickToManageThisBookshelf = Value;
                    break;

                case "You Cannot Delete Bookshelves Which Contain Other Bookshelves":
                    YouCannotDeleteBookshelvesWhichContainOther = Value;
                    break;

                case "You Cannot Delete Your Last Bookshelf":
                    YouCannotDeleteYourLastBookshelf = Value;
                    break;

                case "Make Private":
                    MakePrivate = Value;
                    break;

                case "Make Public":
                    MakePublic = Value;
                    break;

                case "Manage":
                    Manage = Value;
                    break;

                case "Public Folder":
                    PublicFolder = Value;
                    break;

                case "Enter The Email Information Below":
                    EnterTheEmailInformationBelow = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "Format":
                    Format = Value;
                    break;

                case "CLOSE":
                    CLOSE = Value;
                    break;

                case "ACTIONS":
                    ACTIONS = Value;
                    break;

            }
        }
        /// <remarks> 'My Library' localization string </remarks>
        public string MyLibrary { get; private set; }

        /// <remarks> 'Move Item Between Bookshelves' localization string </remarks>
        public string MoveItemBetweenBookshelves { get; private set; }

        /// <remarks> 'Select new bookshelf for this item' localization string </remarks>
        public string SelectNewBookshelfForThisItem { get; private set; }

        /// <remarks> 'Bookshelf:' localization string </remarks>
        public string Bookshelf { get; private set; }

        /// <remarks> 'Add/Edit Notes for Bookshelf Item' localization string </remarks>
        public string AddeditNotesForBookshelfItem { get; private set; }

        /// <remarks> 'Enter notes for this item in your bookshelf' localization string </remarks>
        public string EnterNotesForThisItemInYourBookshelf { get; private set; }

        /// <remarks> 'Notes:' localization string </remarks>
        public string Notes { get; private set; }

        /// <remarks> 'New Bookshelf' localization string </remarks>
        public string NewBookshelf { get; private set; }

        /// <remarks> 'Enter the information for your new bookshelf' localization string </remarks>
        public string EnterTheInformationForYourNewBookshelf { get; private set; }

        /// <remarks> 'Name:' localization string </remarks>
        public string Name { get; private set; }

        /// <remarks> 'Parent:' localization string </remarks>
        public string Parent { get; private set; }

        /// <remarks> 'Manage my library' localization string </remarks>
        public string ManageMyLibrary { get; private set; }

        /// <remarks> 'View my collections home page' localization string </remarks>
        public string ViewMyCollectionsHomePage { get; private set; }

        /// <remarks> 'My Collections Home' localization string </remarks>
        public string MyCollectionsHome { get; private set; }

        /// <remarks> 'My Saved Searches' localization string </remarks>
        public string MySavedSearches { get; private set; }

        /// <remarks> 'View my saved searches' localization string </remarks>
        public string ViewMySavedSearches { get; private set; }

        /// <remarks> 'This bookshelf is currently empty' localization string </remarks>
        public string ThisBookshelfIsCurrentlyEmpty { get; private set; }

        /// <remarks> 'Manage My Bookshelves' localization string </remarks>
        public string ManageMyBookshelves { get; private set; }

        /// <remarks> 'Click to add a new bookshelf' localization string </remarks>
        public string ClickToAddANewBookshelf { get; private set; }

        /// <remarks> 'Add New Bookshelf' localization string </remarks>
        public string AddNewBookshelf { get; private set; }

        /// <remarks> 'Refresh Bookshelves List' localization string </remarks>
        public string RefreshBookshelvesList { get; private set; }

        /// <remarks> 'BOOKSHELF NAME' localization string </remarks>
        public string BOOKSHELFNAME { get; private set; }

        /// <remarks> 'Click to delete this bookshelf' localization string </remarks>
        public string ClickToDeleteThisBookshelf { get; private set; }

        /// <remarks> 'Make this bookshelf private' localization string </remarks>
        public string MakeThisBookshelfPrivate { get; private set; }

        /// <remarks> 'Make this bookshelf public' localization string </remarks>
        public string MakeThisBookshelfPublic { get; private set; }

        /// <remarks> 'Click to manage this bookshelf' localization string </remarks>
        public string ClickToManageThisBookshelf { get; private set; }

        /// <remarks> 'You cannot delete bookshelves which contain other bookshelves' localization string </remarks>
        public string YouCannotDeleteBookshelvesWhichContainOther { get; private set; }

        /// <remarks> 'You cannot delete your last bookshelf' localization string </remarks>
        public string YouCannotDeleteYourLastBookshelf { get; private set; }

        /// <remarks> 'make private' localization string </remarks>
        public string MakePrivate { get; private set; }

        /// <remarks> 'make public' localization string </remarks>
        public string MakePublic { get; private set; }

        /// <remarks> 'manage' localization string </remarks>
        public string Manage { get; private set; }

        /// <remarks> 'Public folder' localization string </remarks>
        public string PublicFolder { get; private set; }

        /// <remarks> 'Enter the email information below' localization string </remarks>
        public string EnterTheEmailInformationBelow { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> 'Comments' localization string </remarks>
        public string Comments { get; private set; }

        /// <remarks> 'Format:' localization string </remarks>
        public string Format { get; private set; }

        /// <remarks> 'CLOSE' localization string </remarks>
        public string CLOSE { get; private set; }

        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

    }
}
