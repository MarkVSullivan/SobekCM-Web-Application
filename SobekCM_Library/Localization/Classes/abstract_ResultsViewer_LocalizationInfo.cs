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
                case "Artifact":
                    Artifact = Value;
                    break;

                case "Artifacts":
                    Artifacts = Value;
                    break;

                case "Audio":
                    Audio = Value;
                    break;

                case "Audio Item":
                    AudioItem = Value;
                    break;

                case "Audio Items":
                    AudioItems = Value;
                    break;

                case "Audios":
                    Audios = Value;
                    break;

                case "Flight Line":
                    FlightLine = Value;
                    break;

                case "Flight Lines":
                    FlightLines = Value;
                    break;

                case "Issue":
                    Issue = Value;
                    break;

                case "Issues":
                    Issues = Value;
                    break;

                case "Item":
                    Item = Value;
                    break;

                case "Items":
                    Items = Value;
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

            }
        }
        /// <remarks> "In the search result, if a title is all artifacts use 'artifact' and 'artifacts'" </remarks>
        public string Artifact { get; private set; }

        /// <remarks> "In the search result, if a title is all artifacts use 'artifact' and 'artifacts'" </remarks>
        public string Artifacts { get; private set; }

        /// <remarks> 'audio' localization string </remarks>
        public string Audio { get; private set; }

        /// <remarks> "In the search result, if a title is all audio use 'audio item' and 'audio items'" </remarks>
        public string AudioItem { get; private set; }

        /// <remarks> "In the search result, if a title is all audio use 'audio item' and 'audio items'" </remarks>
        public string AudioItems { get; private set; }

        /// <remarks> 'audios' localization string </remarks>
        public string Audios { get; private set; }

        /// <remarks> "In the search result, if a title is all aerial photography use 'flight line' and 'flight lines'" </remarks>
        public string FlightLine { get; private set; }

        /// <remarks> "In the search result, if a title is all aerial photography use 'flight line' and 'flight lines'" </remarks>
        public string FlightLines { get; private set; }

        /// <remarks> "In the search result, if a title is all newspaper, use 'issue' and 'issues'" </remarks>
        public string Issue { get; private set; }

        /// <remarks> "In the search result, if a title is all newspaper, use 'issue' and 'issues'" </remarks>
        public string Issues { get; private set; }

        /// <remarks> "In the search result, if a title is all newspaper, use 'item' and 'itens'" </remarks>
        public string Item { get; private set; }

        /// <remarks> "In the search result, if a title is all newspaper, use 'item' and 'itens'" </remarks>
        public string Items { get; private set; }

        /// <remarks> "In the search result, if a title is all maps use 'map set' and 'map sets'" </remarks>
        public string MapSet { get; private set; }

        /// <remarks> "In the search result, if a title is all maps use 'map set' and 'map sets'" </remarks>
        public string MapSets { get; private set; }

        /// <remarks> "In the search result, if a title is all newspaper, use 'photographic set' and 'photographic sets'" </remarks>
        public string PhotographSet { get; private set; }

        /// <remarks> "In the search result, if a title is all newspaper, use 'photographic set' and 'photographic sets'" </remarks>
        public string PhotographSets { get; private set; }

        /// <remarks> "In the search result, if a title is all videos use 'video' and 'videos'" </remarks>
        public string Video { get; private set; }

        /// <remarks> "In the search result, if a title is all videos use 'video' and 'videos'" </remarks>
        public string Videos { get; private set; }

    }
}
