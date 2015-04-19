namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Edit_Item_Behaviors_MySobekViewer class </summary>
    public class Edit_Item_Behaviors_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Edit_Item_Behaviors_MySobekViewer_Localization class </summary>
        public Edit_Item_Behaviors_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Edit_Item_Behaviors_MySobekViewer";
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
                case "Edit Item Behaviors":
                    EditItemBehaviors = Value;
                    break;

                case "Edit This Items Behaviors Within This Library":
                    EditThisItemsBehaviorsWithinThisLibrary = Value;
                    break;

                case "Enter The Data For This Item Below And Press The SAVE Button When All Your Edits Are Complete":
                    EnterTheDataForThisItemBelowAndPressTheS = Value;
                    break;

                case "Clicking On The Green Plus Button":
                    ClickingOnTheGreenPlusButton = Value;
                    break;

                case "Will Add Another Instance Of The Element If The Element Is Repeatable":
                    WillAddAnotherInstanceOfTheElementIfTheEl = Value;
                    break;

                case "Click":
                    Click = Value;
                    break;

                case "Here For Detailed Instructions":
                    HereForDetailedInstructions = Value;
                    break;

                case "On Editing Behaviors Online":
                    OnEditingBehaviorsOnline = Value;
                    break;

            }
        }
        /// <remarks> 'Edit Item Behaviors' localization string </remarks>
        public string EditItemBehaviors { get; private set; }

        /// <remarks> 'Edit this item's behaviors within this library' localization string </remarks>
        public string EditThisItemsBehaviorsWithinThisLibrary { get; private set; }

        /// <remarks> 'Enter the data for this item below and press the SAVE button when all your edits are complete.' localization string </remarks>
        public string EnterTheDataForThisItemBelowAndPressTheS { get; private set; }

        /// <remarks> 'Clicking on the green plus button ( ' localization string </remarks>
        public string ClickingOnTheGreenPlusButton { get; private set; }

        /// <remarks> '") will add another instance of the element, if the element is repeatable."' localization string </remarks>
        public string WillAddAnotherInstanceOfTheElementIfTheEl { get; private set; }

        /// <remarks> 'Click' localization string </remarks>
        public string Click { get; private set; }

        /// <remarks> 'here for detailed instructions' localization string </remarks>
        public string HereForDetailedInstructions { get; private set; }

        /// <remarks> 'on editing behaviors online' localization string </remarks>
        public string OnEditingBehaviorsOnline { get; private set; }

    }
}
