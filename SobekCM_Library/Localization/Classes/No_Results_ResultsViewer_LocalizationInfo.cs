namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the No_Results_ResultsViewer class </summary>
    public class No_Results_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the No_Results_ResultsViewer_Localization class </summary>
        public No_Results_ResultsViewer_LocalizationInfo() : base()
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
                case "XXX Found In The University Of Florida Library Catalog":
                    XXXFoundInTheUniversityOfFloridaLibraryCatalog = Value;
                    break;

                case "XXX Result Found In XXX":
                    XXXResultFoundInXXX = Value;
                    break;

                case "Consider Searching One Of The Following":
                    ConsiderSearchingOneOfTheFollowing = Value;
                    break;

                case "One Found In The University Of Florida Library Catalog":
                    OneFoundInTheUniversityOfFloridaLibraryCatalog = Value;
                    break;

                case "Online Resource":
                    OnlineResource = Value;
                    break;

                case "Physical Holdings":
                    PhysicalHoldings = Value;
                    break;

                case "The Following Matches Were Found":
                    TheFollowingMatchesWereFound = Value;
                    break;

                case "Your Search Returned No Results":
                    YourSearchReturnedNoResults = Value;
                    break;

            }
        }
        /// <remarks> '%1 found in the University of Florida Library Catalog' localization string </remarks>
        public string XXXFoundInTheUniversityOfFloridaLibraryCatalog { get; private set; }

        /// <remarks> "for example, '10 results found in dLOC'" </remarks>
        public string XXXResultFoundInXXX { get; private set; }

        /// <remarks> 'Consider searching one of the following:' localization string </remarks>
        public string ConsiderSearchingOneOfTheFollowing { get; private set; }

        /// <remarks> 'One found in the University of Florida Library Catalog' localization string </remarks>
        public string OneFoundInTheUniversityOfFloridaLibraryCatalog { get; private set; }

        /// <remarks> 'Online Resource' localization string </remarks>
        public string OnlineResource { get; private set; }

        /// <remarks> 'Physical Holdings' localization string </remarks>
        public string PhysicalHoldings { get; private set; }

        /// <remarks> 'The following matches were found:' localization string </remarks>
        public string TheFollowingMatchesWereFound { get; private set; }

        /// <remarks> Text displayed when no results found for a search </remarks>
        public string YourSearchReturnedNoResults { get; private set; }

    }
}
