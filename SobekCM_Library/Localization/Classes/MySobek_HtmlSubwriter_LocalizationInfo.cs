namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the MySobek_HtmlSubwriter class </summary>
    public class MySobek_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the MySobek_HtmlSubwriter_Localization class </summary>
        public MySobek_HtmlSubwriter_LocalizationInfo() : base()
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

                case "Welcome Back XXX":
                    WelcomeBackXXX = Value;
                    break;

            }
        }
        /// <remarks> '%1 Home' localization string </remarks>
        public string XXXHome { get; private set; }

        /// <remarks> Message to welcome back a user by name </remarks>
        public string WelcomeBackXXX { get; private set; }

    }
}
