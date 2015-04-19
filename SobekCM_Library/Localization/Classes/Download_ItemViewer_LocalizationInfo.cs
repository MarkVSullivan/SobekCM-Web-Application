namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Download_ItemViewer class </summary>
    public class Download_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Download_ItemViewer_Localization class </summary>
        public Download_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Download_ItemViewer";
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
                case "This Item Has The Following Downloads":
                    ThisItemHasTheFollowingDownloads = Value;
                    break;

                case "This Item Is Only Available As The Following Downloads":
                    ThisItemIsOnlyAvailableAsTheFollowingDownl = Value;
                    break;

                case "To Download Right Click On The Tile Name Below Select Save Link As And Save The JPEG2000 To Your Local Computer":
                    ToDownloadRightClickOnTheTileNameBelowSel = Value;
                    break;

                case "To Download Right Click On The Tile Name Below Select Save Target As And Save The JPEG2000 To Your Local Computer":
                    ToDownloadRightClickOnTheTileNameBelowSel = Value;
                    break;

                case "The Following Tiles Are Available For Download":
                    TheFollowingTilesAreAvailableForDownload = Value;
                    break;

            }
        }
        /// <remarks> 'This item has the following downloads:' localization string </remarks>
        public string ThisItemHasTheFollowingDownloads { get; private set; }

        /// <remarks> 'This item is only available as the following downloads:' localization string </remarks>
        public string ThisItemIsOnlyAvailableAsTheFollowingDownl { get; private set; }

        /// <remarks> '"To download, right click on the tile name below, select 'Save Link As...' and save the JPEG2000 to your local computer."' localization string </remarks>
        public string ToDownloadRightClickOnTheTileNameBelowSel { get; private set; }

        /// <remarks> '"To download, right click on the tile name below, select 'Save Target As...' and save the JPEG2000 to your local computer. "' localization string </remarks>
        public string ToDownloadRightClickOnTheTileNameBelowSel2 { get; private set; }

        /// <remarks> 'The following tiles are available for download:' localization string </remarks>
        public string TheFollowingTilesAreAvailableForDownload { get; private set; }

    }
}
