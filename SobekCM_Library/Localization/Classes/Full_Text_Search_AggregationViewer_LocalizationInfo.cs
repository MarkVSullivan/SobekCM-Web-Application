namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Full_Text_Search_AggregationViewer class </summary>
    public class Full_Text_Search_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Full_Text_Search_AggregationViewer_Localization class </summary>
        public Full_Text_Search_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Full_Text_Search_AggregationViewer";
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
                case "Search Full Text":
                    SearchFullText = Value;
                    break;

                case "Go":
                    Go = Value;
                    break;

            }
        }
        /// <remarks> 'Search full text' localization string </remarks>
        public string SearchFullText { get; private set; }

        /// <remarks> 'Go' localization string </remarks>
        public string Go { get; private set; }

    }
}
