namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Bookshelf_View_ResultsViewer class </summary>
    public class Bookshelf_View_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Bookshelf_View_ResultsViewer_Localization class </summary>
        public Bookshelf_View_ResultsViewer_LocalizationInfo() : base()
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
                case "ACTIONS":
                    ACTIONS = Value;
                    break;

                case "Add Note":
                    AddNote = Value;
                    break;

                case "Edit":
                    Edit = Value;
                    break;

                case "Edit Note":
                    EditNote = Value;
                    break;

                case "Edit This Item":
                    EditThisItem = Value;
                    break;

                case "Move":
                    Move = Value;
                    break;

                case "Move All Checked Items To A New Bookshelf":
                    MoveAllCheckedItemsToANewBookshelf = Value;
                    break;

                case "Move This Item To A New Bookshelf":
                    MoveThisItemToANewBookshelf = Value;
                    break;

                case "Remove":
                    Remove = Value;
                    break;

                case "Remove All Checked Items From Your Bookshelf":
                    RemoveAllCheckedItemsFromYourBookshelf = Value;
                    break;

                case "Remove This Item From Your Bookshelf":
                    RemoveThisItemFromYourBookshelf = Value;
                    break;

                case "Select Or Unselect All Items In This Bookshelf":
                    SelectOrUnselectAllItemsInThisBookshelf = Value;
                    break;

                case "Select Or Unselect This Item":
                    SelectOrUnselectThisItem = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

                case "Send This Item To A Friend":
                    SendThisItemToAFriend = Value;
                    break;

                case "TITLE NOTES":
                    TITLENOTES = Value;
                    break;

            }
        }
        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

        /// <remarks> Text for the link to add a new note to an item in your bookshelf </remarks>
        public string AddNote { get; private set; }

        /// <remarks> Text for the edit action in the bookshelf view </remarks>
        public string Edit { get; private set; }

        /// <remarks> Text for the link to edit an existing note on an item in your bookshelf </remarks>
        public string EditNote { get; private set; }

        /// <remarks> Hover over text for the edit action in the bookshelf view </remarks>
        public string EditThisItem { get; private set; }

        /// <remarks> Text for the move action in the bookshelf view </remarks>
        public string Move { get; private set; }

        /// <remarks> Hover over text for the bulk move action in the bookshelf view </remarks>
        public string MoveAllCheckedItemsToANewBookshelf { get; private set; }

        /// <remarks> Hover over text for the move action in the bookshelf view </remarks>
        public string MoveThisItemToANewBookshelf { get; private set; }

        /// <remarks> Text for the remove action in the bookshelf view </remarks>
        public string Remove { get; private set; }

        /// <remarks> Hover over text for the bulk remove action in the bookshelf view </remarks>
        public string RemoveAllCheckedItemsFromYourBookshelf { get; private set; }

        /// <remarks> Hover over text for the remove action in the bookshelf view </remarks>
        public string RemoveThisItemFromYourBookshelf { get; private set; }

        /// <remarks> Hover over for button to select or unselect all items in the bookshelf </remarks>
        public string SelectOrUnselectAllItemsInThisBookshelf { get; private set; }

        /// <remarks> Hover over text for the checkbox which selects or deselects an item for bulk actions </remarks>
        public string SelectOrUnselectThisItem { get; private set; }

        /// <remarks> Text for the send action in the bookshelf view </remarks>
        public string Send { get; private set; }

        /// <remarks> Hover over text for the send action in the bookshelf view </remarks>
        public string SendThisItemToAFriend { get; private set; }

        /// <remarks> Table header for the title/notes column </remarks>
        public string TITLENOTES { get; private set; }

    }
}
