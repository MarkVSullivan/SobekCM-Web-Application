namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Feature_ItemViewer class </summary>
    public class Feature_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Feature_ItemViewer_Localization class </summary>
        public Feature_ItemViewer_LocalizationInfo() : base()
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
                case "Index Of Features":
                    IndexOfFeatures = Value;
                    break;

                case "UNABLE TO LOAD FEATURES FROM DATABASE":
                    UNABLETOLOADFEATURESFROMDATABASE = Value;
                    break;

            }
        }
        /// <remarks> 'Index of Features' localization string </remarks>
        public string IndexOfFeatures { get; private set; }

        /// <remarks> 'UNABLE TO LOAD FEATURES FROM DATABASE' localization string </remarks>
        public string UNABLETOLOADFEATURESFROMDATABASE { get; private set; }

    }
}
