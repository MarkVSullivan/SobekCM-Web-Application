namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Edit_Serial_Hierarchy_MySobekViewer class </summary>
    public class Edit_Serial_Hierarchy_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Edit_Serial_Hierarchy_MySobekViewer_Localization class </summary>
        public Edit_Serial_Hierarchy_MySobekViewer_LocalizationInfo() : base()
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
                case "EDIT SERIAL HIERARCHY":
                    EDITSERIALHIERARCHY = Value;
                    break;

                case "Implementation For This Feature Is Currently Pending":
                    ImplementationForThisFeatureIsCurrentlyPending = Value;
                    break;

            }
        }
        /// <remarks> 'EDIT SERIAL HIERARCHY' localization string </remarks>
        public string EDITSERIALHIERARCHY { get; private set; }

        /// <remarks> 'Implementation for this feature is currently pending.' localization string </remarks>
        public string ImplementationForThisFeatureIsCurrentlyPending { get; private set; }

    }
}
