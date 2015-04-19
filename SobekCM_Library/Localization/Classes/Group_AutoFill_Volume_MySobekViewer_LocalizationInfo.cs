namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Group_AutoFill_Volume_MySobekViewer class </summary>
    public class Group_AutoFill_Volume_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Group_AutoFill_Volume_MySobekViewer_Localization class </summary>
        public Group_AutoFill_Volume_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Group_AutoFill_Volume_MySobekViewer";
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
                case "AUTOFILL NEW VOLUMES":
                    AUTOFILLNEWVOLUMES = Value;
                    break;

                case "Implementation For This Feature Is Currently Pending":
                    ImplementationForThisFeatureIsCurrentlyPendi = Value;
                    break;

            }
        }
        /// <remarks> 'AUTO-FILL NEW VOLUMES' localization string </remarks>
        public string AUTOFILLNEWVOLUMES { get; private set; }

        /// <remarks> 'Implementation for this feature is currently pending.' localization string </remarks>
        public string ImplementationForThisFeatureIsCurrentlyPendi { get; private set; }

    }
}
