namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the URL_Email_Helper class </summary>
    public class URL_Email_Helper_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the URL_Email_Helper_Localization class </summary>
        public URL_Email_Helper_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "URL_Email_Helper";
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
                case "XXX Wanted You To See These Search Results On XXX And Included The Following Comments":
                    XXXWantedYouToSeeTheseSearchResultsOnXXXAndIncludedTheFollowingComments = Value;
                    break;

                case "XXX Wanted You To See These Search Results On XXX":
                    XXXWantedYouToSeeTheseSearchResultsOnXXX = Value;
                    break;

                case "XXX Wanted You To See This XXX On XXX And Included The Following Comments":
                    XXXWantedYouToSeeThisXXXOnXXXAndIncludedTheFollowingComments = Value;
                    break;

                case "XXX Wanted You To See This On XXX":
                    XXXWantedYouToSeeThisOnXXX = Value;
                    break;

                case "XXX Wanted You To See This Page On XXX And Included The Following Comments":
                    XXXWantedYouToSeeThisPageOnXXXAndIncludedTheFollowingComments = Value;
                    break;

                case "XXX Wanted You To See This Page On XXX":
                    XXXWantedYouToSeeThisPageOnXXX = Value;
                    break;

                case "XXX XXX From XXX":
                    XXXXXXFromXXX = Value;
                    break;

                case "Item From XXX":
                    ItemFromXXX = Value;
                    break;

                case "Page From XXX":
                    PageFromXXX = Value;
                    break;

                case "Search Results From XXX":
                    SearchResultsFromXXX = Value;
                    break;

                case "Title XXX":
                    TitleXXX = Value;
                    break;

            }
        }
        /// <remarks> When someone emails any search result page from the system and includes comments </remarks>
        public string XXXWantedYouToSeeTheseSearchResultsOnXXXAndIncludedTheFollowingComments { get; private set; }

        /// <remarks> "%1=username, %2=SobekCM instance abbreviation (like UFDC, dLOC). When someone emails any search result page from the system" </remarks>
        public string XXXWantedYouToSeeTheseSearchResultsOnXXX { get; private set; }

        /// <remarks> '%1 wanted you to see this %2 on %3 and included the following comments' localization string </remarks>
        public string XXXWantedYouToSeeThisXXXOnXXXAndIncludedTheFollowingComments { get; private set; }

        /// <remarks> '%1 wanted you to see this on %2' localization string </remarks>
        public string XXXWantedYouToSeeThisOnXXX { get; private set; }

        /// <remarks> When someone emails any other web page from the system and includes comments </remarks>
        public string XXXWantedYouToSeeThisPageOnXXXAndIncludedTheFollowingComments { get; private set; }

        /// <remarks> When someone emails any other web page from the system </remarks>
        public string XXXWantedYouToSeeThisPageOnXXX { get; private set; }

        /// <remarks> '"%1,%2 from %3"' localization string </remarks>
        public string XXXXXXFromXXX { get; private set; }

        /// <remarks> "Used as the subject line of that email.  ""Item from dLOC""" </remarks>
        public string ItemFromXXX { get; private set; }

        /// <remarks> "Used as the subject line of that email.  ""Page from dLOC""" </remarks>
        public string PageFromXXX { get; private set; }

        /// <remarks> "Used as the subject line of that email. ""Search results from dLOC""" </remarks>
        public string SearchResultsFromXXX { get; private set; }

        /// <remarks> 'Title: %1' localization string </remarks>
        public string TitleXXX { get; private set; }

    }
}
