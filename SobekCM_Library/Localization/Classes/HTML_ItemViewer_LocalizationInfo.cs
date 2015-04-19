namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the HTML_ItemViewer class </summary>
    public class HTML_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the HTML_ItemViewer_Localization class </summary>
        public HTML_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "HTML_ItemViewer";
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
                case "Unable To Pull Html View For Item":
                    UnableToPullHtmlViewForItem = Value;
                    break;

                case "We Apologize For The Inconvenience":
                    WeApologizeForTheInconvenience = Value;
                    break;

                case "Click Here To Report The Problem":
                    ClickHereToReportTheProblem = Value;
                    break;

                case "Unable To Pull Html View For Item XXX":
                    UnableToPullHtmlViewForItemXXX = Value;
                    break;

            }
        }
        /// <remarks> 'Unable to pull html view for item' localization string </remarks>
        public string UnableToPullHtmlViewForItem { get; private set; }

        /// <remarks> 'We apologize for the inconvenience.' localization string </remarks>
        public string WeApologizeForTheInconvenience { get; private set; }

        /// <remarks> 'Click here to report the problem.' localization string </remarks>
        public string ClickHereToReportTheProblem { get; private set; }

        /// <remarks> 'Unable to pull html view for item %1' localization string </remarks>
        public string UnableToPullHtmlViewForItemXXX { get; private set; }

    }
}
