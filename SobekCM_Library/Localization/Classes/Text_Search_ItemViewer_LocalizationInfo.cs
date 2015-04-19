namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Text_Search_ItemViewer class </summary>
    public class Text_Search_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Text_Search_ItemViewer_Localization class </summary>
        public Text_Search_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Text_Search_ItemViewer";
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
                case "Search This Document":
                    SearchThisDocument = Value;
                    break;

                case "Your Search Within This Document For":
                    YourSearchWithinThisDocumentFor = Value;
                    break;

                case "AND":
                    AND = Value;
                    break;

                case "OR":
                    OR = Value;
                    break;

                case "AND NOT":
                    ANDNOT = Value;
                    break;

                case "Resulted In":
                    ResultedIn = Value;
                    break;

                case "Matching Pages":
                    MatchingPages = Value;
                    break;

                case "No Matching Pages":
                    NoMatchingPages = Value;
                    break;

                case "You Can Expand Your Results By Searching For":
                    YouCanExpandYourResultsBySearchingFor = Value;
                    break;

                case "You Can Restrict Your Results By Searching For":
                    YouCanRestrictYourResultsBySearchingFor = Value;
                    break;

                case "First Page":
                    FirstPage = Value;
                    break;

                case "Previous Page":
                    PreviousPage = Value;
                    break;

                case "Next Page":
                    NextPage = Value;
                    break;

                case "Last Page":
                    LastPage = Value;
                    break;

                case "First":
                    First = Value;
                    break;

                case "Previous":
                    Previous = Value;
                    break;

                case "Next":
                    Next = Value;
                    break;

                case "Last":
                    Last = Value;
                    break;

            }
        }
        /// <remarks> 'Search this document' localization string </remarks>
        public string SearchThisDocument { get; private set; }

        /// <remarks> 'Your search within this document for ' localization string </remarks>
        public string YourSearchWithinThisDocumentFor { get; private set; }

        /// <remarks> 'AND' localization string </remarks>
        public string AND { get; private set; }

        /// <remarks> 'OR ' localization string </remarks>
        public string OR { get; private set; }

        /// <remarks> 'AND NOT' localization string </remarks>
        public string ANDNOT { get; private set; }

        /// <remarks> 'resulted in' localization string </remarks>
        public string ResultedIn { get; private set; }

        /// <remarks> 'matching pages' localization string </remarks>
        public string MatchingPages { get; private set; }

        /// <remarks> 'no matching pages' localization string </remarks>
        public string NoMatchingPages { get; private set; }

        /// <remarks> 'You can expand your results by searching for' localization string </remarks>
        public string YouCanExpandYourResultsBySearchingFor { get; private set; }

        /// <remarks> 'You can restrict your results by searching for' localization string </remarks>
        public string YouCanRestrictYourResultsBySearchingFor { get; private set; }

        /// <remarks> 'First Page' localization string </remarks>
        public string FirstPage { get; private set; }

        /// <remarks> 'Previous Page' localization string </remarks>
        public string PreviousPage { get; private set; }

        /// <remarks> 'Next Page' localization string </remarks>
        public string NextPage { get; private set; }

        /// <remarks> 'Last Page' localization string </remarks>
        public string LastPage { get; private set; }

        /// <remarks> 'First' localization string </remarks>
        public string First { get; private set; }

        /// <remarks> 'Previous' localization string </remarks>
        public string Previous { get; private set; }

        /// <remarks> 'Next' localization string </remarks>
        public string Next { get; private set; }

        /// <remarks> 'Last' localization string </remarks>
        public string Last { get; private set; }

    }
}
