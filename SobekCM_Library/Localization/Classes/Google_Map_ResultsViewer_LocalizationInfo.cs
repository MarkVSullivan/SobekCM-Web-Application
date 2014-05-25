namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Google_Map_ResultsViewer class </summary>
    public class Google_Map_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Google_Map_ResultsViewer_Localization class </summary>
        public Google_Map_ResultsViewer_LocalizationInfo() : base()
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
                case "The Following XXX Matches In XXX Sets Have No Coordinate Information":
                    TheFollowingXXXMatchesInXXXSetsHaveNoCoordinateInformation = Value;
                    break;

                case "The Following XXX Matches Have No Coordinate Information":
                    TheFollowingXXXMatchesHaveNoCoordinateInformation = Value;
                    break;

                case "The Following XXX Matches Share The Same Coordinate Information":
                    TheFollowingXXXMatchesShareTheSameCoordinateInformation = Value;
                    break;

                case "The Following XXX Matches In XXX Sets Share The Same Coordinate Information":
                    TheFollowingXXXMatchesInXXXSetsShareTheSameCoordinateInformation = Value;
                    break;

                case "The Following XXX Titles Have The Same Coordinate Point":
                    TheFollowingXXXTitlesHaveTheSameCoordinatePoint = Value;
                    break;

            }
        }
        /// <remarks> 'The following  %1 matches  in %2  sets have no coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesInXXXSetsHaveNoCoordinateInformation { get; private set; }

        /// <remarks> 'The following %1  matches have no coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesHaveNoCoordinateInformation { get; private set; }

        /// <remarks> 'The following %1  matches share the same coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesShareTheSameCoordinateInformation { get; private set; }

        /// <remarks> 'The following %1 matches in  %2 sets share the same coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesInXXXSetsShareTheSameCoordinateInformation { get; private set; }

        /// <remarks> 'The following %1 titles have the same coordinate point' localization string </remarks>
        public string TheFollowingXXXTitlesHaveTheSameCoordinatePoint { get; private set; }

    }
}
