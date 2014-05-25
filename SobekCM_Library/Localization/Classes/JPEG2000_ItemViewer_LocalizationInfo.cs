namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the JPEG2000_ItemViewer class </summary>
    public class JPEG2000_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the JPEG2000_ItemViewer_Localization class </summary>
        public JPEG2000_ItemViewer_LocalizationInfo() : base()
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
                case "THUMBNAIL":
                    THUMBNAIL = Value;
                    break;

            }
        }
        /// <remarks> 'THUMBNAIL' localization string </remarks>
        public string THUMBNAIL { get; private set; }

    }
}
