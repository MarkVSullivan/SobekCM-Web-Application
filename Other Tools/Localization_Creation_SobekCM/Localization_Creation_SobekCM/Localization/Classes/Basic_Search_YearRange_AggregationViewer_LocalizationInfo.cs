namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Basic_Search_YearRange_AggregationViewer class </summary>
    public class Basic_Search_YearRange_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Basic_Search_YearRange_AggregationViewer_Localization class </summary>
        public Basic_Search_YearRange_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Basic_Search_YearRange_AggregationViewer";
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
                case "Limit By Year":
                    LimitByYear = Value;
                    break;

                case "Search Collection":
                    SearchCollection = Value;
                    break;

            }
        }
        /// <remarks> 'Limit by Year' localization string </remarks>
        public string LimitByYear { get; private set; }

        /// <remarks> 'Search Collection' localization string </remarks>
        public string SearchCollection { get; private set; }

    }
}
