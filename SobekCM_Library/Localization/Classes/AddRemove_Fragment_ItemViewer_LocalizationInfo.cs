namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the AddRemove_Fragment_ItemViewer class </summary>
    public class AddRemove_Fragment_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the AddRemove_Fragment_ItemViewer_Localization class </summary>
        public AddRemove_Fragment_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "AddRemove_Fragment_ItemViewer";
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
                case "Add This Item To Your Bookshelf":
                    AddThisItemToYourBookshelf = Value;
                    break;

                case "Enter Notes For This Item In Your Bookshelf":
                    EnterNotesForThisItemInYourBookshelf = Value;
                    break;

                case "Bookshelf":
                    Bookshelf = Value;
                    break;

                case "My Bookshelf":
                    MyBookshelf = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

                case "Open Bookshelf In New Window":
                    OpenBookshelfInNewWindow = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "Save":
                    Save = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

            }
        }
        /// <remarks> 'Add this Item to your Bookshelf' localization string </remarks>
        public string AddThisItemToYourBookshelf { get; private set; }

        /// <remarks> 'Enter notes for this item in your bookshelf' localization string </remarks>
        public string EnterNotesForThisItemInYourBookshelf { get; private set; }

        /// <remarks> 'Bookshelf:' localization string </remarks>
        public string Bookshelf { get; private set; }

        /// <remarks> 'My Bookshelf' localization string </remarks>
        public string MyBookshelf { get; private set; }

        /// <remarks> 'Notes:' localization string </remarks>
        public string Notes { get; private set; }

        /// <remarks> 'Open bookshelf in new window' localization string </remarks>
        public string OpenBookshelfInNewWindow { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> 'Save' localization string </remarks>
        public string Save { get; private set; }

        /// <remarks> 'Send' localization string </remarks>
        public string Send { get; private set; }

    }
}
