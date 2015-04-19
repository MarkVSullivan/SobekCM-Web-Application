namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Map_ResultsWriter class </summary>
    public class Map_ResultsWriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Map_ResultsWriter_Localization class </summary>
        public Map_ResultsWriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Map_ResultsWriter";
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

                case "The Following XXX Matches In XXX Sets Share The Same Coordinate Information":
                    TheFollowingXXXMatchesInXXXSetsShareTheSa = Value;
                    break;

                case "The Following XXX Matches Share The Same Coordinate Information":
                    TheFollowingXXXMatchesShareTheSameCoordinat = Value;
                    break;

                case "The Followingxxx Matches In XXX Sets Have No Coordinate Information":
                    TheFollowingxxxMatchesInXXXSetsHaveNoCoord = Value;
                    break;

                case "The Following XXX Matches Have No Coordinate Information":
                    TheFollowingXXXMatchesHaveNoCoordinateInfor = Value;
                    break;

                case "The Following XXX Titles Have The Same Coordinate Point":
                    TheFollowingXXXTitlesHaveTheSameCoordinate = Value;
                    break;

            }
        }
        /// <remarks> '( varies )' localization string </remarks>
        public string Varies { get; private set; }

        /// <remarks> 'The following {0} matches in {1} sets share the same coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesInXXXSetsShareTheSa { get; private set; }

        /// <remarks> 'The following {0} matches share the same coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesShareTheSameCoordinat { get; private set; }

        /// <remarks> 'The following{0} matches in {1} sets have no coordinate information' localization string </remarks>
        public string TheFollowingxxxMatchesInXXXSetsHaveNoCoord { get; private set; }

        /// <remarks> 'The following {0} matches have no coordinate information' localization string </remarks>
        public string TheFollowingXXXMatchesHaveNoCoordinateInfor { get; private set; }

        /// <remarks> 'The following {0} titles have the same coordinate point' localization string </remarks>
        public string TheFollowingXXXTitlesHaveTheSameCoordinate { get; private set; }

    }
}
