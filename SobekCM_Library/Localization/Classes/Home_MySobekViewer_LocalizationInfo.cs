namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Home_MySobekViewer class </summary>
    public class Home_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Home_MySobekViewer_Localization class </summary>
        public Home_MySobekViewer_LocalizationInfo() : base()
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
                case "Comments Or Recommendations Please Xxxcontact Usxxx":
                    CommentsOrRecommendationsPleaseXxxcontactUsxxx = Value;
                    break;

                case "Edit My Account Preferences":
                    EditMyAccountPreferences = Value;
                    break;

                case "If You Would Like To Contribute Materials Through The Online System Please Xxxcontact Usxxx As Well":
                    IfYouWouldLikeToContributeMaterialsThroughTheOnlineSystemPleaseXxxcontactUsxxxAsWell = Value;
                    break;

                case "Log Out":
                    LogOut = Value;
                    break;

                case "Online Submittals Are Temporarily Disabled":
                    OnlineSubmittalsAreTemporarilyDisabled = Value;
                    break;

                case "Return To XXX":
                    ReturnToXXX = Value;
                    break;

                case "Return To Previous XXX Page":
                    ReturnToPreviousXXXPage = Value;
                    break;

                case "Start A New Item":
                    StartANewItem = Value;
                    break;

                case "Track Item Scanningprocessing":
                    TrackItemScanningprocessing = Value;
                    break;

                case "View All My Submitted Items":
                    ViewAllMySubmittedItems = Value;
                    break;

                case "View And Organize My Bookshelves":
                    ViewAndOrganizeMyBookshelves = Value;
                    break;

                case "View My Descriptive Tags":
                    ViewMyDescriptiveTags = Value;
                    break;

                case "View My Saved Searches":
                    ViewMySavedSearches = Value;
                    break;

                case "View Usage For My Items":
                    ViewUsageForMyItems = Value;
                    break;

                case "Welcome Back XXX":
                    WelcomeBackXXX = Value;
                    break;

                case "Welcome To XXX XXX":
                    WelcomeToXXXXXX = Value;
                    break;

                case "Welcome To XXX This Feature Allows You To Add Items To Your Bookshelves Organize Your Bookshelves And Email Your Bookshelves To Friends":
                    WelcomeToXXXThisFeatureAllowsYouToAddItemsToYourBookshelvesOrganizeYourBookshelvesAndEmailYourBookshelvesToFriends = Value;
                    break;

                case "What Would You Like To Do Today":
                    WhatWouldYouLikeToDoToday = Value;
                    break;

            }
        }
        /// <remarks> Prompt at the bottom of the home page to provide assistance.  ( {0} and {1} are replaced with link-enabling HTML) </remarks>
        public string CommentsOrRecommendationsPleaseXxxcontactUsxxx { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string EditMyAccountPreferences { get; private set; }

        /// <remarks> Optional prompt if the user does not have rights to submit items online.  ( {0} and {1} are replaced with link-enabling HTML) </remarks>
        public string IfYouWouldLikeToContributeMaterialsThroughTheOnlineSystemPleaseXxxcontactUsxxxAsWell { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string LogOut { get; private set; }

        /// <remarks> Menu item within the mySobek home page if online submittals are disabled </remarks>
        public string OnlineSubmittalsAreTemporarilyDisabled { get; private set; }

        /// <remarks> "If they were forwarded from a spot in the system to logon, this returns them to that spot" </remarks>
        public string ReturnToXXX { get; private set; }

        /// <remarks> 'Return to previous %1 page' localization string </remarks>
        public string ReturnToPreviousXXXPage { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string StartANewItem { get; private set; }

        /// <remarks> 'Track Item Scanning/Processing' localization string </remarks>
        public string TrackItemScanningprocessing { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string ViewAllMySubmittedItems { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string ViewAndOrganizeMyBookshelves { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string ViewMyDescriptiveTags { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string ViewMySavedSearches { get; private set; }

        /// <remarks> Menu item within the mySobek home page </remarks>
        public string ViewUsageForMyItems { get; private set; }

        /// <remarks> "Message to welcome back a user by name ( Welcome back, Mark )" </remarks>
        public string WelcomeBackXXX { get; private set; }

        /// <remarks> "Message to welcome a newly registered user ( Welcome to dLOC, Mark )" </remarks>
        public string WelcomeToXXXXXX { get; private set; }

        /// <remarks> Longer welcome message and explanation for the mySobek home page </remarks>
        public string WelcomeToXXXThisFeatureAllowsYouToAddItemsToYourBookshelvesOrganizeYourBookshelvesAndEmailYourBookshelvesToFriends { get; private set; }

        /// <remarks> Prompt for user to select an action from the mySobek menu </remarks>
        public string WhatWouldYouLikeToDoToday { get; private set; }

    }
}
