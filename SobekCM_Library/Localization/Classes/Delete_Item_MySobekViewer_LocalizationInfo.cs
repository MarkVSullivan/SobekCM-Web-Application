namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Delete_Item_MySobekViewer class </summary>
    public class Delete_Item_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Delete_Item_MySobekViewer_Localization class </summary>
        public Delete_Item_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Delete_Item_MySobekViewer";
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
                case "Delete Item":
                    DeleteItem = Value;
                    break;

                case "Enter DELETE In The Textbox Below And Select GO To Complete This Deletion":
                    EnterDELETEInTheTextboxBelowAndSelectGOTo = Value;
                    break;

                case "Confirm Delete Of This Item":
                    ConfirmDeleteOfThisItem = Value;
                    break;

                case "DELETE SUCCESSFUL":
                    DELETESUCCESSFUL = Value;
                    break;

                case "DELETE FAILED":
                    DELETEFAILED = Value;
                    break;

                case "Insufficient User Permissions To Perform Delete":
                    InsufficientUserPermissionsToPerformDelete = Value;
                    break;

                case "Item Indicated Does Not Exists":
                    ItemIndicatedDoesNotExists = Value;
                    break;

                case "Error While Performing Delete In Database":
                    ErrorWhilePerformingDeleteInDatabase = Value;
                    break;

                case "DELETE PARTIALLY SUCCESSFUL":
                    DELETEPARTIALLYSUCCESSFUL = Value;
                    break;

                case "Unable To Move All Files To The RECYCLE BIN Folder":
                    UnableToMoveAllFilesToTheRECYCLEBINFolder = Value;
                    break;

            }
        }
        /// <remarks> 'Delete Item' localization string </remarks>
        public string DeleteItem { get; private set; }

        /// <remarks> 'Enter DELETE in the textbox below and select GO to complete this deletion.' localization string </remarks>
        public string EnterDELETEInTheTextboxBelowAndSelectGOTo { get; private set; }

        /// <remarks> 'Confirm delete of this item' localization string </remarks>
        public string ConfirmDeleteOfThisItem { get; private set; }

        /// <remarks> 'DELETE SUCCESSFUL' localization string </remarks>
        public string DELETESUCCESSFUL { get; private set; }

        /// <remarks> 'DELETE FAILED' localization string </remarks>
        public string DELETEFAILED { get; private set; }

        /// <remarks> 'Insufficient user permissions to perform delete' localization string </remarks>
        public string InsufficientUserPermissionsToPerformDelete { get; private set; }

        /// <remarks> 'Item indicated does not exists' localization string </remarks>
        public string ItemIndicatedDoesNotExists { get; private set; }

        /// <remarks> 'Error while performing delete in database' localization string </remarks>
        public string ErrorWhilePerformingDeleteInDatabase { get; private set; }

        /// <remarks> 'DELETE PARTIALLY SUCCESSFUL' localization string </remarks>
        public string DELETEPARTIALLYSUCCESSFUL { get; private set; }

        /// <remarks> 'Unable to move all files to the RECYCLE BIN folder' localization string </remarks>
        public string UnableToMoveAllFilesToTheRECYCLEBINFolder { get; private set; }

    }
}
