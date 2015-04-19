namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Google_Map_ResultsViewer class </summary>
    public class Google_Map_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Google_Map_ResultsViewer_Localization class </summary>
        public Google_Map_ResultsViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Google_Map_ResultsViewer";
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
                case "The Following XXX Matches In XXX Sets Share The Same Coordinate Information":
                    TheFollowingXXXMatchesInXXXSetsShareTheSa = Value;
                    break;

                case "The Following XXX Matches Share The Same Coordinate Information":
                    TheFollowingXXXMatchesShareTheSameCoordinat = Value;
                    break;

                case "The Following XXX Matches In XXX Sets Have No Coordinate Information":
                    TheFollowingXXXMatchesInXXXSetsHaveNoCoor = Value;
                    break;

                case "The Following XXX Matches Have No Coordinate Information":
                    TheFollowingXXXMatchesHaveNoCoordinateInfor = Value;
                    break;

                case "The Following XXX Titles Have The Same Coordinate Point":
                    TheFollowingXXXTitlesHaveTheSameCoordinate = Value;
                    break;

            }
        }
        /// <remarks> 'The following %1 matches in  %2 sets share the same coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesInXXXSetsShareTheSa { get; private set; }

        /// <remarks> 'The following %1  matches share the same coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesShareTheSameCoordinat { get; private set; }

        /// <remarks> 'The following  %1 matches  in %2  sets have no coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesInXXXSetsHaveNoCoor { get; private set; }

        /// <remarks> 'The following %1  matches have no coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesHaveNoCoordinateInfor { get; private set; }

        /// <remarks> 'The following %1 titles have the same coordinate point' localization string </remarks>
        public string TheFollowingXXXTitlesHaveTheSameCoordinate { get; private set; }

    }
}
