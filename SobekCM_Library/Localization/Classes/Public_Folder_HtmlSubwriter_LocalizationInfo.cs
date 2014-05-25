namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Public_Folder_HtmlSubwriter class </summary>
    public class Public_Folder_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Public_Folder_HtmlSubwriter_Localization class </summary>
        public Public_Folder_HtmlSubwriter_LocalizationInfo() : base()
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
                case "XXX Home":
                    XXXHome = Value;
                    break;

                case "Public Bookshelf":
                    PublicBookshelf = Value;
                    break;

            }
        }
        /// <remarks> '%1 Home' localization string </remarks>
        public string XXXHome { get; private set; }

        /// <remarks> "If a user makes their bookshelf public, this is used to display it" </remarks>
        public string PublicBookshelf { get; private set; }

    }
}
