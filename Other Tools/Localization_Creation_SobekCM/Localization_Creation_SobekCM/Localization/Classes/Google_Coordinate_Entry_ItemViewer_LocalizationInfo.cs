namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Google_Coordinate_Entry_ItemViewer class </summary>
    public class Google_Coordinate_Entry_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Google_Coordinate_Entry_ItemViewer_Localization class </summary>
        public Google_Coordinate_Entry_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Google_Coordinate_Entry_ItemViewer";
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
                case "Loading":
                    Loading = Value;
                    break;

            }
        }
        /// <remarks> 'Loading...' localization string </remarks>
        public string Loading { get; private set; }

    }
}
