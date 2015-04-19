namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Error_HtmlSubwriter class </summary>
    public class Error_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Error_HtmlSubwriter_Localization class </summary>
        public Error_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Error_HtmlSubwriter";
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
                case "The Item Indicated Was Not Valid":
                    TheItemIndicatedWasNotValid = Value;
                    break;

                case "Click Here To Report An Error":
                    ClickHereToReportAnError = Value;
                    break;

                case "Unknown Error Occurred":
                    UnknownErrorOccurred = Value;
                    break;

                case "We Apologize For The Inconvenience":
                    WeApologizeForTheInconvenience = Value;
                    break;

                case "Click Here To Return To The Library":
                    ClickHereToReturnToTheLibrary = Value;
                    break;

                case "Click Here To Report The Problem":
                    ClickHereToReportTheProblem = Value;
                    break;

            }
        }
        /// <remarks> 'The item indicated was not valid.' localization string </remarks>
        public string TheItemIndicatedWasNotValid { get; private set; }

        /// <remarks> 'Click here to report an error.' localization string </remarks>
        public string ClickHereToReportAnError { get; private set; }

        /// <remarks> 'Unknown error occurred.' localization string </remarks>
        public string UnknownErrorOccurred { get; private set; }

        /// <remarks> 'We apologize for the inconvenience.' localization string </remarks>
        public string WeApologizeForTheInconvenience { get; private set; }

        /// <remarks> 'Click here to return to the library.' localization string </remarks>
        public string ClickHereToReturnToTheLibrary { get; private set; }

        /// <remarks> 'Click here to report the problem.' localization string </remarks>
        public string ClickHereToReportTheProblem { get; private set; }

    }
}
