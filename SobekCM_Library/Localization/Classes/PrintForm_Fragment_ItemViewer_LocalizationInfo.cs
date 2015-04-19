namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the PrintForm_Fragment_ItemViewer class </summary>
    public class PrintForm_Fragment_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the PrintForm_Fragment_ItemViewer_Localization class </summary>
        public PrintForm_Fragment_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "PrintForm_Fragment_ItemViewer";
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
                case "Print Options":
                    PrintOptions = Value;
                    break;

                case "Include Brief Citation":
                    IncludeBriefCitation = Value;
                    break;

                case "Full Citation":
                    FullCitation = Value;
                    break;

                case "Citation Only":
                    CitationOnly = Value;
                    break;

                case "Print Thumbnails":
                    PrintThumbnails = Value;
                    break;

                case "Print Current View":
                    PrintCurrentView = Value;
                    break;

                case "Print Current Page":
                    PrintCurrentPage = Value;
                    break;

                case "Print All Pages":
                    PrintAllPages = Value;
                    break;

                case "Print A Range Of Pages":
                    PrintARangeOfPages = Value;
                    break;

                case "From":
                    From = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "Print":
                    Print = Value;
                    break;

                case "Close":
                    Close = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "Select The Options Below To Print This Item":
                    SelectTheOptionsBelowToPrintThisItem = Value;
                    break;

            }
        }
        /// <remarks> 'Print Options' localization string </remarks>
        public string PrintOptions { get; private set; }

        /// <remarks> 'Include brief citation?' localization string </remarks>
        public string IncludeBriefCitation { get; private set; }

        /// <remarks> 'Full citation' localization string </remarks>
        public string FullCitation { get; private set; }

        /// <remarks> 'Citation only' localization string </remarks>
        public string CitationOnly { get; private set; }

        /// <remarks> 'Print thumbnails' localization string </remarks>
        public string PrintThumbnails { get; private set; }

        /// <remarks> 'Print current view' localization string </remarks>
        public string PrintCurrentView { get; private set; }

        /// <remarks> 'Print current page' localization string </remarks>
        public string PrintCurrentPage { get; private set; }

        /// <remarks> 'Print all pages' localization string </remarks>
        public string PrintAllPages { get; private set; }

        /// <remarks> 'Print a range of pages' localization string </remarks>
        public string PrintARangeOfPages { get; private set; }

        /// <remarks> 'from' localization string </remarks>
        public string From { get; private set; }

        /// <remarks> 'to' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> 'Print' localization string </remarks>
        public string Print { get; private set; }

        /// <remarks> 'Close' localization string </remarks>
        public string Close { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> 'Select the options below to print this item' localization string </remarks>
        public string SelectTheOptionsBelowToPrintThisItem { get; private set; }

    }
}
