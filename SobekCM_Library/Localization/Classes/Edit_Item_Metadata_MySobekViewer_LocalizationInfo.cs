namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Edit_Item_Metadata_MySobekViewer class </summary>
    public class Edit_Item_Metadata_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Edit_Item_Metadata_MySobekViewer_Localization class </summary>
        public Edit_Item_Metadata_MySobekViewer_LocalizationInfo() : base()
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

                case "On Editing Metadata Online":
                    OnEditingMetadataOnline = Value;
                    break;

                case "Click":
                    Click = Value;
                    break;

                case "Click On The Element Names For Detailed Information Inluding Definitions Best Practices And Technical Information":
                    ClickOnTheElementNamesForDetailedInformationInludingDefinitionsBestPracticesAndTechnicalInformation = Value;
                    break;

                case "Clicking On The Blue Plus Signs":
                    ClickingOnTheBluePlusSigns = Value;
                    break;

                case "Clicking On The Green Plus Button":
                    ClickingOnTheGreenPlusButton = Value;
                    break;

                case "Edit Item":
                    EditItem = Value;
                    break;

                case "Edit Project":
                    EditProject = Value;
                    break;

                case "Edit This Item":
                    EditThisItem = Value;
                    break;

                case "Edit This Project":
                    EditThisProject = Value;
                    break;

                case "Enter The Data For This Item Below And Press The SAVE Button When All Your Edits Are Complete":
                    EnterTheDataForThisItemBelowAndPressTheSAVEButtonWhenAllYourEditsAreComplete = Value;
                    break;

                case "Enter The Default Data For This Project Below And Press The SAVE Button When All Your Edits Are Complete":
                    EnterTheDefaultDataForThisProjectBelowAndPressTheSAVEButtonWhenAllYourEditsAreComplete = Value;
                    break;

                case "Here For Detailed Instructions":
                    HereForDetailedInstructions = Value;
                    break;

                case "Here To Return To The Simplified Version":
                    HereToReturnToTheSimplifiedVersion = Value;
                    break;

                case "MARC View":
                    MARCView = Value;
                    break;

                case "METS View":
                    METSView = Value;
                    break;

                case "Standard View":
                    StandardView = Value;
                    break;

                case "To Open Detailed Edit Forms Click On The Linked Metadata Values":
                    ToOpenDetailedEditFormsClickOnTheLinkedMetadataValues = Value;
                    break;

                case "You Are Using The Full Editing Form Because This Item Contains Complex Elements Or Was Derived From MARC":
                    YouAreUsingTheFullEditingFormBecauseThisItemContainsComplexElementsOrWasDerivedFromMARC = Value;
                    break;

                case "You Are Using The Full Editing Form Because You Are Editing A Project":
                    YouAreUsingTheFullEditingFormBecauseYouAreEditingAProject = Value;
                    break;

                case "You Are Using The Full Editing Form Click":
                    YouAreUsingTheFullEditingFormClick = Value;
                    break;

                case "You Are Using The Simplified Editing Form Click":
                    YouAreUsingTheSimplifiedEditingFormClick = Value;
                    break;

            }
        }
        /// <remarks> '" ) will add another instance of the element, if the element is repeatable."' localization string </remarks>
        public string WillAddAnotherInstanceOfTheElementIfTheElementIsRepeatable { get; private set; }

        /// <remarks> ' on editing metadata online.' localization string </remarks>
        public string OnEditingMetadataOnline { get; private set; }

        /// <remarks> 'Click' localization string </remarks>
        public string Click { get; private set; }

        /// <remarks> '"Click on the element names for detailed information inluding definitions, best practices, and technical information."' localization string </remarks>
        public string ClickOnTheElementNamesForDetailedInformationInludingDefinitionsBestPracticesAndTechnicalInformation { get; private set; }

        /// <remarks> 'Clicking on the blue plus signs ( ' localization string </remarks>
        public string ClickingOnTheBluePlusSigns { get; private set; }

        /// <remarks> 'Clicking on the green plus button ( ' localization string </remarks>
        public string ClickingOnTheGreenPlusButton { get; private set; }

        /// <remarks> 'Edit Item' localization string </remarks>
        public string EditItem { get; private set; }

        /// <remarks> 'Edit Project' localization string </remarks>
        public string EditProject { get; private set; }

        /// <remarks> 'Edit this item' localization string </remarks>
        public string EditThisItem { get; private set; }

        /// <remarks> 'Edit this project' localization string </remarks>
        public string EditThisProject { get; private set; }

        /// <remarks> 'Enter the data for this item below and press the SAVE button when all your edits are complete.' localization string </remarks>
        public string EnterTheDataForThisItemBelowAndPressTheSAVEButtonWhenAllYourEditsAreComplete { get; private set; }

        /// <remarks> 'Enter the default data for this project below and press the SAVE button when all your edits are complete.' localization string </remarks>
        public string EnterTheDefaultDataForThisProjectBelowAndPressTheSAVEButtonWhenAllYourEditsAreComplete { get; private set; }

        /// <remarks> 'here for detailed instructions' localization string </remarks>
        public string HereForDetailedInstructions { get; private set; }

        /// <remarks> 'here to return to the simplified version' localization string </remarks>
        public string HereToReturnToTheSimplifiedVersion { get; private set; }

        /// <remarks> 'MARC View' localization string </remarks>
        public string MARCView { get; private set; }

        /// <remarks> 'METS View' localization string </remarks>
        public string METSView { get; private set; }

        /// <remarks> 'Standard View' localization string </remarks>
        public string StandardView { get; private set; }

        /// <remarks> '"To open detailed edit forms, click on the linked metadata values."' localization string </remarks>
        public string ToOpenDetailedEditFormsClickOnTheLinkedMetadataValues { get; private set; }

        /// <remarks> 'You are using the full editing form because this item contains complex elements or was derived from MARC.' localization string </remarks>
        public string YouAreUsingTheFullEditingFormBecauseThisItemContainsComplexElementsOrWasDerivedFromMARC { get; private set; }

        /// <remarks> 'You are using the full editing form because you are editing a project.' localization string </remarks>
        public string YouAreUsingTheFullEditingFormBecauseYouAreEditingAProject { get; private set; }

        /// <remarks> 'You are using the full editing form.  Click' localization string </remarks>
        public string YouAreUsingTheFullEditingFormClick { get; private set; }

        /// <remarks> 'You are using the simplified editing form.  Click' localization string </remarks>
        public string YouAreUsingTheSimplifiedEditingFormClick { get; private set; }

    }
}
