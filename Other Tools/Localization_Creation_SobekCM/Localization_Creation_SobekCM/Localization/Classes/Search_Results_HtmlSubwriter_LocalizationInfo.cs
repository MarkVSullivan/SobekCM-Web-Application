namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Search_Results_HtmlSubwriter class </summary>
    public class Search_Results_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Search_Results_HtmlSubwriter_Localization class </summary>
        public Search_Results_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Search_Results_HtmlSubwriter";
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
                case "Modify Your Search":
                    ModifyYourSearch = Value;
                    break;

                case "New Search":
                    NewSearch = Value;
                    break;

            }
        }
        /// <remarks> Used as main tabs when viewing search results </remarks>
        public string ModifyYourSearch { get; private set; }

        /// <remarks> Used as main tabs when viewing search results </remarks>
        public string NewSearch { get; private set; }

    }
}
