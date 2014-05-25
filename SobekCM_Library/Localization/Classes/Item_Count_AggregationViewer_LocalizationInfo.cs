namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Item_Count_AggregationViewer class </summary>
    public class Item_Count_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Item_Count_AggregationViewer_Localization class </summary>
        public Item_Count_AggregationViewer_LocalizationInfo() : base()
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
                case "Below Is The Number Of Titles And Items For All Items Within This Aggregation Including Currently Online Items As Well As Items In Process":
                    BelowIsTheNumberOfTitlesAndItemsForAllItemsWithinThisAggregationIncludingCurrentlyOnlineItemsAsWellAsItemsInProcess = Value;
                    break;

                case "File Count":
                    FileCount = Value;
                    break;

                case "Item Count":
                    ItemCount = Value;
                    break;

                case "Last Milestone":
                    LastMilestone = Value;
                    break;

                case "Page Count":
                    PageCount = Value;
                    break;

                case "Resource Count In Aggregation":
                    ResourceCountInAggregation = Value;
                    break;

                case "Title Count":
                    TitleCount = Value;
                    break;

            }
        }
        /// <remarks> From the admin view of the number of items within the collection. Accessible from the admin internal header. </remarks>
        public string BelowIsTheNumberOfTitlesAndItemsForAllItemsWithinThisAggregationIncludingCurrentlyOnlineItemsAsWellAsItemsInProcess { get; private set; }

        /// <remarks> From the admin view of the number of items within the collection. Accessible from the admin internal header. </remarks>
        public string FileCount { get; private set; }

        /// <remarks> From the admin view of the number of items within the collection. Accessible from the admin internal header. </remarks>
        public string ItemCount { get; private set; }

        /// <remarks> From the admin view of the number of items within the collection. Accessible from the admin internal header. </remarks>
        public string LastMilestone { get; private set; }

        /// <remarks> From the admin view of the number of items within the collection. Accessible from the admin internal header. </remarks>
        public string PageCount { get; private set; }

        /// <remarks> 'Resource Count in Aggregation' localization string </remarks>
        public string ResourceCountInAggregation { get; private set; }

        /// <remarks> From the admin view of the number of items within the collection. Accessible from the admin internal header. </remarks>
        public string TitleCount { get; private set; }

    }
}
