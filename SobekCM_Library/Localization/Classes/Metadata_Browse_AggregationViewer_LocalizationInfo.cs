namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Metadata_Browse_AggregationViewer class </summary>
    public class Metadata_Browse_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Metadata_Browse_AggregationViewer_Localization class </summary>
        public Metadata_Browse_AggregationViewer_LocalizationInfo() : base()
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
                case "Browse By XXX":
                    BrowseByXXX = Value;
                    break;

                case "Browse By":
                    BrowseBy = Value;
                    break;

                case "Internal Browses":
                    InternalBrowses = Value;
                    break;

                case "NO MATCHING VALUES":
                    NOMATCHINGVALUES = Value;
                    break;

                case "Public Browses":
                    PublicBrowses = Value;
                    break;

                case "Select A Metadata Field To Browse By From The List On The Left":
                    SelectAMetadataFieldToBrowseByFromTheListOnTheLeft = Value;
                    break;

            }
        }
        /// <remarks> "i.e., Browse by Title.  When metadata browsing, appears in gray box at top" </remarks>
        public string BrowseByXXX { get; private set; }

        /// <remarks> Text on the facet box to the left </remarks>
        public string BrowseBy { get; private set; }

        /// <remarks> Only shows up if logged on as internal user </remarks>
        public string InternalBrowses { get; private set; }

        /// <remarks> Displays if there no metadata values returned in the browse </remarks>
        public string NOMATCHINGVALUES { get; private set; }

        /// <remarks> Then all public browses are lists </remarks>
        public string PublicBrowses { get; private set; }

        /// <remarks> Instructions appears sometimes </remarks>
        public string SelectAMetadataFieldToBrowseByFromTheListOnTheLeft { get; private set; }

    }
}
