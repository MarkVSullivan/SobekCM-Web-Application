namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Metadata_Browse_AggregationViewer class </summary>
    public class Metadata_Browse_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Metadata_Browse_AggregationViewer_Localization class </summary>
        public Metadata_Browse_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Metadata_Browse_AggregationViewer";
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
                case "Browse By XXX":
                    BrowseByXXX = Value;
                    break;

                case "Browse By":
                    BrowseBy = Value;
                    break;

                case "Public Browses":
                    PublicBrowses = Value;
                    break;

                case "Internal Browses":
                    InternalBrowses = Value;
                    break;

                case "Select A Metadata Field To Browse By From The List On The Left":
                    SelectAMetadataFieldToBrowseByFromTheList = Value;
                    break;

                case "NO MATCHING VALUES":
                    NOMATCHINGVALUES = Value;
                    break;

            }
        }
        /// <remarks> 'Browse by %1' localization string </remarks>
        public string BrowseByXXX { get; private set; }

        /// <remarks> 'Browse By:' localization string </remarks>
        public string BrowseBy { get; private set; }

        /// <remarks> 'Public Browses' localization string </remarks>
        public string PublicBrowses { get; private set; }

        /// <remarks> 'Internal Browses' localization string </remarks>
        public string InternalBrowses { get; private set; }

        /// <remarks> 'Select a metadata field to browse by from the list on the left' localization string </remarks>
        public string SelectAMetadataFieldToBrowseByFromTheList { get; private set; }

        /// <remarks> 'NO MATCHING VALUES' localization string </remarks>
        public string NOMATCHINGVALUES { get; private set; }

    }
}
