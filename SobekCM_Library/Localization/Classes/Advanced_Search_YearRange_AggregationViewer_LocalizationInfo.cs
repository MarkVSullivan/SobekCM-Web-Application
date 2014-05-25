namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Advanced_Search_YearRange_AggregationViewer class </summary>
    public class Advanced_Search_YearRange_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Advanced_Search_YearRange_AggregationViewer_Localization class </summary>
        public Advanced_Search_YearRange_AggregationViewer_LocalizationInfo() : base()
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
                case "And":
                    And = Value;
                    break;

                case "And Not":
                    AndNot = Value;
                    break;

                case "Contains Any Form Of The Search Terms":
                    ContainsAnyFormOfTheSearchTerms = Value;
                    break;

                case "Contains Exactly The Search Terms":
                    ContainsExactlyTheSearchTerms = Value;
                    break;

                case "Contains The Search Term Or Terms Of Similar Meaning":
                    ContainsTheSearchTermOrTermsOfSimilarMeaning = Value;
                    break;

                case "In":
                    In = Value;
                    break;

                case "Limit By Year":
                    LimitByYear = Value;
                    break;

                case "Or":
                    Or = Value;
                    break;

                case "Precision":
                    Precision = Value;
                    break;

                case "Search":
                    Search = Value;
                    break;

                case "Search For":
                    SearchFor = Value;
                    break;

                case "Search Options":
                    SearchOptions = Value;
                    break;

            }
        }
        /// <remarks> 'and' localization string </remarks>
        public string And { get; private set; }

        /// <remarks> 'and not' localization string </remarks>
        public string AndNot { get; private set; }

        /// <remarks> 'Contains any form of the search terms' localization string </remarks>
        public string ContainsAnyFormOfTheSearchTerms { get; private set; }

        /// <remarks> 'Contains exactly the search terms' localization string </remarks>
        public string ContainsExactlyTheSearchTerms { get; private set; }

        /// <remarks> 'Contains the search term or terms of similar meaning' localization string </remarks>
        public string ContainsTheSearchTermOrTermsOfSimilarMeaning { get; private set; }

        /// <remarks> 'in' localization string </remarks>
        public string In { get; private set; }

        /// <remarks> 'Limit by Year' localization string </remarks>
        public string LimitByYear { get; private set; }

        /// <remarks> 'or' localization string </remarks>
        public string Or { get; private set; }

        /// <remarks> 'Precision' localization string </remarks>
        public string Precision { get; private set; }

        /// <remarks> 'Search' localization string </remarks>
        public string Search { get; private set; }

        /// <remarks> 'Search for:' localization string </remarks>
        public string SearchFor { get; private set; }

        /// <remarks> 'Search Options' localization string </remarks>
        public string SearchOptions { get; private set; }

    }
}
