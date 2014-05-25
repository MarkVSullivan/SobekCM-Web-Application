namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Map_Browse_AggregationViewer class </summary>
    public class Map_Browse_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Map_Browse_AggregationViewer_Localization class </summary>
        public Map_Browse_AggregationViewer_LocalizationInfo() : base()
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
                case "XXX Issues":
                    XXXIssues = Value;
                    break;

                case "XXX Volumes":
                    XXXVolumes = Value;
                    break;

                case "Click Here For More Information About These XXX Titles":
                    ClickHereForMoreInformationAboutTheseXXXTitles = Value;
                    break;

                case "Click Here For More Information About This Title":
                    ClickHereForMoreInformationAboutThisTitle = Value;
                    break;

                case "Press The SHIFT Button And Then Drag A Box On The Map To Zoom In":
                    PressTheSHIFTButtonAndThenDragABoxOnTheMapToZoomIn = Value;
                    break;

                case "Select A Point Below To View The Items From Or About That Location":
                    SelectAPointBelowToViewTheItemsFromOrAboutThatLocation = Value;
                    break;

            }
        }
        /// <remarks> "i.e., ( 12 issues ). Used when the content type is newspaper Used in the popup window when you select a point in a map browse" </remarks>
        public string XXXIssues { get; private set; }

        /// <remarks> "i.2., ( 12 volumes ). Used when not newspapers.  Used in the popup window when you select a point in a map browse" </remarks>
        public string XXXVolumes { get; private set; }

        /// <remarks> %1 = number of titles.  Used in the popup window when you select a point in a map browse </remarks>
        public string ClickHereForMoreInformationAboutTheseXXXTitles { get; private set; }

        /// <remarks> Used in the popup window when you select a point in a map browse </remarks>
        public string ClickHereForMoreInformationAboutThisTitle { get; private set; }

        /// <remarks> Appears in the search box during geographic browse </remarks>
        public string PressTheSHIFTButtonAndThenDragABoxOnTheMapToZoomIn { get; private set; }

        /// <remarks> Appears in the search box during geographic browse </remarks>
        public string SelectAPointBelowToViewTheItemsFromOrAboutThatLocation { get; private set; }

    }
}
