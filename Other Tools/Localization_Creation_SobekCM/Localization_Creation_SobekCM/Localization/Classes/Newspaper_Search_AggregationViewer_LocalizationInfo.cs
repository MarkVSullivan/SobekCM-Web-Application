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
                case "Full Citation":
                    FullCitation = Value;
                    break;

                case "Full Text":
                    FullText = Value;
                    break;

                case "Go":
                    Go = Value;
                    break;

                case "In":
                    In = Value;
                    break;

                case "Location":
                    Location = Value;
                    break;

                case "Newspaper Title":
                    NewspaperTitle = Value;
                    break;

                case "Search For":
                    SearchFor = Value;
                    break;

            }
        }
        /// <remarks> Used in search box for newspaper-specific searches </remarks>
        public string FullCitation { get; private set; }

        /// <remarks> Used in search box for newspaper-specific searches </remarks>
        public string FullText { get; private set; }

        /// <remarks> 'Go' localization string </remarks>
        public string Go { get; private set; }

        /// <remarks> Used in search box for newspaper-specific searches </remarks>
        public string In { get; private set; }

        /// <remarks> Used in search box for newspaper-specific searches </remarks>
        public string Location { get; private set; }

        /// <remarks> Used in search box for newspaper-specific searches </remarks>
        public string NewspaperTitle { get; private set; }

        /// <remarks> Used in search box for newspaper-specific searches </remarks>
        public string SearchFor { get; private set; }

    }
}
