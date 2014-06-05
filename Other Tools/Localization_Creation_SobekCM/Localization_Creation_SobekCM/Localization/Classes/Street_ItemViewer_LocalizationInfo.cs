namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Street_ItemViewer class </summary>
    public class Street_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Street_ItemViewer_Localization class </summary>
        public Street_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Street_ItemViewer";
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
                case "To Report This Issue":
                    ToReportThisIssue = Value;
                    break;

                case "Click":
                    Click = Value;
                    break;

                case "Here":
                    Here = Value;
                    break;

                case "Index Of Streets":
                    IndexOfStreets = Value;
                    break;

                case "UNABLE TO LOAD STREETS FROM DATABASE":
                    UNABLETOLOADSTREETSFROMDATABASE = Value;
                    break;

            }
        }
        /// <remarks> ' to report this issue.' localization string </remarks>
        public string ToReportThisIssue { get; private set; }

        /// <remarks> 'Click' localization string </remarks>
        public string Click { get; private set; }

        /// <remarks> 'here' localization string </remarks>
        public string Here { get; private set; }

        /// <remarks> 'Index of Streets' localization string </remarks>
        public string IndexOfStreets { get; private set; }

        /// <remarks> 'UNABLE TO LOAD STREETS FROM DATABASE' localization string </remarks>
        public string UNABLETOLOADSTREETSFROMDATABASE { get; private set; }

    }
}
