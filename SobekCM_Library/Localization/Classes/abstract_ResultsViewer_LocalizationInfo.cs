namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the abstract_ResultsViewer class </summary>
    public class abstract_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the abstract_ResultsViewer_Localization class </summary>
        public abstract_ResultsViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "abstract_ResultsViewer";
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

                case "Flight Line":
                    FlightLine = Value;
                    break;

                case "Flight Lines":
                    FlightLines = Value;
                    break;

                case "Map Set":
                    MapSet = Value;
                    break;

                case "Map Sets":
                    MapSets = Value;
                    break;

                case "Photograph Set":
                    PhotographSet = Value;
                    break;

                case "Photograph Sets":
                    PhotographSets = Value;
                    break;

                case "Video":
                    Video = Value;
                    break;

                case "Videos":
                    Videos = Value;
                    break;

                case "Audio Item":
                    AudioItem = Value;
                    break;

                case "Audio Items":
                    AudioItems = Value;
                    break;

                case "Audio":
                    Audio = Value;
                    break;

                case "Audios":
                    Audios = Value;
                    break;

                case "Artifact":
                    Artifact = Value;
                    break;

                case "Artifacts":
                    Artifacts = Value;
                    break;

                case "Item":
                    Item = Value;
                    break;

                case "Items":
                    Items = Value;
                    break;

            }
        }
        /// <remarks> 'issue' localization string </remarks>
        public string Issue { get; private set; }

        /// <remarks> 'issues' localization string </remarks>
        public string Issues { get; private set; }

        /// <remarks> 'flight line' localization string </remarks>
        public string FlightLine { get; private set; }

        /// <remarks> 'flight lines' localization string </remarks>
        public string FlightLines { get; private set; }

        /// <remarks> 'map set' localization string </remarks>
        public string MapSet { get; private set; }

        /// <remarks> 'map sets' localization string </remarks>
        public string MapSets { get; private set; }

        /// <remarks> 'photograph set' localization string </remarks>
        public string PhotographSet { get; private set; }

        /// <remarks> 'photograph sets' localization string </remarks>
        public string PhotographSets { get; private set; }

        /// <remarks> 'video' localization string </remarks>
        public string Video { get; private set; }

        /// <remarks> 'videos' localization string </remarks>
        public string Videos { get; private set; }

        /// <remarks> 'audio item' localization string </remarks>
        public string AudioItem { get; private set; }

        /// <remarks> 'audio items' localization string </remarks>
        public string AudioItems { get; private set; }

        /// <remarks> 'audio' localization string </remarks>
        public string Audio { get; private set; }

        /// <remarks> 'audios' localization string </remarks>
        public string Audios { get; private set; }

        /// <remarks> 'artifact' localization string </remarks>
        public string Artifact { get; private set; }

        /// <remarks> 'artifacts' localization string </remarks>
        public string Artifacts { get; private set; }

        /// <remarks> 'item' localization string </remarks>
        public string Item { get; private set; }

        /// <remarks> 'items' localization string </remarks>
        public string Items { get; private set; }

    }
}
