namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Group_Add_Volume_MySobekViewer class </summary>
    public class Group_Add_Volume_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Group_Add_Volume_MySobekViewer_Localization class </summary>
        public Group_Add_Volume_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Group_Add_Volume_MySobekViewer";
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
                case "No Base Volume Selected":
                    NoBaseVolumeSelected = Value;
                    break;

                case "EXCEPTION CAUGHT":
                    EXCEPTIONCAUGHT = Value;
                    break;

                case "NEW VOLUME":
                    NEWVOLUME = Value;
                    break;

                case "Add A New Volume To This Existing Titleitem Group":
                    AddANewVolumeToThisExistingTitleitemGroup = Value;
                    break;

                case "Only Enter Data That You Wish To Override The Data In The Existing Base Volume":
                    OnlyEnterDataThatYouWishToOverrideTheData = Value;
                    break;

                case "Click":
                    Click = Value;
                    break;

                case "Here For Detailed Instructions":
                    HereForDetailedInstructions = Value;
                    break;

                case "In Addition The Following Actions Are Available":
                    InAdditionTheFollowingActionsAreAvailable = Value;
                    break;

                case "SAVE EDIT ITEM":
                    SAVEEDITITEM = Value;
                    break;

                case "SAVE ADD FILES":
                    SAVEADDFILES = Value;
                    break;

                case "SAVE ADD ANOTHER":
                    SAVEADDANOTHER = Value;
                    break;

            }
        }
        /// <remarks> 'No base volume selected!' localization string </remarks>
        public string NoBaseVolumeSelected { get; private set; }

        /// <remarks> 'EXCEPTION CAUGHT!' localization string </remarks>
        public string EXCEPTIONCAUGHT { get; private set; }

        /// <remarks> 'NEW VOLUME' localization string </remarks>
        public string NEWVOLUME { get; private set; }

        /// <remarks> 'Add a new volume to this existing title/item group' localization string </remarks>
        public string AddANewVolumeToThisExistingTitleitemGroup { get; private set; }

        /// <remarks> 'Only enter data that you wish to override the data in the existing base volume.' localization string </remarks>
        public string OnlyEnterDataThatYouWishToOverrideTheData { get; private set; }

        /// <remarks> 'Click' localization string </remarks>
        public string Click { get; private set; }

        /// <remarks> 'here for detailed instructions' localization string </remarks>
        public string HereForDetailedInstructions { get; private set; }

        /// <remarks> '"In addition, the following actions are available:"' localization string </remarks>
        public string InAdditionTheFollowingActionsAreAvailable { get; private set; }

        /// <remarks> 'SAVE &amp; EDIT ITEM' localization string </remarks>
        public string SAVEEDITITEM { get; private set; }

        /// <remarks> 'SAVE &amp; ADD FILES' localization string </remarks>
        public string SAVEADDFILES { get; private set; }

        /// <remarks> 'SAVE &amp; ADD ANOTHER' localization string </remarks>
        public string SAVEADDANOTHER { get; private set; }

    }
}
