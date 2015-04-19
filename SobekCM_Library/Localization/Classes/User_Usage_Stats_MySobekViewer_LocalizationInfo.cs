namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the User_Usage_Stats_MySobekViewer class </summary>
    public class User_Usage_Stats_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the User_Usage_Stats_MySobekViewer_Localization class </summary>
        public User_Usage_Stats_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "User_Usage_Stats_MySobekViewer";
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
                case "Below Is A List Of Items Associated With Your Account Including Usage Statistics Total Views And Visits Represents The Total Amount Of Usage Since The Item Was Added To The Library And The Monthly Views And Visits Is The Usage In The Selected Month":
                    BelowIsAListOfItemsAssociatedWithYourAcco = Value;
                    break;

                case "For More Information About These Terms See The":
                    ForMoreInformationAboutTheseTermsSeeThe = Value;
                    break;

                case "Definitions On The Main Statistics Page":
                    DefinitionsOnTheMainStatisticsPage = Value;
                    break;

                case "Select Any Column To Resort This Data":
                    SelectAnyColumnToResortThisData = Value;
                    break;

                case "TITLE":
                    TITLE = Value;
                    break;

                case "TOTAL VIEWS":
                    TOTALVIEWS = Value;
                    break;

                case "TOTAL VISITS":
                    TOTALVISITS = Value;
                    break;

                case "MONTHLY VIEWS":
                    MONTHLYVIEWS = Value;
                    break;

            }
        }
        /// <remarks> 'Below is a list of items associated with your account including usage statistics.  Total views and visits represents the total amount of usage since the item was added to the library and the monthly views and visits is the usage in the selected month.' localization string </remarks>
        public string BelowIsAListOfItemsAssociatedWithYourAcco { get; private set; }

        /// <remarks> '"For more information about these terms, see the"' localization string </remarks>
        public string ForMoreInformationAboutTheseTermsSeeThe { get; private set; }

        /// <remarks> 'definitions on the main statistics page' localization string </remarks>
        public string DefinitionsOnTheMainStatisticsPage { get; private set; }

        /// <remarks> 'Select any column to re-sort this data' localization string </remarks>
        public string SelectAnyColumnToResortThisData { get; private set; }

        /// <remarks> 'TITLE' localization string </remarks>
        public string TITLE { get; private set; }

        /// <remarks> 'TOTAL VIEWS' localization string </remarks>
        public string TOTALVIEWS { get; private set; }

        /// <remarks> 'TOTAL VISITS' localization string </remarks>
        public string TOTALVISITS { get; private set; }

        /// <remarks> 'MONTHLY VIEWS' localization string </remarks>
        public string MONTHLYVIEWS { get; private set; }

    }
}
