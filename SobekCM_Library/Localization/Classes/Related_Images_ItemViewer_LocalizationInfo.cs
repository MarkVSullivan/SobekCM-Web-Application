namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Related_Images_ItemViewer class </summary>
    public class Related_Images_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Related_Images_ItemViewer_Localization class </summary>
        public Related_Images_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Related_Images_ItemViewer";
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
                case "Thumbnails Per Page":
                    ThumbnailsPerPage = Value;
                    break;

                case "All Thumbnails":
                    AllThumbnails = Value;
                    break;

                case "Go To Thumbnail":
                    GoToThumbnail = Value;
                    break;

                case "Page":
                    Page = Value;
                    break;

                case "Small Thumbnails":
                    SmallThumbnails = Value;
                    break;

                case "Medium Thumbnails":
                    MediumThumbnails = Value;
                    break;

                case "Large Thumbnails":
                    LargeThumbnails = Value;
                    break;

                case "Switch To Small Thumbnails":
                    SwitchToSmallThumbnails = Value;
                    break;

                case "Switch To Medium Thumbnails":
                    SwitchToMediumThumbnails = Value;
                    break;

                case "Switch To Large Thumbnails":
                    SwitchToLargeThumbnails = Value;
                    break;

                case "Small":
                    Small = Value;
                    break;

                case "Medium":
                    Medium = Value;
                    break;

                case "Large":
                    Large = Value;
                    break;

                case "MISSING THUMBNAIL":
                    MISSINGTHUMBNAIL = Value;
                    break;

            }
        }
        /// <remarks> '% Thumbnails per page' localization string </remarks>
        public string ThumbnailsPerPage { get; private set; }

        /// <remarks> 'All Thumbnails' localization string </remarks>
        public string AllThumbnails { get; private set; }

        /// <remarks> 'Go to thumbnail:' localization string </remarks>
        public string GoToThumbnail { get; private set; }

        /// <remarks> 'Page' localization string </remarks>
        public string Page { get; private set; }

        /// <remarks> 'Small thumbnails' localization string </remarks>
        public string SmallThumbnails { get; private set; }

        /// <remarks> 'Medium thumbnails' localization string </remarks>
        public string MediumThumbnails { get; private set; }

        /// <remarks> 'Large thumbnails' localization string </remarks>
        public string LargeThumbnails { get; private set; }

        /// <remarks> 'Switch to small thumbnails' localization string </remarks>
        public string SwitchToSmallThumbnails { get; private set; }

        /// <remarks> 'Switch to medium thumbnails' localization string </remarks>
        public string SwitchToMediumThumbnails { get; private set; }

        /// <remarks> 'Switch to large thumbnails' localization string </remarks>
        public string SwitchToLargeThumbnails { get; private set; }

        /// <remarks> 'Small' localization string </remarks>
        public string Small { get; private set; }

        /// <remarks> 'Medium' localization string </remarks>
        public string Medium { get; private set; }

        /// <remarks> 'Large' localization string </remarks>
        public string Large { get; private set; }

        /// <remarks> 'MISSING THUMBNAIL' localization string </remarks>
        public string MISSINGTHUMBNAIL { get; private set; }

    }
}
