namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Bookshelf_View_ResultsViewer class </summary>
    public class Bookshelf_View_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Bookshelf_View_ResultsViewer_Localization class </summary>
        public Bookshelf_View_ResultsViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Bookshelf_View_ResultsViewer";
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
                case "TITLE NOTES":
                    TITLENOTES = Value;
                    break;

                case "Remove All Checked Items From Your Bookshelf":
                    RemoveAllCheckedItemsFromYourBookshelf = Value;
                    break;

                case "Remove":
                    Remove = Value;
                    break;

                case "Move All Checked Items To A New Bookshelf":
                    MoveAllCheckedItemsToANewBookshelf = Value;
                    break;

                case "Move":
                    Move = Value;
                    break;

                case "Select Or Unselect All Items In This Bookshelf":
                    SelectOrUnselectAllItemsInThisBookshelf = Value;
                    break;

                case "Remove This Item From Your Bookshelf":
                    RemoveThisItemFromYourBookshelf = Value;
                    break;

                case "Move This Item To A New Bookshelf":
                    MoveThisItemToANewBookshelf = Value;
                    break;

                case "Send This Item To A Friend":
                    SendThisItemToAFriend = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

                case "Edit This Item":
                    EditThisItem = Value;
                    break;

                case "Edit":
                    Edit = Value;
                    break;

                case "Select Or Unselect This Item":
                    SelectOrUnselectThisItem = Value;
                    break;

                case "Edit Note":
                    EditNote = Value;
                    break;

                case "Add Note":
                    AddNote = Value;
                    break;

                case "ACTIONS":
                    ACTIONS = Value;
                    break;

            }
        }
        /// <remarks> 'TITLE / NOTES' localization string </remarks>
        public string TITLENOTES { get; private set; }

        /// <remarks> 'Remove all checked items from your bookshelf' localization string </remarks>
        public string RemoveAllCheckedItemsFromYourBookshelf { get; private set; }

        /// <remarks> 'remove' localization string </remarks>
        public string Remove { get; private set; }

        /// <remarks> 'Move all checked items to a new bookshelf' localization string </remarks>
        public string MoveAllCheckedItemsToANewBookshelf { get; private set; }

        /// <remarks> 'move' localization string </remarks>
        public string Move { get; private set; }

        /// <remarks> 'Select or unselect all items in this bookshelf' localization string </remarks>
        public string SelectOrUnselectAllItemsInThisBookshelf { get; private set; }

        /// <remarks> 'Remove this item from your bookshelf' localization string </remarks>
        public string RemoveThisItemFromYourBookshelf { get; private set; }

        /// <remarks> 'Move this item to a new bookshelf' localization string </remarks>
        public string MoveThisItemToANewBookshelf { get; private set; }

        /// <remarks> 'Send this item to a friend' localization string </remarks>
        public string SendThisItemToAFriend { get; private set; }

        /// <remarks> 'send' localization string </remarks>
        public string Send { get; private set; }

        /// <remarks> 'Edit this item' localization string </remarks>
        public string EditThisItem { get; private set; }

        /// <remarks> 'edit' localization string </remarks>
        public string Edit { get; private set; }

        /// <remarks> 'Select or unselect this item' localization string </remarks>
        public string SelectOrUnselectThisItem { get; private set; }

        /// <remarks> 'edit note' localization string </remarks>
        public string EditNote { get; private set; }

        /// <remarks> 'add note' localization string </remarks>
        public string AddNote { get; private set; }

        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

    }
}
