namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Print_Item_HtmlSubwriter class </summary>
    public class Print_Item_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Print_Item_HtmlSubwriter_Localization class </summary>
        public Print_Item_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Print_Item_HtmlSubwriter";
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
                case "Title":
                    Title = Value;
                    break;

                case "URL":
                    URL = Value;
                    break;

                case "Site":
                    Site = Value;
                    break;

            }
        }
        /// <remarks> 'Title:' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'URL:' localization string </remarks>
        public string URL { get; private set; }

        /// <remarks> 'Site:' localization string </remarks>
        public string Site { get; private set; }

    }
}
