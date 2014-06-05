namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Item_Nav_Bar_HTML_Factory class </summary>
    public class Item_Nav_Bar_HTML_Factory_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Item_Nav_Bar_HTML_Factory_Localization class </summary>
        public Item_Nav_Bar_HTML_Factory_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Item_Nav_Bar_HTML_Factory";
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
                case "ALL ISSUES":
                    ALLISSUES = Value;
                    break;

                case "ALL VOLUMES":
                    ALLVOLUMES = Value;
                    break;

                case "CITATION":
                    CITATION = Value;
                    break;

                case "CONTAINER LIST":
                    CONTAINERLIST = Value;
                    break;

                case "DATA STRUCTURE":
                    DATASTRUCTURE = Value;
                    break;

                case "DOWNLOADS":
                    DOWNLOADS = Value;
                    break;

                case "EXPLORE DATA":
                    EXPLOREDATA = Value;
                    break;

                case "FEATURES":
                    FEATURES = Value;
                    break;

                case "FLASH VIEW":
                    FLASHVIEW = Value;
                    break;

                case "MAP COVERAGE":
                    MAPCOVERAGE = Value;
                    break;

                case "MAP IT":
                    MAPIT = Value;
                    break;

                case "MAP SEARCH":
                    MAPSEARCH = Value;
                    break;

                case "PAGE IMAGE WITH TEXT":
                    PAGEIMAGEWITHTEXT = Value;
                    break;

                case "PAGE TEXT":
                    PAGETEXT = Value;
                    break;

                case "PAGE TURNER":
                    PAGETURNER = Value;
                    break;

                case "PDF VIEWER":
                    PDFVIEWER = Value;
                    break;

                case "RELATED FLIGHTS":
                    RELATEDFLIGHTS = Value;
                    break;

                case "RELATED MAPS":
                    RELATEDMAPS = Value;
                    break;

                case "REPORTS":
                    REPORTS = Value;
                    break;

                case "RESTRICTED":
                    RESTRICTED = Value;
                    break;

                case "SEARCH":
                    SEARCH = Value;
                    break;

                case "SEARCH RESULTS":
                    SEARCHRESULTS = Value;
                    break;

                case "STANDARD":
                    STANDARD = Value;
                    break;

                case "STREETS":
                    STREETS = Value;
                    break;

                case "THUMBNAILS":
                    THUMBNAILS = Value;
                    break;

                case "VIDEO":
                    VIDEO = Value;
                    break;

                case "ZOOMABLE":
                    ZOOMABLE = Value;
                    break;

            }
        }
        /// <remarks> 'ALL ISSUES' localization string </remarks>
        public string ALLISSUES { get; private set; }

        /// <remarks> 'ALL VOLUMES' localization string </remarks>
        public string ALLVOLUMES { get; private set; }

        /// <remarks> 'CITATION' localization string </remarks>
        public string CITATION { get; private set; }

        /// <remarks> 'CONTAINER LIST' localization string </remarks>
        public string CONTAINERLIST { get; private set; }

        /// <remarks> 'DATA STRUCTURE' localization string </remarks>
        public string DATASTRUCTURE { get; private set; }

        /// <remarks> 'DOWNLOADS' localization string </remarks>
        public string DOWNLOADS { get; private set; }

        /// <remarks> 'EXPLORE DATA' localization string </remarks>
        public string EXPLOREDATA { get; private set; }

        /// <remarks> 'FEATURES' localization string </remarks>
        public string FEATURES { get; private set; }

        /// <remarks> 'FLASH VIEW' localization string </remarks>
        public string FLASHVIEW { get; private set; }

        /// <remarks> 'MAP COVERAGE' localization string </remarks>
        public string MAPCOVERAGE { get; private set; }

        /// <remarks> 'MAP IT!' localization string </remarks>
        public string MAPIT { get; private set; }

        /// <remarks> 'MAP SEARCH' localization string </remarks>
        public string MAPSEARCH { get; private set; }

        /// <remarks> 'PAGE IMAGE WITH TEXT' localization string </remarks>
        public string PAGEIMAGEWITHTEXT { get; private set; }

        /// <remarks> 'PAGE TEXT' localization string </remarks>
        public string PAGETEXT { get; private set; }

        /// <remarks> 'PAGE TURNER' localization string </remarks>
        public string PAGETURNER { get; private set; }

        /// <remarks> 'PDF VIEWER' localization string </remarks>
        public string PDFVIEWER { get; private set; }

        /// <remarks> 'RELATED FLIGHTS' localization string </remarks>
        public string RELATEDFLIGHTS { get; private set; }

        /// <remarks> 'RELATED MAPS' localization string </remarks>
        public string RELATEDMAPS { get; private set; }

        /// <remarks> 'REPORTS' localization string </remarks>
        public string REPORTS { get; private set; }

        /// <remarks> 'RESTRICTED' localization string </remarks>
        public string RESTRICTED { get; private set; }

        /// <remarks> 'SEARCH' localization string </remarks>
        public string SEARCH { get; private set; }

        /// <remarks> 'SEARCH RESULTS' localization string </remarks>
        public string SEARCHRESULTS { get; private set; }

        /// <remarks> 'STANDARD' localization string </remarks>
        public string STANDARD { get; private set; }

        /// <remarks> 'STREETS' localization string </remarks>
        public string STREETS { get; private set; }

        /// <remarks> 'THUMBNAILS' localization string </remarks>
        public string THUMBNAILS { get; private set; }

        /// <remarks> 'VIDEO' localization string </remarks>
        public string VIDEO { get; private set; }

        /// <remarks> 'ZOOMABLE' localization string </remarks>
        public string ZOOMABLE { get; private set; }

    }
}
