namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Edit_Group_Behaviors_MySobekViewer class </summary>
    public class Edit_Group_Behaviors_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Edit_Group_Behaviors_MySobekViewer_Localization class </summary>
        public Edit_Group_Behaviors_MySobekViewer_LocalizationInfo() : base()
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
                case "Will Add Another Instance Of The Element If The Element Is Repeatable":
                    WillAddAnotherInstanceOfTheElementIfTheElementIsRepeatable = Value;
                    break;

                case "On Editing Behaviors Online":
                    OnEditingBehaviorsOnline = Value;
                    break;

                case "Click":
                    Click = Value;
                    break;

                case "Clicking On The Green Plus Button":
                    ClickingOnTheGreenPlusButton = Value;
                    break;

                case "Edit The Behaviors Associated With This Item Group Within This Library":
                    EditTheBehaviorsAssociatedWithThisItemGroupWithinThisLibrary = Value;
                    break;

                case "Enter The Data For This Item Group Below And Press The SAVE Button When All Your Edits Are Complete":
                    EnterTheDataForThisItemGroupBelowAndPressTheSAVEButtonWhenAllYourEditsAreComplete = Value;
                    break;

                case "Here For Detailed Instructions":
                    HereForDetailedInstructions = Value;
                    break;

            }
        }
        /// <remarks> '" ) will add another instance of the element, if the element is repeatable."' localization string </remarks>
        public string WillAddAnotherInstanceOfTheElementIfTheElementIsRepeatable { get; private set; }

        /// <remarks> ' on editing behaviors online.' localization string </remarks>
        public string OnEditingBehaviorsOnline { get; private set; }

        /// <remarks> 'Click' localization string </remarks>
        public string Click { get; private set; }

        /// <remarks> 'Clicking on the green plus button ( ' localization string </remarks>
        public string ClickingOnTheGreenPlusButton { get; private set; }

        /// <remarks> 'Edit the behaviors associated with this item group within this library' localization string </remarks>
        public string EditTheBehaviorsAssociatedWithThisItemGroupWithinThisLibrary { get; private set; }

        /// <remarks> 'Enter the data for this item group below and press the SAVE button when all your edits are complete.' localization string </remarks>
        public string EnterTheDataForThisItemGroupBelowAndPressTheSAVEButtonWhenAllYourEditsAreComplete { get; private set; }

        /// <remarks> 'here for detailed instructions' localization string </remarks>
        public string HereForDetailedInstructions { get; private set; }

    }
}
