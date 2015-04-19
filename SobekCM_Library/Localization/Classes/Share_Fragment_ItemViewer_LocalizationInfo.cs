namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Share_Fragment_ItemViewer class </summary>
    public class Share_Fragment_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Share_Fragment_ItemViewer_Localization class </summary>
        public Share_Fragment_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Share_Fragment_ItemViewer";
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
                case "Add To Favorites":
                    AddToFavorites = Value;
                    break;

            }
        }
        /// <remarks> 'Add to favorites' localization string </remarks>
        public string AddToFavorites { get; private set; }

    }
}
