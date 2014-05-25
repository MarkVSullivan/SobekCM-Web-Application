namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the EAD_Description_ItemViewer class </summary>
    public class EAD_Description_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the EAD_Description_ItemViewer_Localization class </summary>
        public EAD_Description_ItemViewer_LocalizationInfo() : base()
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
                case "Archival Description":
                    ArchivalDescription = Value;
                    break;

            }
        }
        /// <remarks> Title for the archival description viewer </remarks>
        public string ArchivalDescription { get; private set; }

    }
}
