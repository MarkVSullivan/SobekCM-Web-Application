namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Text_ItemViewer class </summary>
    public class Text_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Text_ItemViewer_Localization class </summary>
        public Text_ItemViewer_LocalizationInfo() : base()
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
                case "No Text File Exists For This Page":
                    NoTextFileExistsForThisPage = Value;
                    break;

                case "No Text Is Recorded For This Page":
                    NoTextIsRecordedForThisPage = Value;
                    break;

                case "Unknown Error While Retrieving Text":
                    UnknownErrorWhileRetrievingText = Value;
                    break;

            }
        }
        /// <remarks> 'No text file exists for this page' localization string </remarks>
        public string NoTextFileExistsForThisPage { get; private set; }

        /// <remarks> 'No text is recorded for this page' localization string </remarks>
        public string NoTextIsRecordedForThisPage { get; private set; }

        /// <remarks> 'Unknown error while retrieving text' localization string </remarks>
        public string UnknownErrorWhileRetrievingText { get; private set; }

    }
}
