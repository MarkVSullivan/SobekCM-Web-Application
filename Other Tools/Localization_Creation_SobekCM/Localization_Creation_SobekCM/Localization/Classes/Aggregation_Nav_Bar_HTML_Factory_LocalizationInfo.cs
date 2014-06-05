namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Aggregation_Nav_Bar_HTML_Factory class </summary>
    public class Aggregation_Nav_Bar_HTML_Factory_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Aggregation_Nav_Bar_HTML_Factory_Localization class </summary>
        public Aggregation_Nav_Bar_HTML_Factory_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Aggregation_Nav_Bar_HTML_Factory";
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
                case "Advanced Search":
                    AdvancedSearch = Value;
                    break;

                case "Basic Search":
                    BasicSearch = Value;
                    break;

                case "Display Text":
                    DisplayText = Value;
                    break;

                case "Map Search":
                    MapSearch = Value;
                    break;

                case "Newspaper Search":
                    NewspaperSearch = Value;
                    break;

                case "Text Search":
                    TextSearch = Value;
                    break;

            }
        }
        /// <remarks> Tabs on aggregation home page - based on search types available </remarks>
        public string AdvancedSearch { get; private set; }

        /// <remarks> Tabs on aggregation home page - based on search types available </remarks>
        public string BasicSearch { get; private set; }

        /// <remarks> 'Display Text' localization string </remarks>
        public string DisplayText { get; private set; }

        /// <remarks> Tabs on aggregation home page - based on search types available </remarks>
        public string MapSearch { get; private set; }

        /// <remarks> Tabs on aggregation home page - based on search types available </remarks>
        public string NewspaperSearch { get; private set; }

        /// <remarks> Tabs on aggregation home page - based on search types available </remarks>
        public string TextSearch { get; private set; }

    }
}
