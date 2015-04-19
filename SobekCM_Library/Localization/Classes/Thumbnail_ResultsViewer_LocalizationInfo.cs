namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Thumbnail_ResultsViewer class </summary>
    public class Thumbnail_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Thumbnail_ResultsViewer_Localization class </summary>
        public Thumbnail_ResultsViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Thumbnail_ResultsViewer";
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
                case "Issue":
                    Issue = Value;
                    break;

                case "Issues":
                    Issues = Value;
                    break;

                case "Volume":
                    Volume = Value;
                    break;

                case "Volumes":
                    Volumes = Value;
                    break;

                case "Access Restricted":
                    AccessRestricted = Value;
                    break;

            }
        }
        /// <remarks> 'issue' localization string </remarks>
        public string Issue { get; private set; }

        /// <remarks> 'issues' localization string </remarks>
        public string Issues { get; private set; }

        /// <remarks> 'volume' localization string </remarks>
        public string Volume { get; private set; }

        /// <remarks> 'volumes' localization string </remarks>
        public string Volumes { get; private set; }

        /// <remarks> 'Access Restricted' localization string </remarks>
        public string AccessRestricted { get; private set; }

    }
}
