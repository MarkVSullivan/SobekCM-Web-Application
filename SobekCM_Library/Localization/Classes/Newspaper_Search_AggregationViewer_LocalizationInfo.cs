namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Newspaper_Search_AggregationViewer class </summary>
    public class Newspaper_Search_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Newspaper_Search_AggregationViewer_Localization class </summary>
        public Newspaper_Search_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Newspaper_Search_AggregationViewer";
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
                case "Search For":
                    SearchFor = Value;
                    break;

                case "In":
                    In = Value;
                    break;

                case "Full Citation":
                    FullCitation = Value;
                    break;

                case "Full Text":
                    FullText = Value;
                    break;

                case "Newspaper Title":
                    NewspaperTitle = Value;
                    break;

                case "Location":
                    Location = Value;
                    break;

                case "Go":
                    Go = Value;
                    break;

            }
        }
        /// <remarks> 'Search for:' localization string </remarks>
        public string SearchFor { get; private set; }

        /// <remarks> 'in' localization string </remarks>
        public string In { get; private set; }

        /// <remarks> 'Full Citation' localization string </remarks>
        public string FullCitation { get; private set; }

        /// <remarks> 'Full Text' localization string </remarks>
        public string FullText { get; private set; }

        /// <remarks> 'Newspaper Title' localization string </remarks>
        public string NewspaperTitle { get; private set; }

        /// <remarks> 'Location' localization string </remarks>
        public string Location { get; private set; }

        /// <remarks> 'Go' localization string </remarks>
        public string Go { get; private set; }

    }
}
