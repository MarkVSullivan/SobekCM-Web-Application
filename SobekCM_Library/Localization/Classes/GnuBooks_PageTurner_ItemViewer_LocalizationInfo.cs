namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the GnuBooks_PageTurner_ItemViewer class </summary>
    public class GnuBooks_PageTurner_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the GnuBooks_PageTurner_ItemViewer_Localization class </summary>
        public GnuBooks_PageTurner_ItemViewer_LocalizationInfo() : base()
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
                case "Book Turner Presentations Require A Javascriptenabled Browser":
                    BookTurnerPresentationsRequireAJavascriptenabledBrowser = Value;
                    break;

                case "Return To Item":
                    ReturnToItem = Value;
                    break;

                case "Zoom":
                    Zoom = Value;
                    break;

            }
        }
        /// <remarks> 'Book Turner presentations require a Javascript-enabled browser.' localization string </remarks>
        public string BookTurnerPresentationsRequireAJavascriptenabledBrowser { get; private set; }

        /// <remarks> 'Return to Item' localization string </remarks>
        public string ReturnToItem { get; private set; }

        /// <remarks> 'Zoom' localization string </remarks>
        public string Zoom { get; private set; }

    }
}
