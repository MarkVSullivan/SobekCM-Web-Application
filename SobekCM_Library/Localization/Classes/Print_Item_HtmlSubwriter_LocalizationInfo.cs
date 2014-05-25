namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Print_Item_HtmlSubwriter class </summary>
    public class Print_Item_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Print_Item_HtmlSubwriter_Localization class </summary>
        public Print_Item_HtmlSubwriter_LocalizationInfo() : base()
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
                case "Site":
                    Site = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "URL":
                    URL = Value;
                    break;

            }
        }
        /// <remarks> "(i.e., label for name of site, as in Site: Digital Library of the Carribea)  Creates a simpler page for printing purposes when printing an item" </remarks>
        public string Site { get; private set; }

        /// <remarks> Creates a simpler page for printing purposes when printing an item </remarks>
        public string Title { get; private set; }

        /// <remarks> Creates a simpler page for printing purposes when printing an item </remarks>
        public string URL { get; private set; }

    }
}
