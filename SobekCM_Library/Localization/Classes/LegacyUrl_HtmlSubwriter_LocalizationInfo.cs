namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the LegacyUrl_HtmlSubwriter class </summary>
    public class LegacyUrl_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the LegacyUrl_HtmlSubwriter_Localization class </summary>
        public LegacyUrl_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "LegacyUrl_HtmlSubwriter";
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
                case "Deprecated URL Detected":
                    DeprecatedURLDetected = Value;
                    break;

                case "The URL You Entered Is A Legacy URL Support For This URL Will End Shortly":
                    TheURLYouEnteredIsALegacyURLSupportForTh = Value;
                    break;

                case "Please Update Your Records To The New URL Below":
                    PleaseUpdateYourRecordsToTheNewURLBelow = Value;
                    break;

            }
        }
        /// <remarks> 'Deprecated URL detected' localization string </remarks>
        public string DeprecatedURLDetected { get; private set; }

        /// <remarks> 'The URL you entered is a legacy URL.  Support for this URL will end shortly.' localization string </remarks>
        public string TheURLYouEnteredIsALegacyURLSupportForTh { get; private set; }

        /// <remarks> 'Please update your records to the new URL below:' localization string </remarks>
        public string PleaseUpdateYourRecordsToTheNewURLBelow { get; private set; }

    }
}
