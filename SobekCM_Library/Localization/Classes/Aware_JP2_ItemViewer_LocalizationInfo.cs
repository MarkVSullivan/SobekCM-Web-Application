namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Aware_JP2_ItemViewer class </summary>
    public class Aware_JP2_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Aware_JP2_ItemViewer_Localization class </summary>
        public Aware_JP2_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Aware_JP2_ItemViewer";
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
                case "Zoom Out":
                    ZoomOut = Value;
                    break;

                case "Zoom To Level":
                    ZoomToLevel = Value;
                    break;

                case "Current Zoom":
                    CurrentZoom = Value;
                    break;

                case "Zoom In":
                    ZoomIn = Value;
                    break;

                case "Rotate Clockwise":
                    RotateClockwise = Value;
                    break;

                case "Rotate Counter Clockwise":
                    RotateCounterClockwise = Value;
                    break;

                case "Pan Up":
                    PanUp = Value;
                    break;

                case "Pan Down":
                    PanDown = Value;
                    break;

                case "Pan Left":
                    PanLeft = Value;
                    break;

                case "Pan Right":
                    PanRight = Value;
                    break;

                case "Click On Thumbnail To Recenter Image":
                    ClickOnThumbnailToRecenterImage = Value;
                    break;

                case "THUMBNAIL":
                    THUMBNAIL = Value;
                    break;

                case "Small Size View":
                    SmallSizeView = Value;
                    break;

                case "Medium Size View":
                    MediumSizeView = Value;
                    break;

                case "Mediumlarge Size View":
                    MediumlargeSizeView = Value;
                    break;

                case "Large Size View":
                    LargeSizeView = Value;
                    break;

                case "To Download Right Click Here Select Save Target As And Save The JPEG2000 To Your Local Computer":
                    ToDownloadRightClickHereSelectSaveTargetAs = Value;
                    break;

                case "JPEG2000 IMAGE NOT FOUND IN DATABASE":
                    JPEG2000IMAGENOTFOUNDINDATABASE = Value;
                    break;

            }
        }
        /// <remarks> 'Zoom Out' localization string </remarks>
        public string ZoomOut { get; private set; }

        /// <remarks> 'Zoom to Level' localization string </remarks>
        public string ZoomToLevel { get; private set; }

        /// <remarks> 'Current Zoom' localization string </remarks>
        public string CurrentZoom { get; private set; }

        /// <remarks> 'Zoom In' localization string </remarks>
        public string ZoomIn { get; private set; }

        /// <remarks> 'Rotate Clockwise' localization string </remarks>
        public string RotateClockwise { get; private set; }

        /// <remarks> 'Rotate Counter Clockwise' localization string </remarks>
        public string RotateCounterClockwise { get; private set; }

        /// <remarks> 'Pan Up' localization string </remarks>
        public string PanUp { get; private set; }

        /// <remarks> 'Pan Down' localization string </remarks>
        public string PanDown { get; private set; }

        /// <remarks> 'Pan Left' localization string </remarks>
        public string PanLeft { get; private set; }

        /// <remarks> 'Pan Right' localization string </remarks>
        public string PanRight { get; private set; }

        /// <remarks> 'Click on Thumbnail to Recenter Image' localization string </remarks>
        public string ClickOnThumbnailToRecenterImage { get; private set; }

        /// <remarks> 'THUMBNAIL' localization string </remarks>
        public string THUMBNAIL { get; private set; }

        /// <remarks> 'Small size view' localization string </remarks>
        public string SmallSizeView { get; private set; }

        /// <remarks> 'Medium size view' localization string </remarks>
        public string MediumSizeView { get; private set; }

        /// <remarks> 'Medium-large size view' localization string </remarks>
        public string MediumlargeSizeView { get; private set; }

        /// <remarks> 'Large size view' localization string </remarks>
        public string LargeSizeView { get; private set; }

        /// <remarks> '"To download, right click here, select 'Save Target As...' and save the JPEG2000 to your local computer."' localization string </remarks>
        public string ToDownloadRightClickHereSelectSaveTargetAs { get; private set; }

        /// <remarks> 'JPEG2000 IMAGE NOT FOUND IN DATABASE!' localization string </remarks>
        public string JPEG2000IMAGENOTFOUNDINDATABASE { get; private set; }

    }
}
