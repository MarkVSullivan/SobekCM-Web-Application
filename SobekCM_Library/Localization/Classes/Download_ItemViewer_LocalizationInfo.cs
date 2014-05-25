namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Download_ItemViewer class </summary>
    public class Download_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Download_ItemViewer_Localization class </summary>
        public Download_ItemViewer_LocalizationInfo() : base()
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
                case "The Following Tiles Are Available For Download":
                    TheFollowingTilesAreAvailableForDownload = Value;
                    break;

                case "This Item Has The Following Downloads":
                    ThisItemHasTheFollowingDownloads = Value;
                    break;

                case "This Item Is Only Available As The Following Downloads":
                    ThisItemIsOnlyAvailableAsTheFollowingDownloads = Value;
                    break;

                case "To Download Right Click On The Tile Name Below Select Save Link As And Save The JPEG2000 To Your Local Computer":
                    ToDownloadRightClickOnTheTileNameBelowSelectSaveLinkAsAndSaveTheJPEG2000ToYourLocalComputer = Value;
                    break;

                case "To Download Right Click On The Tile Name Below Select Save Target As And Save The JPEG2000 To Your Local Computer":
                    ToDownloadRightClickOnTheTileNameBelowSelectSaveTargetAsAndSaveTheJPEG2000ToYourLocalComputer = Value;
                    break;

            }
        }
        /// <remarks> 'The following tiles are available for download:' localization string </remarks>
        public string TheFollowingTilesAreAvailableForDownload { get; private set; }

        /// <remarks> If there are pages and also downloads </remarks>
        public string ThisItemHasTheFollowingDownloads { get; private set; }

        /// <remarks> If there are only downloads for this item (no pages) </remarks>
        public string ThisItemIsOnlyAvailableAsTheFollowingDownloads { get; private set; }

        /// <remarks> '"To download, right click on the tile name below, select 'Save Link As...' and save the JPEG2000 to your local computer."' localization string </remarks>
        public string ToDownloadRightClickOnTheTileNameBelowSelectSaveLinkAsAndSaveTheJPEG2000ToYourLocalComputer { get; private set; }

        /// <remarks> '"To download, right click on the tile name below, select 'Save Target As...' and save the JPEG2000 to your local computer. "' localization string </remarks>
        public string ToDownloadRightClickOnTheTileNameBelowSelectSaveTargetAsAndSaveTheJPEG2000ToYourLocalComputer { get; private set; }

    }
}
