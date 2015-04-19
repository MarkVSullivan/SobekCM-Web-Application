namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the MultiVolumes_ItemViewer class </summary>
    public class MultiVolumes_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the MultiVolumes_ItemViewer_Localization class </summary>
        public MultiVolumes_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "MultiVolumes_ItemViewer";
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
                case "All Issues":
                    AllIssues = Value;
                    break;

                case "Related Maps":
                    RelatedMaps = Value;
                    break;

                case "MISSING THUMBNAIL":
                    MISSINGTHUMBNAIL = Value;
                    break;

                case "Related Map Sets":
                    RelatedMapSets = Value;
                    break;

                case "Related Flights":
                    RelatedFlights = Value;
                    break;

                case "All Volumes":
                    AllVolumes = Value;
                    break;

                case "Tree View":
                    TreeView = Value;
                    break;

                case "Thumbnails":
                    Thumbnails = Value;
                    break;

                case "List View":
                    ListView = Value;
                    break;

                case "Level 1":
                    Level1 = Value;
                    break;

                case "Level 2":
                    Level2 = Value;
                    break;

                case "Level 3":
                    Level3 = Value;
                    break;

                case "Access":
                    Access = Value;
                    break;

                case "Public":
                    Public = Value;
                    break;

                case "Private":
                    Private = Value;
                    break;

                case "Restricted":
                    Restricted = Value;
                    break;

                case "Dark":
                    Dark = Value;
                    break;

            }
        }
        /// <remarks> 'All Issues' localization string </remarks>
        public string AllIssues { get; private set; }

        /// <remarks> 'Related Maps' localization string </remarks>
        public string RelatedMaps { get; private set; }

        /// <remarks> 'MISSING THUMBNAIL' localization string </remarks>
        public string MISSINGTHUMBNAIL { get; private set; }

        /// <remarks> 'Related Map sets' localization string </remarks>
        public string RelatedMapSets { get; private set; }

        /// <remarks> 'Related Flights' localization string </remarks>
        public string RelatedFlights { get; private set; }

        /// <remarks> 'All Volumes' localization string </remarks>
        public string AllVolumes { get; private set; }

        /// <remarks> 'Tree View' localization string </remarks>
        public string TreeView { get; private set; }

        /// <remarks> 'Thumbnails' localization string </remarks>
        public string Thumbnails { get; private set; }

        /// <remarks> 'List View' localization string </remarks>
        public string ListView { get; private set; }

        /// <remarks> 'Level 1' localization string </remarks>
        public string Level1 { get; private set; }

        /// <remarks> 'Level 2' localization string </remarks>
        public string Level2 { get; private set; }

        /// <remarks> 'Level 3' localization string </remarks>
        public string Level3 { get; private set; }

        /// <remarks> 'Access' localization string </remarks>
        public string Access { get; private set; }

        /// <remarks> 'public' localization string </remarks>
        public string Public { get; private set; }

        /// <remarks> 'private' localization string </remarks>
        public string Private { get; private set; }

        /// <remarks> 'restricted' localization string </remarks>
        public string Restricted { get; private set; }

        /// <remarks> 'dark' localization string </remarks>
        public string Dark { get; private set; }

    }
}
