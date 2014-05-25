namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Saved_Searches_MySobekViewer class </summary>
    public class Saved_Searches_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Saved_Searches_MySobekViewer_Localization class </summary>
        public Saved_Searches_MySobekViewer_LocalizationInfo() : base()
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
                case "ACTIONS":
                    ACTIONS = Value;
                    break;

                case "Click To Delete This Saved Search":
                    ClickToDeleteThisSavedSearch = Value;
                    break;

                case "Click To View This Search":
                    ClickToViewThisSearch = Value;
                    break;

                case "Delete":
                    Delete = Value;
                    break;

                case "My Saved Searches":
                    MySavedSearches = Value;
                    break;

                case "SAVED SEARCH":
                    SAVEDSEARCH = Value;
                    break;

                case "You Do Not Have Any Saved Searches Or Browses Br Br To Add A Search Or Browse Use The ADD Button While Viewing The Results Of Your Search Or Browse":
                    YouDoNotHaveAnySavedSearchesOrBrowsesBrBrToAddASearchOrBrowseUseTheADDButtonWhileViewingTheResultsOfYourSearchOrBrowse = Value;
                    break;

            }
        }
        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

        /// <remarks> Hover over text for the delete action to remove an existing saved search </remarks>
        public string ClickToDeleteThisSavedSearch { get; private set; }

        /// <remarks> Hover over text for the view action to see the saved search </remarks>
        public string ClickToViewThisSearch { get; private set; }

        /// <remarks> 'delete' localization string </remarks>
        public string Delete { get; private set; }

        /// <remarks> Title for the saved searches viewer </remarks>
        public string MySavedSearches { get; private set; }

        /// <remarks> Header for the table which lists all the saved searches </remarks>
        public string SAVEDSEARCH { get; private set; }

        /// <remarks> Prompt provided if the user does not have any saved searches </remarks>
        public string YouDoNotHaveAnySavedSearchesOrBrowsesBrBrToAddASearchOrBrowseUseTheADDButtonWhileViewingTheResultsOfYourSearchOrBrowse { get; private set; }

    }
}
