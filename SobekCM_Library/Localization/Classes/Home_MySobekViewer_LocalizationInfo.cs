namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Home_MySobekViewer class </summary>
    public class Home_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Home_MySobekViewer_Localization class </summary>
        public Home_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Home_MySobekViewer";
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
                case "Welcome To XXX XXX":
                    WelcomeToXXXXXX = Value;
                    break;

                case "Welcome Back XXX":
                    WelcomeBackXXX = Value;
                    break;

                case "Welcome To XXX This Feature Allows You To Add Items To Your Bookshelves Organize Your Bookshelves And Email Your Bookshelves To Friends":
                    WelcomeToXXXThisFeatureAllowsYouToAddItem = Value;
                    break;

                case "What Would You Like To Do Today":
                    WhatWouldYouLikeToDoToday = Value;
                    break;

                case "Start A New Item":
                    StartANewItem = Value;
                    break;

                case "Online Submittals Are Temporarily Disabled":
                    OnlineSubmittalsAreTemporarilyDisabled = Value;
                    break;

                case "View All My Submitted Items":
                    ViewAllMySubmittedItems = Value;
                    break;

                case "View Usage For My Items":
                    ViewUsageForMyItems = Value;
                    break;

                case "View My Descriptive Tags":
                    ViewMyDescriptiveTags = Value;
                    break;

                case "View And Organize My Bookshelves":
                    ViewAndOrganizeMyBookshelves = Value;
                    break;

                case "View My Saved Searches":
                    ViewMySavedSearches = Value;
                    break;

                case "Edit My Account Preferences":
                    EditMyAccountPreferences = Value;
                    break;

                case "Track Item Scanningprocessing":
                    TrackItemScanningprocessing = Value;
                    break;

                case "Return To XXX":
                    ReturnToXXX = Value;
                    break;

                case "Return To Previous XXX Page":
                    ReturnToPreviousXXXPage = Value;
                    break;

                case "Log Out":
                    LogOut = Value;
                    break;

                case "Comments Or Recommendations Please Xxxcontact Usxxx":
                    CommentsOrRecommendationsPleaseXxxcontactUsxx = Value;
                    break;

                case "If You Would Like To Contribute Materials Through The Online System Please Xxxcontact Usxxx As Well":
                    IfYouWouldLikeToContributeMaterialsThrough = Value;
                    break;

            }
        }
        /// <remarks> '"Welcome to {0}, {1}"' localization string </remarks>
        public string WelcomeToXXXXXX { get; private set; }

        /// <remarks> '"Welcome back, {0}"' localization string </remarks>
        public string WelcomeBackXXX { get; private set; }

        /// <remarks> '"Welcome to {0}.  This feature allows you to add items to your bookshelves, organize your bookshelves, and email your bookshelves to friends."' localization string </remarks>
        public string WelcomeToXXXThisFeatureAllowsYouToAddItem { get; private set; }

        /// <remarks> 'What would you like to do today?' localization string </remarks>
        public string WhatWouldYouLikeToDoToday { get; private set; }

        /// <remarks> 'Start a new item' localization string </remarks>
        public string StartANewItem { get; private set; }

        /// <remarks> 'Online submittals are temporarily disabled' localization string </remarks>
        public string OnlineSubmittalsAreTemporarilyDisabled { get; private set; }

        /// <remarks> 'View all my submitted items' localization string </remarks>
        public string ViewAllMySubmittedItems { get; private set; }

        /// <remarks> 'View usage for my items' localization string </remarks>
        public string ViewUsageForMyItems { get; private set; }

        /// <remarks> 'View my descriptive tags' localization string </remarks>
        public string ViewMyDescriptiveTags { get; private set; }

        /// <remarks> 'View and organize my bookshelves' localization string </remarks>
        public string ViewAndOrganizeMyBookshelves { get; private set; }

        /// <remarks> 'View my saved searches' localization string </remarks>
        public string ViewMySavedSearches { get; private set; }

        /// <remarks> 'Edit my account preferences' localization string </remarks>
        public string EditMyAccountPreferences { get; private set; }

        /// <remarks> 'Track Item Scanning/Processing' localization string </remarks>
        public string TrackItemScanningprocessing { get; private set; }

        /// <remarks> 'Return to {0}' localization string </remarks>
        public string ReturnToXXX { get; private set; }

        /// <remarks> 'Return to previous %1 page' localization string </remarks>
        public string ReturnToPreviousXXXPage { get; private set; }

        /// <remarks> 'Log Out' localization string </remarks>
        public string LogOut { get; private set; }

        /// <remarks> 'Comments or recommendations?  Please {0}contact us{1}.' localization string </remarks>
        public string CommentsOrRecommendationsPleaseXxxcontactUsxx { get; private set; }

        /// <remarks> '"If you would like to contribute materials through the online system, please {0}contact us{1} as well."' localization string </remarks>
        public string IfYouWouldLikeToContributeMaterialsThrough { get; private set; }

    }
}
