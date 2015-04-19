namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Saved_Searches_MySobekViewer class </summary>
    public class Saved_Searches_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Saved_Searches_MySobekViewer_Localization class </summary>
        public Saved_Searches_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Saved_Searches_MySobekViewer";
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
                case "My Saved Searches":
                    MySavedSearches = Value;
                    break;

                case "You Do Not Have Any Saved Searches Or Browses Br Br To Add A Search Or Browse Use The ADD Button While Viewing The Results Of Your Search Or Browse":
                    YouDoNotHaveAnySavedSearchesOrBrowsesBrB = Value;
                    break;

                case "SAVED SEARCH":
                    SAVEDSEARCH = Value;
                    break;

                case "Click To Delete This Saved Search":
                    ClickToDeleteThisSavedSearch = Value;
                    break;

                case "Click To View This Search":
                    ClickToViewThisSearch = Value;
                    break;

                case "ACTIONS":
                    ACTIONS = Value;
                    break;

                case "Delete":
                    Delete = Value;
                    break;

            }
        }
        /// <remarks> 'My Saved Searches' localization string </remarks>
        public string MySavedSearches { get; private set; }

        /// <remarks> '"You do not have any saved searches or browses.<br /><br />To add a search or browse, use the ADD button while viewing the results of your search or browse."' localization string </remarks>
        public string YouDoNotHaveAnySavedSearchesOrBrowsesBrB { get; private set; }

        /// <remarks> 'SAVED SEARCH' localization string </remarks>
        public string SAVEDSEARCH { get; private set; }

        /// <remarks> 'Click to delete this saved search' localization string </remarks>
        public string ClickToDeleteThisSavedSearch { get; private set; }

        /// <remarks> 'Click to view this search' localization string </remarks>
        public string ClickToViewThisSearch { get; private set; }

        /// <remarks> 'ACTIONS' localization string </remarks>
        public string ACTIONS { get; private set; }

        /// <remarks> 'delete' localization string </remarks>
        public string Delete { get; private set; }

    }
}
