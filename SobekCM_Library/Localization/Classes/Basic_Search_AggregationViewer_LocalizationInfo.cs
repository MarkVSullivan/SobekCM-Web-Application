namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Basic_Search_AggregationViewer class </summary>
    public class Basic_Search_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Basic_Search_AggregationViewer_Localization class </summary>
        public Basic_Search_AggregationViewer_LocalizationInfo() : base()
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
                case "Include Nonpublic Items":
                    IncludeNonpublicItems = Value;
                    break;

                case "Search Collection":
                    SearchCollection = Value;
                    break;

            }
        }
        /// <remarks> Used for admins to be able to search private or dark items </remarks>
        public string IncludeNonpublicItems { get; private set; }

        /// <remarks> Used for the basic search usually on the home page </remarks>
        public string SearchCollection { get; private set; }

    }
}
