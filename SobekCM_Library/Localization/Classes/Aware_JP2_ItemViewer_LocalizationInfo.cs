namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Aware_JP2_ItemViewer class </summary>
    public class Aware_JP2_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Aware_JP2_ItemViewer_Localization class </summary>
        public Aware_JP2_ItemViewer_LocalizationInfo() : base()
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
                case "Click On Thumbnail To Recenter Image":
                    ClickOnThumbnailToRecenterImage = Value;
                    break;

                case "Current Zoom":
                    CurrentZoom = Value;
                    break;

                case "JPEG2000 IMAGE NOT FOUND IN DATABASE":
                    JPEG2000IMAGENOTFOUNDINDATABASE = Value;
                    break;

                case "Large Size View":
                    LargeSizeView = Value;
                    break;

                case "Medium Size View":
                    MediumSizeView = Value;
                    break;

                case "Mediumlarge Size View":
                    MediumlargeSizeView = Value;
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

                case "Pan Up":
                    PanUp = Value;
                    break;

                case "Rotate Clockwise":
                    RotateClockwise = Value;
                    break;

                case "Rotate Counter Clockwise":
                    RotateCounterClockwise = Value;
                    break;

                case "Small Size View":
                    SmallSizeView = Value;
                    break;

                case "THUMBNAIL":
                    THUMBNAIL = Value;
                    break;

                case "To Download Right Click Here Select Save Target As And Save The JPEG2000 To Your Local Computer":
                    ToDownloadRightClickHereSelectSaveTargetAsAndSaveTheJPEG2000ToYourLocalComputer = Value;
                    break;

                case "Zoom In":
                    ZoomIn = Value;
                    break;

                case "Zoom Out":
                    ZoomOut = Value;
                    break;

                case "Zoom To Level":
                    ZoomToLevel = Value;
                    break;

            }
        }
        /// <remarks> Text under the thumbnail in the nav menu </remarks>
        public string ClickOnThumbnailToRecenterImage { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string CurrentZoom { get; private set; }

        /// <remarks> 'JPEG2000 IMAGE NOT FOUND IN DATABASE!' localization string </remarks>
        public string JPEG2000IMAGENOTFOUNDINDATABASE { get; private set; }

        /// <remarks> 'Large size view' localization string </remarks>
        public string LargeSizeView { get; private set; }

        /// <remarks> 'Medium size view' localization string </remarks>
        public string MediumSizeView { get; private set; }

        /// <remarks> 'Medium-large size view' localization string </remarks>
        public string MediumlargeSizeView { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string PanDown { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string PanLeft { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string PanRight { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string PanUp { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string RotateClockwise { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string RotateCounterClockwise { get; private set; }

        /// <remarks> 'Small size view' localization string </remarks>
        public string SmallSizeView { get; private set; }

        /// <remarks> Title for the thumbnail in the nav menu </remarks>
        public string THUMBNAIL { get; private set; }

        /// <remarks> '"To download, right click here, select 'Save Target As...' and save the JPEG2000 to your local computer."' localization string </remarks>
        public string ToDownloadRightClickHereSelectSaveTargetAsAndSaveTheJPEG2000ToYourLocalComputer { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string ZoomIn { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string ZoomOut { get; private set; }

        /// <remarks> Buttons used when zooming in and out on an image in the Aware JP2 server </remarks>
        public string ZoomToLevel { get; private set; }

    }
}
