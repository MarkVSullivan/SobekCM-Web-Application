namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the abstractItemViewer class </summary>
    public class abstractItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the abstractItemViewer_Localization class </summary>
        public abstractItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "abstractItemViewer";
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
                case "Unnumbered":
                    Unnumbered = Value;
                    break;

                case "Page":
                    Page = Value;
                    break;

            }
        }
        /// <remarks> 'Unnumbered ' localization string </remarks>
        public string Unnumbered { get; private set; }

        /// <remarks> 'Page ' localization string </remarks>
        public string Page { get; private set; }

    }
}
