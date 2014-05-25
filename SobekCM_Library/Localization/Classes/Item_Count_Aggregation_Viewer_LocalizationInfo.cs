namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Item_Count_Aggregation_Viewer class </summary>
    public class Item_Count_Aggregation_Viewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Item_Count_Aggregation_Viewer_Localization class </summary>
        public Item_Count_Aggregation_Viewer_LocalizationInfo() : base()
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

                case "FILE COUNT":
                    FILECOUNT = Value;
                    break;

                case "ITEM COUNT":
                    ITEMCOUNT = Value;
                    break;

                case "LAST MILESTONE":
                    LASTMILESTONE = Value;
                    break;

                case "PAGE COUNT":
                    PAGECOUNT = Value;
                    break;

                case "Resource Count In Aggregation":
                    ResourceCountInAggregation = Value;
                    break;

                case "TITLE COUNT":
                    TITLECOUNT = Value;
                    break;

            }
        }
        /// <remarks> '"Below is the number of titles and items for all items within this aggregation, including currently online items as well as items in process."' localization string </remarks>
        public string BelowIsTheNumberOfTitlesAndItemsForAllItemsWithinThisAggregationIncludingCurrentlyOnlineItemsAsWellAsItemsInProcess { get; private set; }

        /// <remarks> 'FILE COUNT' localization string </remarks>
        public string FILECOUNT { get; private set; }

        /// <remarks> 'ITEM COUNT' localization string </remarks>
        public string ITEMCOUNT { get; private set; }

        /// <remarks> 'LAST MILESTONE' localization string </remarks>
        public string LASTMILESTONE { get; private set; }

        /// <remarks> 'PAGE COUNT' localization string </remarks>
        public string PAGECOUNT { get; private set; }

        /// <remarks> 'Resource Count in Aggregation' localization string </remarks>
        public string ResourceCountInAggregation { get; private set; }

        /// <remarks> 'TITLE COUNT' localization string </remarks>
        public string TITLECOUNT { get; private set; }

    }
}
