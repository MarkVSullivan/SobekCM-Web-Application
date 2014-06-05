namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the JPEG_ItemViewer class </summary>
    public class JPEG_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the JPEG_ItemViewer_Localization class </summary>
        public JPEG_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "JPEG_ItemViewer";
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
                case "Click On Image Below To Switch To Zoomable Version":
                    ClickOnImageBelowToSwitchToZoomableVersion = Value;
                    break;

                case "Click On Image To Switch To Zoomable Version":
                    ClickOnImageToSwitchToZoomableVersion = Value;
                    break;

                case "MISSING IMAGE":
                    MISSINGIMAGE = Value;
                    break;

            }
        }
        /// <remarks> 'Click on image below to switch to zoomable version' localization string </remarks>
        public string ClickOnImageBelowToSwitchToZoomableVersion { get; private set; }

        /// <remarks> 'Click on image to switch to zoomable version' localization string </remarks>
        public string ClickOnImageToSwitchToZoomableVersion { get; private set; }

        /// <remarks> 'MISSING IMAGE' localization string </remarks>
        public string MISSINGIMAGE { get; private set; }

    }
}
