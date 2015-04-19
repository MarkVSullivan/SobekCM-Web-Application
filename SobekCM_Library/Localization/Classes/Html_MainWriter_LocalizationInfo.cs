namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Html_MainWriter class </summary>
    public class Html_MainWriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Html_MainWriter_Localization class </summary>
        public Html_MainWriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Html_MainWriter";
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
                case "XXX Item":
                    XXXItem = Value;
                    break;

                case "XXX Home":
                    XXXHome = Value;
                    break;

                case "XXX Search":
                    XXXSearch = Value;
                    break;

                case "XXX Search Results":
                    XXXSearchResults = Value;
                    break;

                case "XXX Preferences":
                    XXXPreferences = Value;
                    break;

                case "XXX Contact Us":
                    XXXContactUs = Value;
                    break;

                case "XXX Contact Sent":
                    XXXContactSent = Value;
                    break;

                case "XXX Error":
                    XXXError = Value;
                    break;

                case "Log Out":
                    LogOut = Value;
                    break;

                case "Xxxs XXX":
                    XxxsXXX = Value;
                    break;

            }
        }
        /// <remarks> '%1 Item' localization string </remarks>
        public string XXXItem { get; private set; }

        /// <remarks> '%1 Home' localization string </remarks>
        public string XXXHome { get; private set; }

        /// <remarks> '%1 Search' localization string </remarks>
        public string XXXSearch { get; private set; }

        /// <remarks> '%1 Search Results' localization string </remarks>
        public string XXXSearchResults { get; private set; }

        /// <remarks> '%1 Preferences' localization string </remarks>
        public string XXXPreferences { get; private set; }

        /// <remarks> '%1 Contact Us' localization string </remarks>
        public string XXXContactUs { get; private set; }

        /// <remarks> '%1 Contact Sent' localization string </remarks>
        public string XXXContactSent { get; private set; }

        /// <remarks> '%1 Error' localization string </remarks>
        public string XXXError { get; private set; }

        /// <remarks> 'Log Out' localization string </remarks>
        public string LogOut { get; private set; }

        /// <remarks> '%1's %2 ' localization string </remarks>
        public string XxxsXXX { get; private set; }

    }
}
