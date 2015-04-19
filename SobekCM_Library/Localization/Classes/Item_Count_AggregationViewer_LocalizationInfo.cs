namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Item_Count_AggregationViewer class </summary>
    public class Item_Count_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Item_Count_AggregationViewer_Localization class </summary>
        public Item_Count_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Item_Count_AggregationViewer";
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
                case "Resource Count In Aggregation":
                    ResourceCountInAggregation = Value;
                    break;

                case "Below Is The Number Of Titles And Items For All Items Within This Aggregation Including Currently Online Items As Well As Items In Process":
                    BelowIsTheNumberOfTitlesAndItemsForAllIt = Value;
                    break;

                case "Last Milestone":
                    LastMilestone = Value;
                    break;

                case "Title Count":
                    TitleCount = Value;
                    break;

                case "Item Count":
                    ItemCount = Value;
                    break;

                case "Page Count":
                    PageCount = Value;
                    break;

                case "File Count":
                    FileCount = Value;
                    break;

            }
        }
        /// <remarks> 'Resource Count in Aggregation' localization string </remarks>
        public string ResourceCountInAggregation { get; private set; }

        /// <remarks> '"Below is the number of titles and items for all items within this aggregation, including currently online items as well as items in process."' localization string </remarks>
        public string BelowIsTheNumberOfTitlesAndItemsForAllIt { get; private set; }

        /// <remarks> 'Last Milestone' localization string </remarks>
        public string LastMilestone { get; private set; }

        /// <remarks> 'Title Count' localization string </remarks>
        public string TitleCount { get; private set; }

        /// <remarks> 'Item Count' localization string </remarks>
        public string ItemCount { get; private set; }

        /// <remarks> 'Page Count' localization string </remarks>
        public string PageCount { get; private set; }

        /// <remarks> 'File Count' localization string </remarks>
        public string FileCount { get; private set; }

    }
}
