namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Item_Email_Helper class </summary>
    public class Item_Email_Helper_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Item_Email_Helper_Localization class </summary>
        public Item_Email_Helper_LocalizationInfo() : base()
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
                case "XXX Wanted You To See This Item On XXX And Included The Following Comments":
                    XXXWantedYouToSeeThisItemOnXXXAndIncludedTheFollowingComments = Value;
                    break;

                case "XXX Wanted You To See This Item On XXX":
                    XXXWantedYouToSeeThisItemOnXXX = Value;
                    break;

                case "Abbreviated Title XXX":
                    AbbreviatedTitleXXX = Value;
                    break;

                case "Alternate Title XXX":
                    AlternateTitleXXX = Value;
                    break;

                case "BLOCKED THUMBNAIL IMAGE":
                    BLOCKEDTHUMBNAILIMAGE = Value;
                    break;

                case "Creator XXX":
                    CreatorXXX = Value;
                    break;

                case "Date XXX":
                    DateXXX = Value;
                    break;

                case "Description XXX":
                    DescriptionXXX = Value;
                    break;

                case "Genre XXX":
                    GenreXXX = Value;
                    break;

                case "ITEM INFORMATION":
                    ITEMINFORMATION = Value;
                    break;

                case "Publisher XXX":
                    PublisherXXX = Value;
                    break;

                case "Rights XXX":
                    RightsXXX = Value;
                    break;

                case "Series Title XXX":
                    SeriesTitleXXX = Value;
                    break;

                case "Spatial Coverage XXX":
                    SpatialCoverageXXX = Value;
                    break;

                case "Subject XXX":
                    SubjectXXX = Value;
                    break;

                case "Translated Title XXX":
                    TranslatedTitleXXX = Value;
                    break;

                case "Uniform Title XXX":
                    UniformTitleXXX = Value;
                    break;

            }
        }
        /// <remarks> "%1=username, %2=SobekCM instance abbreviation (like UFDC, dLOC). When someone emails an item from the system" </remarks>
        public string XXXWantedYouToSeeThisItemOnXXXAndIncludedTheFollowingComments { get; private set; }

        /// <remarks> "%1=username, %2=SobekCM instance abbreviation (like UFDC, dLOC). When someone emails an item from the system" </remarks>
        public string XXXWantedYouToSeeThisItemOnXXX { get; private set; }

        /// <remarks> 'Abbreviated Title: %1' localization string </remarks>
        public string AbbreviatedTitleXXX { get; private set; }

        /// <remarks> 'Alternate Title: %1' localization string </remarks>
        public string AlternateTitleXXX { get; private set; }

        /// <remarks> 'BLOCKED THUMBNAIL IMAGE' localization string </remarks>
        public string BLOCKEDTHUMBNAILIMAGE { get; private set; }

        /// <remarks> 'Creator: %1' localization string </remarks>
        public string CreatorXXX { get; private set; }

        /// <remarks> 'Date: %1' localization string </remarks>
        public string DateXXX { get; private set; }

        /// <remarks> 'Description: %1' localization string </remarks>
        public string DescriptionXXX { get; private set; }

        /// <remarks> 'Genre: %1' localization string </remarks>
        public string GenreXXX { get; private set; }

        /// <remarks> 'ITEM INFORMATION' localization string </remarks>
        public string ITEMINFORMATION { get; private set; }

        /// <remarks> 'Publisher: %1' localization string </remarks>
        public string PublisherXXX { get; private set; }

        /// <remarks> 'Rights: %1' localization string </remarks>
        public string RightsXXX { get; private set; }

        /// <remarks> 'Series Title: %1' localization string </remarks>
        public string SeriesTitleXXX { get; private set; }

        /// <remarks> 'Spatial Coverage: %1' localization string </remarks>
        public string SpatialCoverageXXX { get; private set; }

        /// <remarks> 'Subject: %1' localization string </remarks>
        public string SubjectXXX { get; private set; }

        /// <remarks> 'Translated Title: %1' localization string </remarks>
        public string TranslatedTitleXXX { get; private set; }

        /// <remarks> 'Uniform Title: %1' localization string </remarks>
        public string UniformTitleXXX { get; private set; }

    }
}
