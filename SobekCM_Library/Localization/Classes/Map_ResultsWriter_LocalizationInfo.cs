namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Map_ResultsWriter class </summary>
    public class Map_ResultsWriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Map_ResultsWriter_Localization class </summary>
        public Map_ResultsWriter_LocalizationInfo() : base()
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
                case "Varies":
                    Varies = Value;
                    break;

                case "The Following XXX Matches Have No Coordinate Information":
                    TheFollowingXXXMatchesHaveNoCoordinateInformation = Value;
                    break;

                case "The Following XXX Matches In XXX Sets Share The Same Coordinate Information":
                    TheFollowingXXXMatchesInXXXSetsShareTheSameCoordinateInformation = Value;
                    break;

                case "The Following XXX Matches Share The Same Coordinate Information":
                    TheFollowingXXXMatchesShareTheSameCoordinateInformation = Value;
                    break;

                case "The Following XXX Titles Have The Same Coordinate Point":
                    TheFollowingXXXTitlesHaveTheSameCoordinatePoint = Value;
                    break;

                case "The Followingxxx Matches In XXX Sets Have No Coordinate Information":
                    TheFollowingxxxMatchesInXXXSetsHaveNoCoordinateInformation = Value;
                    break;

            }
        }
        /// <remarks> "Text used when displaying a result field which is not the same for all the items within the same title.  (i.e., two different publishers over time )" </remarks>
        public string Varies { get; private set; }

        /// <remarks> "Results displayed as a map, but without coordinate information" </remarks>
        public string TheFollowingXXXMatchesHaveNoCoordinateInformation { get; private set; }

        /// <remarks> Results text for a single map when displaying results as a map </remarks>
        public string TheFollowingXXXMatchesInXXXSetsShareTheSameCoordinateInformation { get; private set; }

        /// <remarks> Results text for a single map when displaying results as a map </remarks>
        public string TheFollowingXXXMatchesShareTheSameCoordinateInformation { get; private set; }

        /// <remarks> Single point shared by multiple titles </remarks>
        public string TheFollowingXXXTitlesHaveTheSameCoordinatePoint { get; private set; }

        /// <remarks> "Results displayed as a map, but without coordinate information" </remarks>
        public string TheFollowingxxxMatchesInXXXSetsHaveNoCoordinateInformation { get; private set; }

    }
}
