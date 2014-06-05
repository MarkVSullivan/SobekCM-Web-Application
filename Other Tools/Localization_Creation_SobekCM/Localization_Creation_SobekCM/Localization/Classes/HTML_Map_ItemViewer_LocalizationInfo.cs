namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the HTML_Map_ItemViewer class </summary>
    public class HTML_Map_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the HTML_Map_ItemViewer_Localization class </summary>
        public HTML_Map_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "HTML_Map_ItemViewer";
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
                case "Click On A Sheet In The Map To View A Sheet":
                    ClickOnASheetInTheMapToViewASheet = Value;
                    break;

            }
        }
        /// <remarks> Used to display a map which is clickable to get to each subsequent page </remarks>
        public string ClickOnASheetInTheMapToViewASheet { get; private set; }

    }
}
