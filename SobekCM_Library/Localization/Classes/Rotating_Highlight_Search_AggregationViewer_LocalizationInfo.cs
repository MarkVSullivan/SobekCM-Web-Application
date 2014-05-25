namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Rotating_Highlight_Search_AggregationViewer class </summary>
    public class Rotating_Highlight_Search_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Rotating_Highlight_Search_AggregationViewer_Localization class </summary>
        public Rotating_Highlight_Search_AggregationViewer_LocalizationInfo() : base()
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
                case "Search Collection":
                    SearchCollection = Value;
                    break;

            }
        }
        /// <remarks> On the special rotating image search box on home pages </remarks>
        public string SearchCollection { get; private set; }

    }
}
