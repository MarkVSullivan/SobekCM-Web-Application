namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Google_Map_ItemViewer class </summary>
    public class Google_Map_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Google_Map_ItemViewer_Localization class </summary>
        public Google_Map_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Google_Map_ItemViewer";
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
                case "Address":
                    Address = Value;
                    break;

                case "Click Here To Search Other Items In The Current Collection":
                    ClickHereToSearchOtherItemsInTheCurrentCollection = Value;
                    break;

                case "Enter Address Ie 12 Main Street Gainesville Florida":
                    EnterAddressIe12MainStreetGainesvilleFlorida = Value;
                    break;

                case "Find Address":
                    FindAddress = Value;
                    break;

                case "Modify Item Search":
                    ModifyItemSearch = Value;
                    break;

                case "Modify Search Within Flight":
                    ModifySearchWithinFlight = Value;
                    break;

                case "Press The I Search I Button To See Results":
                    PressTheSearchButtonToSeeResults = Value;
                    break;

                case "PRESS TO SELECT AREA":
                    PRESSTOSELECTAREA = Value;
                    break;

                case "PRESS TO SELECT POINT":
                    PRESSTOSELECTPOINT = Value;
                    break;

                case "Search":
                    Search = Value;
                    break;

                case "Search All Flights":
                    SearchAllFlights = Value;
                    break;

                case "Search Entire Collection":
                    SearchEntireCollection = Value;
                    break;

                case "SELECT A POINT ON THE MAP":
                    SELECTAPOINTONTHEMAP = Value;
                    break;

                case "Select The I Select Area I Button Below To Draw A Search Box On The Map Or Enter An Address And Press Find Address":
                    SelectTheSelectAreaButtonBelowToDrawASearchBoxOnTheMapOrEnterAnAddressAndPressFindAddress = Value;
                    break;

                case "SELECT THE FIRST POINT":
                    SELECTTHEFIRSTPOINT = Value;
                    break;

                case "SELECT THE SECOND POINT":
                    SELECTTHESECONDPOINT = Value;
                    break;

                case "Sheet":
                    Sheet = Value;
                    break;

                case "The Following Results Match Your Geographic Search And Also Appear On The Navigation Bar To The Left":
                    TheFollowingResultsMatchYourGeographicSearchAndAlsoAppearOnTheNavigationBarToTheLeft = Value;
                    break;

                case "There Were No Matches Within This Item For Your Geographic Search":
                    ThereWereNoMatchesWithinThisItemForYourGeographicSearch = Value;
                    break;

                case "Tile":
                    Tile = Value;
                    break;

                case "Zoom To Extent":
                    ZoomToExtent = Value;
                    break;

                case "Zoom To Matches":
                    ZoomToMatches = Value;
                    break;

            }
        }
        /// <remarks> 'Address:' localization string </remarks>
        public string Address { get; private set; }

        /// <remarks> 'Click here to search other items in the current collection' localization string </remarks>
        public string ClickHereToSearchOtherItemsInTheCurrentCollection { get; private set; }

        /// <remarks> '"Enter address ( i.e., 12 Main Street, Gainesville Florida )"' localization string </remarks>
        public string EnterAddressIe12MainStreetGainesvilleFlorida { get; private set; }

        /// <remarks> 'Find Address' localization string </remarks>
        public string FindAddress { get; private set; }

        /// <remarks> 'Modify item search' localization string </remarks>
        public string ModifyItemSearch { get; private set; }

        /// <remarks> Special text for aerial photography sets </remarks>
        public string ModifySearchWithinFlight { get; private set; }

        /// <remarks> 'Press the <i>Search</i> button to see results' localization string </remarks>
        public string PressTheSearchButtonToSeeResults { get; private set; }

        /// <remarks> 'PRESS TO SELECT AREA' localization string </remarks>
        public string PRESSTOSELECTAREA { get; private set; }

        /// <remarks> 'PRESS TO SELECT POINT' localization string </remarks>
        public string PRESSTOSELECTPOINT { get; private set; }

        /// <remarks> 'Search' localization string </remarks>
        public string Search { get; private set; }

        /// <remarks> 'Search all flights' localization string </remarks>
        public string SearchAllFlights { get; private set; }

        /// <remarks> 'Search entire collection' localization string </remarks>
        public string SearchEntireCollection { get; private set; }

        /// <remarks> 'SELECT A POINT ON THE MAP' localization string </remarks>
        public string SELECTAPOINTONTHEMAP { get; private set; }

        /// <remarks> 'Select the <i>Select Area</i> button below to draw a search box on the map or enter an address and press Find Address.' localization string </remarks>
        public string SelectTheSelectAreaButtonBelowToDrawASearchBoxOnTheMapOrEnterAnAddressAndPressFindAddress { get; private set; }

        /// <remarks> When doing an area search </remarks>
        public string SELECTTHEFIRSTPOINT { get; private set; }

        /// <remarks> When doing an area search </remarks>
        public string SELECTTHESECONDPOINT { get; private set; }

        /// <remarks> Used for 'Sheet 4' - for everything except aerials </remarks>
        public string Sheet { get; private set; }

        /// <remarks> 'The following results match your geographic search and also appear on the navigation bar to the left:' localization string </remarks>
        public string TheFollowingResultsMatchYourGeographicSearchAndAlsoAppearOnTheNavigationBarToTheLeft { get; private set; }

        /// <remarks> 'There were no matches within this item for your geographic search.' localization string </remarks>
        public string ThereWereNoMatchesWithinThisItemForYourGeographicSearch { get; private set; }

        /// <remarks> Used for 'Tile 3' - for aerial photography </remarks>
        public string Tile { get; private set; }

        /// <remarks> Zooms to the entire extent of the map sets </remarks>
        public string ZoomToExtent { get; private set; }

        /// <remarks> Zooms in to just the matching sheets/tiles </remarks>
        public string ZoomToMatches { get; private set; }

    }
}
