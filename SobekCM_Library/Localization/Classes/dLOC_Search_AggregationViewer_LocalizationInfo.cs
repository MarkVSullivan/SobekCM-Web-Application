namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the dLOC_Search_AggregationViewer class </summary>
    public class dLOC_Search_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the dLOC_Search_AggregationViewer_Localization class </summary>
        public dLOC_Search_AggregationViewer_LocalizationInfo() : base()
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
                case "Go":
                    Go = Value;
                    break;

                case "Include Newspapers":
                    IncludeNewspapers = Value;
                    break;

                case "Search Full Text":
                    SearchFullText = Value;
                    break;

            }
        }
        /// <remarks> 'Go' localization string </remarks>
        public string Go { get; private set; }

        /// <remarks> "Used for searching full text in dLOC, excludes newspaper text" </remarks>
        public string IncludeNewspapers { get; private set; }

        /// <remarks> "Used for searching full text in dLOC, excludes newspaper text" </remarks>
        public string SearchFullText { get; private set; }

    }
}
