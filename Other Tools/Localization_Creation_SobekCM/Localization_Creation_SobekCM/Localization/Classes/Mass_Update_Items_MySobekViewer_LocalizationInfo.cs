namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Mass_Update_Items_MySobekViewer class </summary>
    public class Mass_Update_Items_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Mass_Update_Items_MySobekViewer_Localization class </summary>
        public Mass_Update_Items_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Mass_Update_Items_MySobekViewer";
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

                case "Change The Behavior Of All Items Belonging To This Group":
                    ChangeTheBehaviorOfAllItemsBelongingToThisGroup = Value;
                    break;

                case "Click":
                    Click = Value;
                    break;

                case "Clicking On The Green Plus Button":
                    ClickingOnTheGreenPlusButton = Value;
                    break;

                case "Enter Any Behaviors You Would Like To Have Propogate Through All The Items Within This Item Groupand Press The SAVE Button When Complete":
                    EnterAnyBehaviorsYouWouldLikeToHavePropogateThroughAllTheItemsWithinThisItemGroupandPressTheSAVEButtonWhenComplete = Value;
                    break;

                case "Here For Detailed Instructions":
                    HereForDetailedInstructions = Value;
                    break;

                case "MASS UPDATE":
                    MASSUPDATE = Value;
                    break;

                case "On Mass Updating Behaviors Online":
                    OnMassUpdatingBehaviorsOnline = Value;
                    break;

            }
        }
        /// <remarks> '" ) will add another instance of the element, if the element is repeatable."' localization string </remarks>
        public string WillAddAnotherInstanceOfTheElementIfTheElementIsRepeatable { get; private set; }

        /// <remarks> 'Change the behavior of all items belonging to this group' localization string </remarks>
        public string ChangeTheBehaviorOfAllItemsBelongingToThisGroup { get; private set; }

        /// <remarks> 'Click' localization string </remarks>
        public string Click { get; private set; }

        /// <remarks> 'Clicking on the green plus button ( ' localization string </remarks>
        public string ClickingOnTheGreenPlusButton { get; private set; }

        /// <remarks> 'Enter any behaviors you would like to have propogate through all the items within this item group.and press the SAVE button when complete.' localization string </remarks>
        public string EnterAnyBehaviorsYouWouldLikeToHavePropogateThroughAllTheItemsWithinThisItemGroupandPressTheSAVEButtonWhenComplete { get; private set; }

        /// <remarks> 'here for detailed instructions' localization string </remarks>
        public string HereForDetailedInstructions { get; private set; }

        /// <remarks> 'MASS UPDATE' localization string </remarks>
        public string MASSUPDATE { get; private set; }

        /// <remarks> 'on mass updating behaviors online.' localization string </remarks>
        public string OnMassUpdatingBehaviorsOnline { get; private set; }

    }
}
