namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the General class </summary>
    public class General_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the General_Localization class </summary>
        public General_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "General";
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
                case "My":
                    My = Value;
                    break;

                case "Close":
                    Close = Value;
                    break;

                case "Search":
                    Search = Value;
                    break;

                case "Help":
                    Help = Value;
                    break;

                case "Submit":
                    Submit = Value;
                    break;

                case "Save":
                    Save = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "Sort By":
                    SortBy = Value;
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

                case "January":
                    January = Value;
                    break;

                case "February":
                    February = Value;
                    break;

                case "March":
                    March = Value;
                    break;

                case "April":
                    April = Value;
                    break;

                case "May":
                    May = Value;
                    break;

                case "June":
                    June = Value;
                    break;

                case "July":
                    July = Value;
                    break;

                case "August":
                    August = Value;
                    break;

                case "September":
                    September = Value;
                    break;

                case "October":
                    October = Value;
                    break;

                case "November":
                    November = Value;
                    break;

                case "December":
                    December = Value;
                    break;

                case "Unknown":
                    Unknown = Value;
                    break;

                case "Error":
                    Error = Value;
                    break;

                case "Total":
                    Total = Value;
                    break;

                case "Bibid":
                    Bibid = Value;
                    break;

                case "VID":
                    VID = Value;
                    break;

                case "One":
                    One = Value;
                    break;

                case "Two":
                    Two = Value;
                    break;

                case "Three":
                    Three = Value;
                    break;

                case "Four":
                    Four = Value;
                    break;

                case "Five":
                    Five = Value;
                    break;

                case "Six":
                    Six = Value;
                    break;

                case "Seven":
                    Seven = Value;
                    break;

                case "Eight":
                    Eight = Value;
                    break;

                case "Nine":
                    Nine = Value;
                    break;

                case "Ten":
                    Ten = Value;
                    break;

                case "Eleven":
                    Eleven = Value;
                    break;

                case "Twelve":
                    Twelve = Value;
                    break;

                case "Print":
                    Print = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

                case "Share":
                    Share = Value;
                    break;

                case "Describe":
                    Describe = Value;
                    break;

                case "Add":
                    Add = Value;
                    break;

                case "Remove":
                    Remove = Value;
                    break;

                case "Missing Image":
                    MissingImage = Value;
                    break;

                case "Missing Banner":
                    MissingBanner = Value;
                    break;

                case "My Library":
                    MyLibrary = Value;
                    break;

                case "My Account":
                    MyAccount = Value;
                    break;

                case "Internal":
                    Internal = Value;
                    break;

                case "System Admin":
                    SystemAdmin = Value;
                    break;

                case "Portal Admin":
                    PortalAdmin = Value;
                    break;

                case "ACTIONS":
                    ACTIONS = Value;
                    break;

                case "Delete":
                    Delete = Value;
                    break;

                case "View":
                    View = Value;
                    break;

                case "XXX Is A Required Field":
                    XXXIsARequiredField = Value;
                    break;

                case "The Following Errors Were Detected":
                    TheFollowingErrorsWereDetected = Value;
                    break;

            }
        }
        /// <remarks> 'my' localization string </remarks>
        public string My { get; private set; }

        /// <remarks> 'Close' localization string </remarks>
        public string Close { get; private set; }

        /// <remarks> 'Search' localization string </remarks>
        public string Search { get; private set; }

        /// <remarks> 'Help' localization string </remarks>
        public string Help { get; private set; }

        /// <remarks> 'Submit' localization string </remarks>
        public string Submit { get; private set; }

        /// <remarks> 'Save' localization string </remarks>
        public string Save { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> 'Sort by' localization string </remarks>
        public string SortBy { get; private set; }

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

        /// <remarks> 'January' localization string </remarks>
        public string January { get; private set; }

        /// <remarks> 'February' localization string </remarks>
        public string February { get; private set; }

        /// <remarks> 'March' localization string </remarks>
        public string March { get; private set; }

        /// <remarks> 'April ' localization string </remarks>
        public string April { get; private set; }

        /// <remarks> 'May ' localization string </remarks>
        public string May { get; private set; }

        /// <remarks> 'June' localization string </remarks>
        public string June { get; private set; }

        /// <remarks> 'July ' localization string </remarks>
        public string July { get; private set; }

        /// <remarks> 'August' localization string </remarks>
        public string August { get; private set; }

        /// <remarks> 'September' localization string </remarks>
        public string September { get; private set; }

        /// <remarks> 'October' localization string </remarks>
        public string October { get; private set; }

        /// <remarks> 'November' localization string </remarks>
        public string November { get; private set; }

        /// <remarks> 'December' localization string </remarks>
        public string December { get; private set; }

        /// <remarks> 'Unknown' localization string </remarks>
        public string Unknown { get; private set; }

        /// <remarks> 'Error' localization string </remarks>
        public string Error { get; private set; }

        /// <remarks> 'Total' localization string </remarks>
        public string Total { get; private set; }

        /// <remarks> 'BibID' localization string </remarks>
        public string Bibid { get; private set; }

        /// <remarks> 'VID' localization string </remarks>
        public string VID { get; private set; }

        /// <remarks> 'one' localization string </remarks>
        public string One { get; private set; }

        /// <remarks> 'two' localization string </remarks>
        public string Two { get; private set; }

        /// <remarks> 'three' localization string </remarks>
        public string Three { get; private set; }

        /// <remarks> 'four' localization string </remarks>
        public string Four { get; private set; }

        /// <remarks> 'five' localization string </remarks>
        public string Five { get; private set; }

        /// <remarks> 'six' localization string </remarks>
        public string Six { get; private set; }

        /// <remarks> 'seven' localization string </remarks>
        public string Seven { get; private set; }

        /// <remarks> 'eight' localization string </remarks>
        public string Eight { get; private set; }

        /// <remarks> 'nine' localization string </remarks>
        public string Nine { get; private set; }

        /// <remarks> 'ten' localization string </remarks>
        public string Ten { get; private set; }

        /// <remarks> 'eleven' localization string </remarks>
        public string Eleven { get; private set; }

        /// <remarks> 'twelve' localization string </remarks>
        public string Twelve { get; private set; }

        /// <remarks> 'Print' localization string </remarks>
        public string Print { get; private set; }

        /// <remarks> 'Send' localization string </remarks>
        public string Send { get; private set; }

        /// <remarks> 'Share' localization string </remarks>
        public string Share { get; private set; }

        /// <remarks> 'Describe' localization string </remarks>
        public string Describe { get; private set; }

        /// <remarks> 'Add' localization string </remarks>
        public string Add { get; private set; }

        /// <remarks> 'Remove' localization string </remarks>
        public string Remove { get; private set; }

        /// <remarks> 'Missing Image' localization string </remarks>
        public string MissingImage { get; private set; }

        /// <remarks> 'Missing Banner' localization string </remarks>
        public string MissingBanner { get; private set; }

        /// <remarks> 'My Library' localization string </remarks>
        public string MyLibrary { get; private set; }

        /// <remarks> 'My Account' localization string </remarks>
        public string MyAccount { get; private set; }

        /// <remarks> 'Internal' localization string </remarks>
        public string Internal { get; private set; }

        /// <remarks> 'System Admin' localization string </remarks>
        public string SystemAdmin { get; private set; }

        /// <remarks> 'Portal Admin' localization string </remarks>
        public string PortalAdmin { get; private set; }

        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

        /// <remarks> 'delete' localization string </remarks>
        public string Delete { get; private set; }

        /// <remarks> 'view' localization string </remarks>
        public string View { get; private set; }

        /// <remarks> '{0} is a required field' localization string </remarks>
        public string XXXIsARequiredField { get; private set; }

        /// <remarks> 'The following errors were detected:' localization string </remarks>
        public string TheFollowingErrorsWereDetected { get; private set; }

    }
}
