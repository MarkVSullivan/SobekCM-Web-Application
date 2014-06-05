namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Checked_Out_ItemViewer class </summary>
    public class Checked_Out_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Checked_Out_ItemViewer_Localization class </summary>
        public Checked_Out_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Checked_Out_ItemViewer";
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
                case "The Item You Have Requested Contains Copyright Material And Is Reserved For Singleuse Br Br Someone Has Currently Checked Out This Digital Copy For Viewing Br Br Please Try Again In Several Minutes":
                    TheItemYouHaveRequestedContainsCopyrightMaterialAndIsReservedForSingleuseBrBrSomeoneHasCurrentlyCheckedOutThisDigitalCopyForViewingBrBrPleaseTryAgainInSeveralMinutes = Value;
                    break;

            }
        }
        /// <remarks> If an item is marked for single use and someone else is already using the item </remarks>
        public string TheItemYouHaveRequestedContainsCopyrightMaterialAndIsReservedForSingleuseBrBrSomeoneHasCurrentlyCheckedOutThisDigitalCopyForViewingBrBrPleaseTryAgainInSeveralMinutes { get; private set; }

    }
}
