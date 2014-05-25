namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Brief_ResultsViewer class </summary>
    public class Brief_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Brief_ResultsViewer_Localization class </summary>
        public Brief_ResultsViewer_LocalizationInfo() : base()
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
                case "Access Restricted":
                    AccessRestricted = Value;
                    break;

                case "MISSING THUMBNAIL":
                    MISSINGTHUMBNAIL = Value;
                    break;

            }
        }
        /// <remarks> Text used when displaying an access-restricted item in the results list </remarks>
        public string AccessRestricted { get; private set; }

        /// <remarks> 'MISSING THUMBNAIL' localization string </remarks>
        public string MISSINGTHUMBNAIL { get; private set; }

    }
}
