namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the User_Tags_MySobekViewer class </summary>
    public class User_Tags_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the User_Tags_MySobekViewer_Localization class </summary>
        public User_Tags_MySobekViewer_LocalizationInfo() : base()
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
                case "Added By XXX On XXX":
                    AddedByXXXOnXXX = Value;
                    break;

                case "Added By You On XXX":
                    AddedByYouOnXXX = Value;
                    break;

                case "All Aggregations":
                    AllAggregations = Value;
                    break;

                case "As A Digital Collection Manager Or Administrator You Can Use This Screen To View Descriptive Tags Added To Collections Of Interest As Well As View The Descriptive Tags You Have Added To Items":
                    AsADigitalCollectionManagerOrAdministratorYouCanUseThisScreenToViewDescriptiveTagsAddedToCollectionsOfInterestAsWellAsViewTheDescriptiveTagsYouHaveAddedToItems = Value;
                    break;

                case "Choose An Aggregation Below To View All Tags For That Aggregation":
                    ChooseAnAggregationBelowToViewAllTagsForThatAggregation = Value;
                    break;

                case "Tags By Aggregation":
                    TagsByAggregation = Value;
                    break;

                case "Tags By User":
                    TagsByUser = Value;
                    break;

                case "This User Has Not Added Any Descriptive Tags To Any Items Or It Is Not A Valid Userid":
                    ThisUserHasNotAddedAnyDescriptiveTagsToAnyItemsOrItIsNotAValidUserid = Value;
                    break;

                case "View All By This User":
                    ViewAllByThisUser = Value;
                    break;

                case "You Have Added The Following XXX Descriptive Tags":
                    YouHaveAddedTheFollowingXXXDescriptiveTags = Value;
                    break;

                case "You Have Not Added Any Descriptive Tags To Any Items":
                    YouHaveNotAddedAnyDescriptiveTagsToAnyItems = Value;
                    break;

                case "Your Descriptive Tags":
                    YourDescriptiveTags = Value;
                    break;

            }
        }
        /// <remarks> 'Added by %1 on %2' localization string </remarks>
        public string AddedByXXXOnXXX { get; private set; }

        /// <remarks> 'Added by you on %1' localization string </remarks>
        public string AddedByYouOnXXX { get; private set; }

        /// <remarks> 'All Aggregations' localization string </remarks>
        public string AllAggregations { get; private set; }

        /// <remarks> '"As a digital collection manager or administrator, you can use this screen to view descriptive tags added to collections of interest, as well as view the descriptive tags you have added to items."' localization string </remarks>
        public string AsADigitalCollectionManagerOrAdministratorYouCanUseThisScreenToViewDescriptiveTagsAddedToCollectionsOfInterestAsWellAsViewTheDescriptiveTagsYouHaveAddedToItems { get; private set; }

        /// <remarks> 'Choose an aggregation below to view all tags for that aggregation:' localization string </remarks>
        public string ChooseAnAggregationBelowToViewAllTagsForThatAggregation { get; private set; }

        /// <remarks> 'Tags By Aggregation' localization string </remarks>
        public string TagsByAggregation { get; private set; }

        /// <remarks> 'Tags By User' localization string </remarks>
        public string TagsByUser { get; private set; }

        /// <remarks> 'This user has not added any descriptive tags to any items or it is not a valid userid.' localization string </remarks>
        public string ThisUserHasNotAddedAnyDescriptiveTagsToAnyItemsOrItIsNotAValidUserid { get; private set; }

        /// <remarks> 'view all by this user' localization string </remarks>
        public string ViewAllByThisUser { get; private set; }

        /// <remarks> 'You have added the following %1 descriptive tags' localization string </remarks>
        public string YouHaveAddedTheFollowingXXXDescriptiveTags { get; private set; }

        /// <remarks> 'You have not added any descriptive tags to any items' localization string </remarks>
        public string YouHaveNotAddedAnyDescriptiveTagsToAnyItems { get; private set; }

        /// <remarks> 'Your Descriptive Tags' localization string </remarks>
        public string YourDescriptiveTags { get; private set; }

    }
}
