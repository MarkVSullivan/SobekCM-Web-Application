namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Describe_Fragment_ItemViewer class </summary>
    public class Describe_Fragment_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Describe_Fragment_ItemViewer_Localization class </summary>
        public Describe_Fragment_ItemViewer_LocalizationInfo() : base()
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
                case "Add Item Description":
                    AddItemDescription = Value;
                    break;

                case "Close":
                    Close = Value;
                    break;

                case "Enter A Description Or Notes To Add To This Item":
                    EnterADescriptionOrNotesToAddToThisItem = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

            }
        }
        /// <remarks> Title for the pop-up form for adding a description to an item </remarks>
        public string AddItemDescription { get; private set; }

        /// <remarks> 'Close' localization string </remarks>
        public string Close { get; private set; }

        /// <remarks> Pop-up form for adding a description to an item </remarks>
        public string EnterADescriptionOrNotesToAddToThisItem { get; private set; }

        /// <remarks> Pop-up form for adding a description to an item </remarks>
        public string Notes { get; private set; }

    }
}
