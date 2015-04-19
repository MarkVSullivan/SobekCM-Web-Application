namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Map_Browse_AggregationViewer class </summary>
    public class Map_Browse_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Map_Browse_AggregationViewer_Localization class </summary>
        public Map_Browse_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Map_Browse_AggregationViewer";
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

                case "Click Here For More Information About This Title":
                    ClickHereForMoreInformationAboutThisTitle = Value;
                    break;

                case "Click Here For More Information About These XXX Titles":
                    ClickHereForMoreInformationAboutTheseXXXTi = Value;
                    break;

                case "Select A Point Below To View The Items From Or About That Location":
                    SelectAPointBelowToViewTheItemsFromOrAbo = Value;
                    break;

                case "Press The SHIFT Button And Then Drag A Box On The Map To Zoom In":
                    PressTheSHIFTButtonAndThenDragABoxOnThe = Value;
                    break;

            }
        }
        /// <remarks> '( %1 issues )' localization string </remarks>
        public string XXXIssues { get; private set; }

        /// <remarks> '( %1 volumes )' localization string </remarks>
        public string XXXVolumes { get; private set; }

        /// <remarks> 'Click here for more information about this title' localization string </remarks>
        public string ClickHereForMoreInformationAboutThisTitle { get; private set; }

        /// <remarks> 'Click here for more information about these %1 titles' localization string </remarks>
        public string ClickHereForMoreInformationAboutTheseXXXTi { get; private set; }

        /// <remarks> 'Select a point below to view the items from or about that location.' localization string </remarks>
        public string SelectAPointBelowToViewTheItemsFromOrAbo { get; private set; }

        /// <remarks> '"Press the SHIFT button, and then drag a box on the map to zoom in."' localization string </remarks>
        public string PressTheSHIFTButtonAndThenDragABoxOnThe { get; private set; }

    }
}
