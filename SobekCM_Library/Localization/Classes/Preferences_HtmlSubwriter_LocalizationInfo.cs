namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Preferences_HtmlSubwriter class </summary>
    public class Preferences_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Preferences_HtmlSubwriter_Localization class </summary>
        public Preferences_HtmlSubwriter_LocalizationInfo() : base()
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
                case "Default Sort":
                    DefaultSort = Value;
                    break;

                case "Default View":
                    DefaultView = Value;
                    break;

                case "Language":
                    Language = Value;
                    break;

                case "Preferences":
                    Preferences = Value;
                    break;

                case "Return":
                    Return = Value;
                    break;

            }
        }
        /// <remarks> Allows user to enter their preferences for some display options </remarks>
        public string DefaultSort { get; private set; }

        /// <remarks> Allows user to enter their preferences for some display options </remarks>
        public string DefaultView { get; private set; }

        /// <remarks> Allows user to enter their preferences for some display options </remarks>
        public string Language { get; private set; }

        /// <remarks> Allows user to enter their preferences for some display options </remarks>
        public string Preferences { get; private set; }

        /// <remarks> Allows user to enter their preferences for some display options </remarks>
        public string Return { get; private set; }

    }
}
